import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { State, AgencySummary } from '../../../core/models/api.models';

@Component({
  selector: 'app-agency-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="page">
      <div class="breadcrumb"><a routerLink="/agencies">Agencies</a> / {{ isEdit() ? 'Edit' : 'New Agency' }}</div>
      <div class="page-header"><h1>{{ isEdit() ? 'Edit Agency' : 'Create Agency' }}</h1></div>

      @if (error()) {
        <div class="alert alert-error">{{ error() }}</div>
      }

      <form [formGroup]="form" (ngSubmit)="submit()">
        <div class="card" style="margin-bottom:16px">
          <div class="card-header">Basic Information</div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Agency Name *</label>
              <input class="form-control" formControlName="agencyName" />
            </div>
            <div class="form-group">
              <label class="form-label">NPN</label>
              <input class="form-control" formControlName="npn" />
            </div>
            <div class="form-group">
              <label class="form-label">Tax ID</label>
              <input class="form-control" formControlName="taxId" />
            </div>
            <div class="form-group">
              <label class="form-label">Tier *</label>
              <select class="form-control" formControlName="agencyTier">
                <option value="1">FMO</option>
                <option value="2">MGA</option>
                <option value="3">GA</option>
                <option value="4">Agent</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Parent Agency</label>
              <select class="form-control" formControlName="parentAgencyId">
                <option value="">None</option>
                @for (a of agencies(); track a.id) {
                  <option [value]="a.id">{{ a.agencyName }}</option>
                }
              </select>
            </div>
          </div>
        </div>

        <div class="card" style="margin-bottom:16px">
          <div class="card-header">Notes</div>
          <textarea class="form-control" formControlName="notes" rows="3"></textarea>
        </div>

        @if (!isEdit()) {
          <div class="card" style="margin-bottom:16px">
            <div class="card-header">Corporate Office Location <span style="color:var(--danger)">*</span></div>
            <div formGroupName="initialLocation">
              <div class="form-row">
                <div class="form-group">
                  <label class="form-label">Location Name *</label>
                  <input class="form-control" formControlName="locationName" placeholder="e.g. Main Office" />
                </div>
                <div class="form-group">
                  <label class="form-label">Phone</label>
                  <input class="form-control" formControlName="phone" />
                </div>
                <div class="form-group">
                  <label class="form-label">Email</label>
                  <input class="form-control" formControlName="email" />
                </div>
                <div class="form-group">
                  <label class="form-label">Address Line 1</label>
                  <input class="form-control" formControlName="addressLine1" />
                </div>
                <div class="form-group">
                  <label class="form-label">City</label>
                  <input class="form-control" formControlName="city" />
                </div>
                <div class="form-group">
                  <label class="form-label">State</label>
                  <select class="form-control" formControlName="stateCode">
                    <option value="">Select state...</option>
                    @for (s of states(); track s.stateCode) {
                      <option [value]="s.stateCode">{{ s.stateName }}</option>
                    }
                  </select>
                </div>
                <div class="form-group">
                  <label class="form-label">Zip Code</label>
                  <input class="form-control" formControlName="zipCode" />
                </div>
              </div>
            </div>
          </div>
        }

        <div style="display:flex;gap:8px">
          <button class="btn btn-primary" type="submit" [disabled]="saving()">
            {{ saving() ? 'Saving...' : (isEdit() ? 'Update Agency' : 'Create Agency') }}
          </button>
          <a class="btn btn-secondary" routerLink="/agencies">Cancel</a>
        </div>
      </form>
    </div>
  `
})
export class AgencyFormComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  isEdit = signal(false);
  saving = signal(false);
  error = signal('');
  states = signal<State[]>([]);
  agencies = signal<AgencySummary[]>([]);
  editId = signal<number | null>(null);

  form = this.fb.group({
    agencyName: ['', Validators.required],
    npn: [''],
    taxId: [''],
    agencyTier: [1, Validators.required],
    parentAgencyId: [null as number | null],
    notes: [''],
    initialLocation: this.fb.group({
      locationName: ['', Validators.required],
      phone: [''], email: [''], website: [''],
      addressLine1: [''], addressLine2: [''], city: [''],
      stateCode: [''], zipCode: [''], county: ['']
    })
  });

  ngOnInit() {
    this.api.getStates().subscribe(s => this.states.set(s));
    this.api.getAgencies({ pageSize: 200 }).subscribe(r => this.agencies.set(r.data));

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true);
      this.editId.set(Number(id));
      this.form.get('initialLocation')?.disable();
      this.api.getAgency(Number(id)).subscribe(r => {
        const a = r.data;
        this.form.patchValue({
          agencyName: a.agencyName, npn: a.npn ?? '', taxId: a.taxId ?? '',
          agencyTier: a.agencyTier, parentAgencyId: a.parentAgencyId ?? null,
          notes: a.notes ?? ''
        });
      });
    }
  }

  submit() {
    if (this.form.invalid) return;
    this.saving.set(true);
    this.error.set('');
    const val = this.form.value;
    const body: Record<string, any> = {
      agencyName: val.agencyName,
      npn: val.npn,
      taxId: val.taxId,
      agencyTier: Number(val.agencyTier),
      parentAgencyId: val.parentAgencyId || null,
      notes: val.notes
    };

    if (!this.isEdit() && val.initialLocation?.locationName) {
      body['initialLocation'] = { ...val.initialLocation, isCorporateOffice: true };
    }

    if (this.isEdit()) {
      body['isActive'] = true;
    }

    const req = this.isEdit()
      ? this.api.updateAgency(this.editId()!, body)
      : this.api.createAgency(body);

    req.subscribe({
      next: r => this.router.navigate(['/agencies', r.data.id]),
      error: e => { this.error.set(e.error?.errors?.[0] ?? 'Failed to save'); this.saving.set(false); }
    });
  }
}
