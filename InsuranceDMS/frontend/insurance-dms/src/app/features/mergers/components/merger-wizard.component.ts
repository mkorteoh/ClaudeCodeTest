import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { AgencySummary, Merger, MergerPreview } from '../../../core/models/api.models';

type WizardStep = 1 | 2 | 3;

@Component({
  selector: 'app-merger-wizard',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page">
      <div class="breadcrumb"><a routerLink="/mergers">Mergers</a> / New Merger</div>
      <div class="page-header"><h1>Initiate Merger</h1></div>

      <div class="wizard-steps">
        <div class="wizard-step" [class.active]="step() === 1" [class.done]="step() > 1">1. Select Agencies</div>
        <div class="wizard-step" [class.active]="step() === 2" [class.done]="step() > 2">2. Preview</div>
        <div class="wizard-step" [class.active]="step() === 3">3. Confirm & Execute</div>
      </div>

      @if (error()) { <div class="alert alert-error">{{ error() }}</div> }
      @if (success()) { <div class="alert alert-success">{{ success() }}</div> }

      <!-- Step 1: Select Agencies -->
      @if (step() === 1) {
        <div class="card">
          <div class="card-header">Select Surviving Agency</div>
          <div class="form-group">
            <label class="form-label">Surviving Agency (will absorb others)</label>
            <select class="form-control" [(ngModel)]="survivingAgencyId" style="max-width:400px">
              <option value="">Select agency...</option>
              @for (a of agencies(); track a.id) {
                <option [value]="a.id">{{ a.agencyName }} ({{ tierLabel(a.agencyTier) }})</option>
              }
            </select>
          </div>

          <div class="card-header" style="margin-top:16px">Select Absorbed Agencies</div>
          <div style="max-height:300px;overflow-y:auto;border:1px solid #e5e7eb;border-radius:6px">
            @for (a of agencies(); track a.id) {
              @if (a.id !== +survivingAgencyId && !a.isMerged) {
                <label style="display:flex;align-items:center;gap:10px;padding:10px 12px;cursor:pointer;border-bottom:1px solid #f3f4f6">
                  <input type="checkbox" [value]="a.id" [checked]="isAbsorbed(a.id)" (change)="toggleAbsorbed(a.id)" />
                  <span>{{ a.agencyName }} ({{ tierLabel(a.agencyTier) }})</span>
                </label>
              }
            }
          </div>

          <div class="form-group" style="margin-top:16px">
            <label class="form-label">Notes</label>
            <textarea class="form-control" [(ngModel)]="notes" rows="3"></textarea>
          </div>

          <div style="margin-top:16px">
            <button class="btn btn-primary" [disabled]="!survivingAgencyId || absorbedIds().length === 0 || saving()" (click)="createMerger()">
              {{ saving() ? 'Creating...' : 'Create Merger & Preview →' }}
            </button>
          </div>
        </div>
      }

      <!-- Step 2: Preview -->
      @if (step() === 2 && preview()) {
        <div class="card">
          <div class="card-header">Merger Preview</div>
          <p><strong>Surviving:</strong> {{ preview()!.survivingAgencyName }}</p>
          <p><strong>Total personnel to transfer:</strong> {{ preview()!.totalPersonnelToTransfer }}</p>

          @if (preview()!.conflicts.length > 0) {
            <div class="alert alert-warning">
              <strong>⚠ Conflicts detected:</strong>
              @for (c of preview()!.conflicts; track c) { <div>• {{ c }}</div> }
            </div>
          }

          @for (a of preview()!.absorbedAgencies; track a.agencyId) {
            <div style="border:1px solid #e5e7eb;border-radius:6px;padding:16px;margin-top:12px">
              <div style="font-weight:600;margin-bottom:8px">{{ a.agencyName }}</div>
              <div class="form-row">
                <div><div class="form-label">Personnel</div><div>{{ a.personnelCount }}</div></div>
                <div><div class="form-label">Producers</div><div>{{ a.producerCount }}</div></div>
                <div><div class="form-label">Licenses</div><div>{{ a.licenseCount }}</div></div>
                <div><div class="form-label">Appointments</div><div>{{ a.appointmentCount }}</div></div>
              </div>
              @if (a.duplicateNPNs.length > 0) {
                <div style="margin-top:8px;color:#e02424;font-size:12px">Duplicate NPNs: {{ a.duplicateNPNs.join(', ') }}</div>
              }
            </div>
          }

          <div style="display:flex;gap:8px;margin-top:16px">
            <button class="btn btn-primary" (click)="goToConfirm()">Proceed to Confirm →</button>
            <button class="btn btn-secondary" (click)="cancelMerger()">Cancel Merger</button>
          </div>
        </div>
      }

      <!-- Step 3: Confirm & Execute -->
      @if (step() === 3) {
        <div class="card">
          <div class="card-header">Confirm & Execute Merger</div>
          <div class="alert alert-warning">
            ⚠ <strong>This action is irreversible.</strong> All personnel from absorbed agencies will be transferred to the surviving agency, and absorbed agencies will be marked as merged.
          </div>
          <p>Surviving agency: <strong>{{ preview()?.survivingAgencyName }}</strong></p>
          <p>Agencies to be absorbed: <strong>{{ absorbedAgencyNames() }}</strong></p>
          <p>Personnel to transfer: <strong>{{ preview()?.totalPersonnelToTransfer }}</strong></p>

          <div style="display:flex;gap:8px;margin-top:16px">
            <button class="btn btn-danger" [disabled]="executing()" (click)="execute()">
              {{ executing() ? 'Executing...' : '✓ Execute Merger' }}
            </button>
            <button class="btn btn-secondary" (click)="step.set(2)">← Back to Preview</button>
          </div>
        </div>
      }
    </div>
  `
})
export class MergerWizardComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);

  step = signal<WizardStep>(1);
  agencies = signal<AgencySummary[]>([]);
  survivingAgencyId = '';
  absorbedIds = signal<number[]>([]);
  notes = '';
  saving = signal(false);
  executing = signal(false);
  error = signal('');
  success = signal('');
  merger = signal<Merger | null>(null);
  preview = signal<MergerPreview | null>(null);

  ngOnInit() {
    this.api.getAgencies({ isActive: true, pageSize: 200 }).subscribe(r => this.agencies.set(r.data));
  }

  isAbsorbed(id: number): boolean { return this.absorbedIds().includes(id); }

  toggleAbsorbed(id: number) {
    this.absorbedIds.update(ids =>
      ids.includes(id) ? ids.filter(i => i !== id) : [...ids, id]
    );
  }

  createMerger() {
    this.saving.set(true); this.error.set('');
    this.api.createMerger({
      survivingAgencyId: Number(this.survivingAgencyId),
      absorbedAgencyIds: this.absorbedIds(),
      notes: this.notes
    }).subscribe({
      next: r => {
        this.merger.set(r.data);
        this.loadPreview(r.data.id);
      },
      error: e => { this.error.set(e.error?.errors?.[0] ?? 'Failed to create merger'); this.saving.set(false); }
    });
  }

  loadPreview(mergerId: number) {
    this.api.getMergerPreview(mergerId).subscribe({
      next: r => { this.preview.set(r.data); this.step.set(2); this.saving.set(false); },
      error: e => { this.error.set(e.error?.errors?.[0] ?? 'Failed to load preview'); this.saving.set(false); }
    });
  }

  goToConfirm() { this.step.set(3); }

  execute() {
    this.executing.set(true); this.error.set('');
    this.api.executeMerger(this.merger()!.id).subscribe({
      next: r => {
        this.success.set('Merger executed successfully!');
        setTimeout(() => this.router.navigate(['/mergers', r.data.id]), 1500);
      },
      error: e => { this.error.set(e.error?.errors?.[0] ?? 'Execution failed'); this.executing.set(false); }
    });
  }

  cancelMerger() {
    if (this.merger()) {
      this.api.cancelMerger(this.merger()!.id).subscribe(() => this.router.navigate(['/mergers']));
    }
  }

  tierLabel(tier: number) { return { 1: 'FMO', 2: 'MGA', 3: 'GA', 4: 'Agent' }[tier] ?? tier; }

  absorbedAgencyNames(): string {
    return (this.preview()?.absorbedAgencies ?? []).map(a => a.agencyName).join(', ');
  }
}
