import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { AgencySummary, AgencyTier, PaginationMeta } from '../../../core/models/api.models';

@Component({
  selector: 'app-agency-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Agencies</h1>
        <a class="btn btn-primary" routerLink="/agencies/new">+ New Agency</a>
      </div>

      <div class="card">
        <div class="filters">
          <input class="form-control" placeholder="Search name or NPN..." [(ngModel)]="search" (ngModelChange)="onSearch()" />
          <select class="form-control" [(ngModel)]="tierFilter" (ngModelChange)="load()">
            <option value="">All Tiers</option>
            <option value="1">FMO</option>
            <option value="2">MGA</option>
            <option value="3">GA</option>
            <option value="4">Agent</option>
          </select>
          <select class="form-control" [(ngModel)]="activeFilter" (ngModelChange)="load()">
            <option value="">All Status</option>
            <option value="true">Active</option>
            <option value="false">Inactive</option>
          </select>
        </div>

        @if (loading()) {
          <div class="loading"><div class="spinner"></div></div>
        } @else if (agencies().length === 0) {
          <div class="empty-state">
            <h3>No agencies found</h3>
            <p>Try adjusting your filters or create a new agency.</p>
          </div>
        } @else {
          <div class="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Agency Name</th>
                  <th>NPN</th>
                  <th>Tier</th>
                  <th>State</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                @for (agency of agencies(); track agency.id) {
                  <tr>
                    <td>
                      <a routerLink="/agencies/{{ agency.id }}">{{ agency.agencyName }}</a>
                    </td>
                    <td>{{ agency.npn || '—' }}</td>
                    <td>{{ tierLabel(agency.agencyTier) }}</td>
                    <td>{{ agency.corporateStateCode || '—' }}</td>
                    <td>
                      @if (agency.isMerged) {
                        <span class="badge badge-merged">Merged</span>
                      } @else if (agency.isActive) {
                        <span class="badge badge-active">Active</span>
                      } @else {
                        <span class="badge badge-inactive">Inactive</span>
                      }
                    </td>
                    <td>
                      <a class="btn btn-sm btn-secondary" routerLink="/agencies/{{ agency.id }}/edit">Edit</a>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          @if (pagination()) {
            <div class="pagination">
              <span class="page-info">{{ pagination()!.totalCount }} agencies, page {{ pagination()!.page }} of {{ pagination()!.totalPages }}</span>
              <button class="btn btn-sm btn-secondary" [disabled]="page <= 1" (click)="prevPage()">‹ Prev</button>
              <button class="btn btn-sm btn-secondary" [disabled]="page >= (pagination()?.totalPages ?? 1)" (click)="nextPage()">Next ›</button>
            </div>
          }
        }
      </div>
    </div>
  `
})
export class AgencyListComponent implements OnInit {
  private api = inject(ApiService);

  agencies = signal<AgencySummary[]>([]);
  pagination = signal<PaginationMeta | null>(null);
  loading = signal(false);
  search = '';
  tierFilter = '';
  activeFilter = '';
  page = 1;
  private searchTimer: ReturnType<typeof setTimeout> | null = null;

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.api.getAgencies({
      search: this.search, tier: this.tierFilter,
      isActive: this.activeFilter, page: this.page, pageSize: 25
    }).subscribe({
      next: r => { this.agencies.set(r.data); this.pagination.set(r.pagination ?? null); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  onSearch() {
    if (this.searchTimer) clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => { this.page = 1; this.load(); }, 350);
  }

  prevPage() { if (this.page > 1) { this.page--; this.load(); } }
  nextPage() { this.page++; this.load(); }

  tierLabel(tier: AgencyTier) {
    return { [AgencyTier.FMO]: 'FMO', [AgencyTier.MGA]: 'MGA', [AgencyTier.GA]: 'GA', [AgencyTier.Agent]: 'Agent' }[tier] ?? tier;
  }
}
