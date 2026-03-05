import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { Personnel, PaginationMeta } from '../../../core/models/api.models';

@Component({
  selector: 'app-personnel-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Personnel</h1>
        <a class="btn btn-primary" routerLink="/personnel/new">+ New Personnel</a>
      </div>
      <div class="card">
        <div class="filters">
          <input class="form-control" placeholder="Search name or email..." [(ngModel)]="search" (ngModelChange)="onSearch()" />
          <select class="form-control" [(ngModel)]="typeFilter" (ngModelChange)="load()">
            <option value="">All Types</option>
            <option value="1">Producer</option>
            <option value="2">CSR</option>
            <option value="3">Manager</option>
            <option value="4">Principal</option>
          </select>
          <select class="form-control" [(ngModel)]="activeFilter" (ngModelChange)="load()">
            <option value="">All Status</option>
            <option value="true">Active</option>
            <option value="false">Inactive</option>
          </select>
        </div>
        @if (loading()) {
          <div class="loading"><div class="spinner"></div></div>
        } @else if (items().length === 0) {
          <div class="empty-state"><h3>No personnel found</h3></div>
        } @else {
          <div class="table-wrapper">
            <table>
              <thead><tr><th>Name</th><th>Type</th><th>Agency</th><th>Email</th><th>NPN</th><th>Status</th><th></th></tr></thead>
              <tbody>
                @for (p of items(); track p.id) {
                  <tr>
                    <td><a routerLink="/personnel/{{ p.id }}">{{ p.lastName }}, {{ p.firstName }}</a></td>
                    <td>{{ typeLabel(p.personnelType) }}</td>
                    <td><a routerLink="/agencies/{{ p.agencyId }}">{{ p.agencyName }}</a></td>
                    <td>{{ p.email || '—' }}</td>
                    <td>{{ p.producer?.npn || '—' }}</td>
                    <td><span [class]="'badge ' + (p.isActive ? 'badge-active' : 'badge-inactive')">{{ p.isActive ? 'Active' : 'Inactive' }}</span></td>
                    <td><a class="btn btn-sm btn-secondary" routerLink="/personnel/{{ p.id }}/edit">Edit</a></td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
          @if (pagination()) {
            <div class="pagination">
              <span class="page-info">{{ pagination()!.totalCount }} records</span>
              <button class="btn btn-sm btn-secondary" [disabled]="page <= 1" (click)="prevPage()">‹</button>
              <button class="btn btn-sm btn-secondary" [disabled]="page >= (pagination()?.totalPages ?? 1)" (click)="nextPage()">›</button>
            </div>
          }
        }
      </div>
    </div>
  `
})
export class PersonnelListComponent implements OnInit {
  private api = inject(ApiService);
  items = signal<Personnel[]>([]); pagination = signal<PaginationMeta | null>(null);
  loading = signal(false); search = ''; typeFilter = ''; activeFilter = ''; page = 1;
  private searchTimer: ReturnType<typeof setTimeout> | null = null;

  ngOnInit() { this.load(); }
  load() {
    this.loading.set(true);
    this.api.getPersonnel({ search: this.search, type: this.typeFilter, isActive: this.activeFilter, page: this.page, pageSize: 25 }).subscribe({
      next: r => { this.items.set(r.data); this.pagination.set(r.pagination ?? null); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }
  onSearch() { if (this.searchTimer) clearTimeout(this.searchTimer); this.searchTimer = setTimeout(() => { this.page = 1; this.load(); }, 350); }
  prevPage() { if (this.page > 1) { this.page--; this.load(); } }
  nextPage() { this.page++; this.load(); }
  typeLabel(t: number) { return { 1: 'Producer', 2: 'CSR', 3: 'Manager', 4: 'Principal' }[t] ?? t; }
}
