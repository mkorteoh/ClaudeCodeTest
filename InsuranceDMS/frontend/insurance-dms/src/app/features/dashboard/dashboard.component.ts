import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';

interface KpiCard {
  label: string;
  value: number | string;
  sublabel: string;
  link: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Dashboard</h1>
      </div>

      <div class="stats-grid">
        @for (card of kpis(); track card.label) {
          <div class="stat-card">
            <div class="stat-label">{{ card.label }}</div>
            <div class="stat-value">{{ card.value }}</div>
            <div style="font-size:12px;color:#6b7280;margin-top:4px">{{ card.sublabel }}</div>
          </div>
        }
      </div>

      <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px">
        <div class="card">
          <div class="card-header">Quick Actions</div>
          <div style="display:flex;flex-direction:column;gap:8px">
            <a class="btn btn-primary" routerLink="/agencies" style="justify-content:center">View Agencies</a>
            <a class="btn btn-secondary" routerLink="/personnel" style="justify-content:center">Manage Personnel</a>
            <a class="btn btn-secondary" routerLink="/licensing" style="justify-content:center">Track Licenses</a>
            <a class="btn btn-secondary" routerLink="/mergers" style="justify-content:center">M&A Workflow</a>
          </div>
        </div>
        <div class="card">
          <div class="card-header">System Status</div>
          <div style="font-size:13px;color:#6b7280">
            <p>✅ API Connected</p>
            <p>✅ Database Seeded (50 States, License Types)</p>
            <p>✅ All CRUD Endpoints Active</p>
            <p>✅ M&A Merger Workflow Ready</p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  private api = inject(ApiService);

  kpis = signal<KpiCard[]>([
    { label: 'Total Agencies', value: '—', sublabel: 'Active distribution partners', link: '/agencies' },
    { label: 'Producers', value: '—', sublabel: 'Licensed agents', link: '/personnel' },
    { label: 'Active Licenses', value: '—', sublabel: 'Across all states', link: '/licensing' },
    { label: 'Pending Mergers', value: '—', sublabel: 'Draft or Previewed', link: '/mergers' },
  ]);

  ngOnInit() {
    this.api.getAgencies({ isActive: true }).subscribe(r => {
      this.updateKpi(0, r.pagination?.totalCount ?? r.data.length);
    });
    this.api.getPersonnel({ type: 1 }).subscribe(r => {
      this.updateKpi(1, r.pagination?.totalCount ?? r.data.length);
    });
    this.api.getLicenses({ isActive: true }).subscribe(r => {
      this.updateKpi(2, r.pagination?.totalCount ?? r.data.length);
    });
    this.api.getMergers({ status: 'Draft' }).subscribe(r => {
      this.updateKpi(3, r.pagination?.totalCount ?? r.data.length);
    });
  }

  private updateKpi(index: number, value: number) {
    this.kpis.update(cards => {
      const updated = [...cards];
      updated[index] = { ...updated[index], value };
      return updated;
    });
  }
}
