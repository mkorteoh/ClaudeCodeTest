import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { Agency, Personnel, AgencySummary, EntityLineage, AgencyLocation, PersonnelLocation, AgencyTier, State } from '../../../core/models/api.models';

type Tab = 'profile' | 'personnel' | 'hierarchy' | 'lineage' | 'locations';

@Component({
  selector: 'app-agency-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule, FormsModule],
  template: `
    <div class="page">
      @if (agency()) {
        <div class="breadcrumb"><a routerLink="/agencies">Agencies</a> / {{ agency()!.agencyName }}</div>
        <div class="page-header">
          <h1>{{ agency()!.agencyName }}</h1>
          <div style="display:flex;gap:8px">
            <a class="btn btn-secondary" routerLink="/agencies/{{ agency()!.id }}/edit">Edit</a>
          </div>
        </div>

        <div class="tabs">
          <div class="tab" [class.active]="activeTab() === 'profile'" (click)="setTab('profile')">Profile</div>
          <div class="tab" [class.active]="activeTab() === 'locations'" (click)="setTab('locations')">Locations</div>
          <div class="tab" [class.active]="activeTab() === 'personnel'" (click)="setTab('personnel')">Personnel</div>
          <div class="tab" [class.active]="activeTab() === 'hierarchy'" (click)="setTab('hierarchy')">Hierarchy</div>
          <div class="tab" [class.active]="activeTab() === 'lineage'" (click)="setTab('lineage')">Lineage</div>
        </div>

        @if (activeTab() === 'profile') {
          <div class="card">
            <div class="form-row">
              <div><div class="form-label">NPN</div><div>{{ agency()!.npn || '—' }}</div></div>
              <div><div class="form-label">Tax ID</div><div>{{ agency()!.taxId || '—' }}</div></div>
              <div><div class="form-label">Tier</div><div>{{ tierLabel(agency()!.agencyTier) }}</div></div>
              <div><div class="form-label">Status</div>
                <span [class]="'badge ' + (agency()!.isMerged ? 'badge-merged' : agency()!.isActive ? 'badge-active' : 'badge-inactive')">
                  {{ agency()!.isMerged ? 'Merged' : agency()!.isActive ? 'Active' : 'Inactive' }}
                </span>
              </div>
              @if (corpOffice()) {
                <div><div class="form-label">Phone</div><div>{{ corpOffice()!.phone || '—' }}</div></div>
                <div><div class="form-label">Email</div><div>{{ corpOffice()!.email || '—' }}</div></div>
                <div><div class="form-label">Corporate Office</div>
                  <div>{{ corpOffice()!.addressLine1 }}<br>{{ corpOffice()!.city }}, {{ corpOffice()!.stateCode }} {{ corpOffice()!.zipCode }}</div>
                </div>
              }
              <div><div class="form-label">Notes</div><div>{{ agency()!.notes || '—' }}</div></div>
            </div>
          </div>
        }

        @if (activeTab() === 'locations') {
          <div class="card">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px">
              <div class="card-header" style="margin:0">Locations</div>
              <button class="btn btn-primary" (click)="showAddLocation.set(!showAddLocation())">
                {{ showAddLocation() ? 'Cancel' : '+ Add Location' }}
              </button>
            </div>

            @if (locationError()) {
              <div class="alert alert-error" style="margin-bottom:16px">{{ locationError() }}</div>
            }

            @if (showAddLocation()) {
              <div style="border:1px solid #e5e7eb;border-radius:6px;padding:16px;margin-bottom:16px;background:#f9fafb">
                <div class="card-header">New Location</div>
                <form [formGroup]="locationForm" (ngSubmit)="saveLocation()">
                  <div class="form-row">
                    <div class="form-group">
                      <label class="form-label">Location Name *</label>
                      <input class="form-control" formControlName="locationName" />
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
                  <div class="form-group">
                    <label style="display:flex;align-items:center;gap:8px;cursor:pointer">
                      <input type="checkbox" formControlName="isCorporateOffice" />
                      Corporate Office
                    </label>
                  </div>
                  <div style="display:flex;gap:8px;margin-top:12px">
                    <button class="btn btn-primary" type="submit" [disabled]="locationForm.invalid || savingLocation()">
                      {{ savingLocation() ? 'Saving...' : 'Save Location' }}
                    </button>
                    <button class="btn btn-secondary" type="button" (click)="showAddLocation.set(false)">Cancel</button>
                  </div>
                </form>
              </div>
            }

            @if (locations().length === 0) {
              <div class="empty-state"><h3>No locations</h3><p>Add a location to get started.</p></div>
            } @else {
              <table>
                <thead>
                  <tr><th>Name</th><th>Type</th><th>City/State</th><th>Phone</th><th>Personnel</th><th>Status</th><th>Actions</th></tr>
                </thead>
                <tbody>
                  @for (loc of locations(); track loc.id) {
                    <tr>
                      <td>
                        <span style="cursor:pointer;color:#3b82f6" (click)="toggleLocationPersonnel(loc)">{{ loc.locationName }}</span>
                      </td>
                      <td>
                        @if (loc.isCorporateOffice) { <span class="badge badge-active">Corporate</span> }
                        @else { <span style="color:#6b7280;font-size:12px">Branch</span> }
                        @if (loc.isMerged) { <span class="badge badge-merged" style="margin-left:4px">Acquired</span> }
                      </td>
                      <td>{{ loc.city }}{{ loc.city && loc.stateCode ? ', ' : '' }}{{ loc.stateCode }}</td>
                      <td>{{ loc.phone || '—' }}</td>
                      <td>{{ loc.personnelCount }}</td>
                      <td><span [class]="'badge ' + (loc.isActive ? 'badge-active' : 'badge-inactive')">{{ loc.isActive ? 'Active' : 'Inactive' }}</span></td>
                      <td style="display:flex;gap:4px">
                        @if (!loc.isCorporateOffice) {
                          <button class="btn btn-secondary" style="padding:4px 8px;font-size:12px" (click)="setCorporate(loc)">Set Corporate</button>
                        }
                        <button class="btn btn-danger" style="padding:4px 8px;font-size:12px" (click)="deleteLocation(loc)">Delete</button>
                      </td>
                    </tr>
                    @if (selectedLocation()?.id === loc.id) {
                      <tr>
                        <td colspan="7" style="background:#f9fafb;padding:12px">
                          <div style="font-weight:600;margin-bottom:8px">Personnel at {{ loc.locationName }}</div>
                          @if (locationPersonnel().length === 0) {
                            <div style="color:#6b7280;font-size:13px">No personnel assigned to this location.</div>
                          } @else {
                            <table style="margin-bottom:8px">
                              <thead><tr><th>Name</th><th>Assigned</th><th></th></tr></thead>
                              <tbody>
                                @for (pl of locationPersonnel(); track pl.id) {
                                  <tr>
                                    <td>{{ pl.personnelName }}</td>
                                    <td>{{ pl.assignedDate | date:'shortDate' }}</td>
                                    <td><button class="btn btn-danger" style="padding:2px 8px;font-size:12px" (click)="removePersonnelFromLocation(pl)">Remove</button></td>
                                  </tr>
                                }
                              </tbody>
                            </table>
                          }
                          <div style="display:flex;gap:8px;align-items:center;margin-top:8px">
                            <select class="form-control" style="max-width:250px" [(ngModel)]="assignPersonnelId" (ngModelChange)="assignPersonnelIdUpdate($event)">
                              <option value="">Assign personnel...</option>
                              @for (p of personnel(); track p.id) {
                                <option [value]="p.id">{{ p.firstName }} {{ p.lastName }}</option>
                              }
                            </select>
                            <button class="btn btn-primary" style="padding:6px 12px" [disabled]="!assignPersonnelId" (click)="assignPersonnel(loc)">Assign</button>
                          </div>
                        </td>
                      </tr>
                    }
                  }
                </tbody>
              </table>
            }
          </div>
        }

        @if (activeTab() === 'personnel') {
          <div class="card">
            @if (personnel().length === 0) {
              <div class="empty-state"><h3>No personnel at this agency</h3></div>
            } @else {
              <table>
                <thead><tr><th>Name</th><th>Type</th><th>Email</th><th>Status</th></tr></thead>
                <tbody>
                  @for (p of personnel(); track p.id) {
                    <tr>
                      <td><a routerLink="/personnel/{{ p.id }}">{{ p.firstName }} {{ p.lastName }}</a></td>
                      <td>{{ personnelTypeLabel(p.personnelType) }}</td>
                      <td>{{ p.email || '—' }}</td>
                      <td><span [class]="'badge ' + (p.isActive ? 'badge-active' : 'badge-inactive')">{{ p.isActive ? 'Active' : 'Inactive' }}</span></td>
                    </tr>
                  }
                </tbody>
              </table>
            }
          </div>
        }

        @if (activeTab() === 'hierarchy') {
          <div class="card">
            <div class="card-header">Agency Hierarchy</div>
            @for (a of hierarchy(); track a.id) {
              <div [style.padding-left.px]="getDepth(a) * 20" style="padding:8px 0;border-bottom:1px solid #e5e7eb">
                <a routerLink="/agencies/{{ a.id }}">{{ a.agencyName }}</a>
                <span style="margin-left:8px;font-size:11px;color:#6b7280">{{ tierLabel(a.agencyTier) }}</span>
                @if (a.id === agency()!.id) { <span class="badge badge-active" style="margin-left:8px;font-size:10px">You</span> }
              </div>
            }
          </div>
        }

        @if (activeTab() === 'lineage') {
          <div class="card">
            @if (lineage().length === 0) {
              <div class="empty-state"><h3>No lineage records</h3><p>This agency has not been involved in any mergers.</p></div>
            } @else {
              <table>
                <thead><tr><th>Merger</th><th>Entity Type</th><th>Action</th><th>From</th><th>To</th><th>Date</th></tr></thead>
                <tbody>
                  @for (l of lineage(); track l.id) {
                    <tr>
                      <td><a routerLink="/mergers/{{ l.mergerId }}">#{{ l.mergerId }}</a></td>
                      <td>{{ l.entityType }}</td>
                      <td>{{ l.action }}</td>
                      <td>{{ l.sourceAgencyName }}</td>
                      <td>{{ l.targetAgencyName }}</td>
                      <td>{{ l.recordedAt | date:'short' }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            }
          </div>
        }
      } @else {
        <div class="loading"><div class="spinner"></div></div>
      }
    </div>
  `
})
export class AgencyDetailComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  agency = signal<Agency | null>(null);
  states = signal<State[]>([]);
  personnel = signal<Personnel[]>([]);
  hierarchy = signal<AgencySummary[]>([]);
  lineage = signal<EntityLineage[]>([]);
  locations = signal<AgencyLocation[]>([]);
  locationPersonnel = signal<PersonnelLocation[]>([]);
  selectedLocation = signal<AgencyLocation | null>(null);
  activeTab = signal<Tab>('profile');
  showAddLocation = signal(false);
  savingLocation = signal(false);
  locationError = signal('');
  assignPersonnelId = '';

  locationForm = this.fb.group({
    locationName: [''],
    isCorporateOffice: [false],
    phone: [''], email: [''], website: [''],
    addressLine1: [''], addressLine2: [''], city: [''],
    stateCode: [''], zipCode: [''], county: ['']
  });

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getStates().subscribe(s => this.states.set(s));
    this.api.getAgency(id).subscribe(r => {
      this.agency.set(r.data);
      this.locations.set(r.data.locations ?? []);
    });
    this.api.getAgencyPersonnel(id).subscribe(r => this.personnel.set(r.data));
    this.api.getAgencyHierarchy(id).subscribe(r => this.hierarchy.set(r.data));
    this.api.getAgencyLineage(id).subscribe(r => this.lineage.set(r.data));
  }

  corpOffice() {
    return this.locations().find(l => l.isCorporateOffice) ?? null;
  }

  setTab(tab: Tab) { this.activeTab.set(tab); }

  saveLocation() {
    const agencyId = this.agency()!.id;
    const val = this.locationForm.value;
    this.savingLocation.set(true);
    this.locationError.set('');
    this.api.createAgencyLocation({ ...val, agencyId }).subscribe({
      next: r => {
        this.locations.update(locs => [...locs, r.data]);
        this.showAddLocation.set(false);
        this.locationForm.reset({ isCorporateOffice: false });
        this.savingLocation.set(false);
      },
      error: e => { this.locationError.set(e.error?.errors?.[0] ?? 'Failed to save location'); this.savingLocation.set(false); }
    });
  }

  setCorporate(loc: AgencyLocation) {
    this.locationError.set('');
    this.api.setAgencyLocationCorporate(loc.id).subscribe({
      next: () => this.refreshLocations(),
      error: e => this.locationError.set(e.error?.errors?.[0] ?? 'Failed to set corporate office')
    });
  }

  deleteLocation(loc: AgencyLocation) {
    this.locationError.set('');
    this.api.deleteAgencyLocation(loc.id).subscribe({
      next: () => this.locations.update(locs => locs.filter(l => l.id !== loc.id)),
      error: e => this.locationError.set(e.error?.errors?.[0] ?? 'Cannot delete location')
    });
  }

  toggleLocationPersonnel(loc: AgencyLocation) {
    if (this.selectedLocation()?.id === loc.id) {
      this.selectedLocation.set(null);
      return;
    }
    this.selectedLocation.set(loc);
    this.api.getLocationPersonnel(loc.id).subscribe(r => this.locationPersonnel.set(r.data));
  }

  assignPersonnel(loc: AgencyLocation) {
    if (!this.assignPersonnelId) return;
    this.api.assignPersonnelToLocation(loc.id, { personnelId: Number(this.assignPersonnelId) }).subscribe({
      next: r => {
        this.locationPersonnel.update(pl => [...pl, r.data]);
        this.assignPersonnelId = '';
        this.refreshLocations();
      },
      error: e => this.locationError.set(e.error?.errors?.[0] ?? 'Failed to assign personnel')
    });
  }

  assignPersonnelIdUpdate(val: string) { this.assignPersonnelId = val; }

  removePersonnelFromLocation(pl: PersonnelLocation) {
    this.api.removePersonnelFromLocation(pl.agencyLocationId, pl.personnelId).subscribe({
      next: () => {
        this.locationPersonnel.update(list => list.filter(p => p.id !== pl.id));
        this.refreshLocations();
      },
      error: e => this.locationError.set(e.error?.errors?.[0] ?? 'Failed to remove personnel')
    });
  }

  private refreshLocations() {
    this.api.getAgencyLocations(this.agency()!.id).subscribe(r => this.locations.set(r.data));
  }

  tierLabel(tier: AgencyTier) {
    return { 1: 'FMO', 2: 'MGA', 3: 'GA', 4: 'Agent' }[tier as number] ?? tier;
  }

  personnelTypeLabel(type: number) {
    return { 1: 'Producer', 2: 'CSR', 3: 'Manager', 4: 'Principal' }[type] ?? type;
  }

  getDepth(a: AgencySummary): number {
    const items = this.hierarchy();
    let depth = 0;
    let current = a;
    while (current.parentAgencyId) {
      const parent = items.find(i => i.id === current.parentAgencyId);
      if (!parent) break;
      depth++;
      current = parent;
    }
    return depth;
  }
}
