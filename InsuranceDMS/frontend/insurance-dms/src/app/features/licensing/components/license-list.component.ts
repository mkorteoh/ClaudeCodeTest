import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { License, State, LicenseType, PaginationMeta } from '../../../core/models/api.models';

@Component({
  selector: 'app-license-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page">
      <div class="page-header"><h1>Licensing</h1></div>
      <div class="card">
        <div class="filters">
          <select class="form-control" [(ngModel)]="stateFilter" (ngModelChange)="load()">
            <option value="">All States</option>
            @for (s of states(); track s.stateCode) { <option [value]="s.stateCode">{{ s.stateName }}</option> }
          </select>
          <select class="form-control" [(ngModel)]="typeFilter" (ngModelChange)="load()">
            <option value="">All Types</option>
            @for (t of licenseTypes(); track t.licenseTypeId) { <option [value]="t.licenseTypeId">{{ t.code }} — {{ t.description }}</option> }
          </select>
          <select class="form-control" [(ngModel)]="activeFilter" (ngModelChange)="load()">
            <option value="">All Status</option>
            <option value="true">Active</option>
            <option value="false">Inactive</option>
          </select>
          <input class="form-control" type="date" [(ngModel)]="expiringBefore" (ngModelChange)="load()" placeholder="Expiring before..." />
        </div>
        @if (loading()) {
          <div class="loading"><div class="spinner"></div></div>
        } @else if (items().length === 0) {
          <div class="empty-state"><h3>No licenses found</h3></div>
        } @else {
          <div class="table-wrapper">
            <table>
              <thead><tr><th>Producer ID</th><th>State</th><th>Type</th><th>License #</th><th>Issue Date</th><th>Expiration</th><th>Status</th></tr></thead>
              <tbody>
                @for (l of items(); track l.id) {
                  <tr>
                    <td><a routerLink="/personnel/{{ l.producerId }}">{{ l.producerId }}</a></td>
                    <td>{{ l.stateCode }}</td>
                    <td>{{ l.licenseTypeCode }}</td>
                    <td>{{ l.licenseNumber }}</td>
                    <td>{{ (l.issueDate | date) || '—' }}</td>
                    <td [style.color]="isExpiringSoon(l.expirationDate) ? '#e02424' : ''">
                      {{ (l.expirationDate | date) || '—' }}
                    </td>
                    <td><span [class]="'badge ' + (l.isActive ? 'badge-active' : 'badge-inactive')">{{ l.isActive ? 'Active' : 'Inactive' }}</span></td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
          @if (pagination()) {
            <div class="pagination">
              <span class="page-info">{{ pagination()!.totalCount }} licenses</span>
              <button class="btn btn-sm btn-secondary" [disabled]="page <= 1" (click)="prevPage()">‹</button>
              <button class="btn btn-sm btn-secondary" [disabled]="page >= (pagination()?.totalPages ?? 1)" (click)="nextPage()">›</button>
            </div>
          }
        }
      </div>
    </div>
  `
})
export class LicenseListComponent implements OnInit {
  private api = inject(ApiService);
  items = signal<License[]>([]); pagination = signal<PaginationMeta | null>(null);
  states = signal<State[]>([]); licenseTypes = signal<LicenseType[]>([]);
  loading = signal(false); stateFilter = ''; typeFilter = ''; activeFilter = ''; expiringBefore = ''; page = 1;

  ngOnInit() {
    this.api.getStates().subscribe(s => this.states.set(s));
    this.api.getLicenseTypes().subscribe(t => this.licenseTypes.set(t));
    this.load();
  }

  load() {
    this.loading.set(true);
    this.api.getLicenses({ stateCode: this.stateFilter, licenseTypeId: this.typeFilter, isActive: this.activeFilter, expiringBefore: this.expiringBefore, page: this.page, pageSize: 25 }).subscribe({
      next: r => { this.items.set(r.data); this.pagination.set(r.pagination ?? null); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  prevPage() { if (this.page > 1) { this.page--; this.load(); } }
  nextPage() { this.page++; this.load(); }

  isExpiringSoon(date?: string): boolean {
    if (!date) return false;
    const d = new Date(date);
    const in90 = new Date(); in90.setDate(in90.getDate() + 90);
    return d <= in90 && d >= new Date();
  }
}
