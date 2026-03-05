import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { Agency, Personnel, AgencySummary, EntityLineage, AgencyTier } from '../../../core/models/api.models';

type Tab = 'profile' | 'personnel' | 'hierarchy' | 'lineage';

@Component({
  selector: 'app-agency-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
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
              <div><div class="form-label">Phone</div><div>{{ agency()!.phone || '—' }}</div></div>
              <div><div class="form-label">Email</div><div>{{ agency()!.email || '—' }}</div></div>
              <div><div class="form-label">Address</div>
                <div>{{ agency()!.addressLine1 }}<br>{{ agency()!.city }}, {{ agency()!.stateCode }} {{ agency()!.zipCode }}</div>
              </div>
              <div><div class="form-label">Notes</div><div>{{ agency()!.notes || '—' }}</div></div>
            </div>
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

  agency = signal<Agency | null>(null);
  personnel = signal<Personnel[]>([]);
  hierarchy = signal<AgencySummary[]>([]);
  lineage = signal<EntityLineage[]>([]);
  activeTab = signal<Tab>('profile');

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getAgency(id).subscribe(r => this.agency.set(r.data));
    this.api.getAgencyPersonnel(id).subscribe(r => this.personnel.set(r.data));
    this.api.getAgencyHierarchy(id).subscribe(r => this.hierarchy.set(r.data));
    this.api.getAgencyLineage(id).subscribe(r => this.lineage.set(r.data));
  }

  setTab(tab: Tab) { this.activeTab.set(tab); }

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
