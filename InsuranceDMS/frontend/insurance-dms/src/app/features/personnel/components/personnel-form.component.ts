import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { AgencySummary, State } from '../../../core/models/api.models';

@Component({
  selector: 'app-personnel-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="page">
      <div class="breadcrumb"><a routerLink="/personnel">Personnel</a> / {{ isEdit() ? 'Edit' : 'New Personnel' }}</div>
      <div class="page-header"><h1>{{ isEdit() ? 'Edit Personnel' : 'Add Personnel' }}</h1></div>
      @if (error()) { <div class="alert alert-error">{{ error() }}</div> }
      <form [formGroup]="form" (ngSubmit)="submit()">
        <div class="card" style="margin-bottom:16px">
          <div class="card-header">Personnel Details</div>
          <div class="form-row">
            <div class="form-group"><label class="form-label">First Name *</label><input class="form-control" formControlName="firstName" /></div>
            <div class="form-group"><label class="form-label">Last Name *</label><input class="form-control" formControlName="lastName" /></div>
            <div class="form-group"><label class="form-label">Middle Name</label><input class="form-control" formControlName="middleName" /></div>
            <div class="form-group"><label class="form-label">Type *</label>
              <select class="form-control" formControlName="personnelType">
                <option value="1">Producer</option><option value="2">CSR</option>
                <option value="3">Manager</option><option value="4">Principal</option>
              </select>
            </div>
            <div class="form-group"><label class="form-label">Agency *</label>
              <select class="form-control" formControlName="agencyId">
                <option value="">Select agency...</option>
                @for (a of agencies(); track a.id) {
                  <option [value]="a.id">{{ a.agencyName }}</option>
                }
              </select>
            </div>
            <div class="form-group"><label class="form-label">Title</label><input class="form-control" formControlName="title" /></div>
            <div class="form-group"><label class="form-label">Email</label><input class="form-control" formControlName="email" type="email" /></div>
            <div class="form-group"><label class="form-label">Phone</label><input class="form-control" formControlName="phone" /></div>
            <div class="form-group"><label class="form-label">Hire Date</label><input class="form-control" formControlName="hireDate" type="date" /></div>
          </div>
        </div>

        @if (isProducer()) {
          <div class="card" style="margin-bottom:16px">
            <div class="card-header">Producer Details</div>
            <div class="form-row">
              <div class="form-group"><label class="form-label">NPN</label><input class="form-control" formControlName="npn" /></div>
              <div class="form-group"><label class="form-label">Resident State</label>
                <select class="form-control" formControlName="residentState">
                  <option value="">Select...</option>
                  @for (s of states(); track s.stateCode) { <option [value]="s.stateCode">{{ s.stateName }}</option> }
                </select>
              </div>
              <div class="form-group"><label class="form-label">SSN Last 4</label><input class="form-control" formControlName="ssnLast4" maxlength="4" /></div>
              <div class="form-group"><label class="form-label">Date of Birth</label><input class="form-control" formControlName="dateOfBirth" type="date" /></div>
              <div class="form-group"><label class="form-label">E&O Expiration</label><input class="form-control" formControlName="eoExpirationDate" type="date" /></div>
            </div>
          </div>
        }

        <div style="display:flex;gap:8px">
          <button class="btn btn-primary" type="submit" [disabled]="saving()">{{ saving() ? 'Saving...' : (isEdit() ? 'Update' : 'Create') }}</button>
          <a class="btn btn-secondary" routerLink="/personnel">Cancel</a>
        </div>
      </form>
    </div>
  `
})
export class PersonnelFormComponent implements OnInit {
  private api = inject(ApiService); private router = inject(Router);
  private route = inject(ActivatedRoute); private fb = inject(FormBuilder);

  isEdit = signal(false); saving = signal(false); error = signal('');
  editId = signal<number | null>(null);
  agencies = signal<AgencySummary[]>([]); states = signal<State[]>([]);

  form = this.fb.group({
    firstName: ['', Validators.required], lastName: ['', Validators.required],
    middleName: [''], personnelType: [1, Validators.required],
    agencyId: [null as number | null, Validators.required],
    title: [''], email: [''], phone: [''], hireDate: [''],
    npn: [''], residentState: [''], ssnLast4: [''], dateOfBirth: [''], eoExpirationDate: ['']
  });

  isProducer = computed(() => Number(this.form.get('personnelType')?.value) === 1);

  ngOnInit() {
    this.api.getAgencies({ pageSize: 200 }).subscribe(r => this.agencies.set(r.data));
    this.api.getStates().subscribe(s => this.states.set(s));
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true); this.editId.set(Number(id));
      this.api.getPersonnelById(Number(id)).subscribe(r => {
        const p = r.data;
        this.form.patchValue({
          firstName: p.firstName, lastName: p.lastName, middleName: p.middleName ?? '',
          personnelType: p.personnelType, agencyId: p.agencyId, title: p.title ?? '',
          email: p.email ?? '', phone: p.phone ?? '',
          hireDate: p.hireDate ? p.hireDate.substring(0, 10) : '',
          npn: p.producer?.npn ?? '', residentState: p.producer?.residentState ?? '',
          ssnLast4: p.producer?.ssnLast4 ?? '',
          dateOfBirth: p.producer?.dateOfBirth ? p.producer.dateOfBirth.substring(0, 10) : '',
          eoExpirationDate: p.producer?.eoExpirationDate ? p.producer.eoExpirationDate.substring(0, 10) : ''
        });
      });
    }
  }

  submit() {
    if (this.form.invalid) return;
    this.saving.set(true); this.error.set('');
    const val = this.form.value;
    const body = { ...val, personnelType: Number(val.personnelType), agencyId: Number(val.agencyId) };
    const req = this.isEdit()
      ? this.api.updatePersonnel(this.editId()!, body as any)
      : this.api.createPersonnel(body as any);
    req.subscribe({
      next: r => this.router.navigate(['/personnel', r.data.id]),
      error: e => { this.error.set(e.error?.errors?.[0] ?? 'Failed to save'); this.saving.set(false); }
    });
  }
}
