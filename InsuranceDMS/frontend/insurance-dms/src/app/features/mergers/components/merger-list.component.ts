import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { Merger, MergerStatus } from '../../../core/models/api.models';

@Component({
  selector: 'app-merger-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Mergers & Acquisitions</h1>
        <a class="btn btn-primary" routerLink="/mergers/new">+ Initiate Merger</a>
      </div>
      <div class="card">
        <div class="filters">
          <select class="form-control" [(ngModel)]="statusFilter" (ngModelChange)="load()">
            <option value="">All Statuses</option>
            <option value="Draft">Draft</option>
            <option value="Previewed">Previewed</option>
            <option value="Approved">Approved</option>
            <option value="Executing">Executing</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
        @if (loading()) {
          <div class="loading"><div class="spinner"></div></div>
        } @else if (mergers().length === 0) {
          <div class="empty-state">
            <h3>No mergers found</h3>
            <p>Initiate a merger to combine agencies and transfer personnel.</p>
          </div>
        } @else {
          <div class="table-wrapper">
            <table>
              <thead><tr><th>ID</th><th>Surviving Agency</th><th>Absorbed Agencies</th><th>Status</th><th>Initiated</th><th>Completed</th></tr></thead>
              <tbody>
                @for (m of mergers(); track m.id) {
                  <tr>
                    <td><a routerLink="/mergers/{{ m.id }}">#{{ m.id }}</a></td>
                    <td>{{ m.survivingAgencyName }}</td>
                    <td>{{ m.participants.length }} agency(s)</td>
                    <td><span [class]="statusBadge(m.status)">{{ m.status }}</span></td>
                    <td>{{ m.initiatedAt | date:'short' }}</td>
                    <td>{{ (m.executedAt | date:'short') || '—' }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </div>
    </div>
  `
})
export class MergerListComponent implements OnInit {
  private api = inject(ApiService);
  mergers = signal<Merger[]>([]); loading = signal(false); statusFilter = '';

  ngOnInit() { this.load(); }
  load() {
    this.loading.set(true);
    this.api.getMergers({ status: this.statusFilter }).subscribe({
      next: r => { this.mergers.set(r.data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  statusBadge(status: string): string {
    const map: Record<string, string> = {
      'Draft': 'badge badge-draft', 'Previewed': 'badge badge-draft',
      'Approved': 'badge badge-draft', 'Executing': 'badge badge-executing',
      'Completed': 'badge badge-completed', 'Cancelled': 'badge badge-cancelled'
    };
    return map[status] ?? 'badge';
  }
}
