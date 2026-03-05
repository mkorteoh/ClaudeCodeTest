import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { Merger, EntityLineage } from '../../../core/models/api.models';

@Component({
  selector: 'app-merger-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page">
      @if (merger()) {
        <div class="breadcrumb"><a routerLink="/mergers">Mergers</a> / Merger #{{ merger()!.id }}</div>
        <div class="page-header">
          <h1>Merger #{{ merger()!.id }}</h1>
          <span [class]="statusBadge(merger()!.status)" style="font-size:14px;padding:6px 14px">{{ merger()!.status }}</span>
        </div>

        <div class="card" style="margin-bottom:16px">
          <div class="card-header">Summary</div>
          <div class="form-row">
            <div><div class="form-label">Surviving Agency</div><a routerLink="/agencies/{{ merger()!.survivingAgencyId }}">{{ merger()!.survivingAgencyName }}</a></div>
            <div><div class="form-label">Initiated By</div><div>{{ merger()!.initiatedBy || '—' }}</div></div>
            <div><div class="form-label">Initiated At</div><div>{{ merger()!.initiatedAt | date:'medium' }}</div></div>
            <div><div class="form-label">Executed At</div><div>{{ (merger()!.executedAt | date:'medium') || '—' }}</div></div>
            <div><div class="form-label">Notes</div><div>{{ merger()!.notes || '—' }}</div></div>
          </div>
        </div>

        <div class="card" style="margin-bottom:16px">
          <div class="card-header">Absorbed Agencies</div>
          <table>
            <thead><tr><th>Agency</th><th>Personnel Transferred</th></tr></thead>
            <tbody>
              @for (p of merger()!.participants; track p.id) {
                <tr>
                  <td><a routerLink="/agencies/{{ p.absorbedAgencyId }}">{{ p.absorbedAgencyName }}</a></td>
                  <td>{{ p.personnelTransferred }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>

        @if (lineage().length > 0) {
          <div class="card">
            <div class="card-header">Entity Lineage ({{ lineage().length }} records)</div>
            <div class="table-wrapper">
              <table>
                <thead><tr><th>Entity Type</th><th>Source ID</th><th>From Agency</th><th>To Agency</th><th>Action</th><th>Recorded</th></tr></thead>
                <tbody>
                  @for (l of lineage(); track l.id) {
                    <tr>
                      <td>{{ l.entityType }}</td>
                      <td>{{ l.sourceEntityId }}</td>
                      <td>{{ l.sourceAgencyName }}</td>
                      <td>{{ l.targetAgencyName }}</td>
                      <td>{{ l.action }}</td>
                      <td>{{ l.recordedAt | date:'short' }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        }
      } @else {
        <div class="loading"><div class="spinner"></div></div>
      }
    </div>
  `
})
export class MergerDetailComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  merger = signal<Merger | null>(null);
  lineage = signal<EntityLineage[]>([]);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getMerger(id).subscribe(r => this.merger.set(r.data));
    this.api.getMergerLineage(id).subscribe(r => this.lineage.set(r.data));
  }

  statusBadge(status: string): string {
    const map: Record<string, string> = {
      'Draft': 'badge badge-draft', 'Previewed': 'badge badge-draft',
      'Completed': 'badge badge-completed', 'Cancelled': 'badge badge-cancelled',
      'Executing': 'badge badge-executing'
    };
    return map[status] ?? 'badge';
  }
}
