import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { Appointment, Carrier, State, PaginationMeta } from '../../../core/models/api.models';

@Component({
  selector: 'app-appointment-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page">
      <div class="page-header"><h1>Carrier Appointments</h1></div>
      <div class="card">
        <div class="filters">
          <select class="form-control" [(ngModel)]="carrierFilter" (ngModelChange)="load()">
            <option value="">All Carriers</option>
            @for (c of carriers(); track c.id) { <option [value]="c.id">{{ c.carrierName }}</option> }
          </select>
          <select class="form-control" [(ngModel)]="stateFilter" (ngModelChange)="load()">
            <option value="">All States</option>
            @for (s of states(); track s.stateCode) { <option [value]="s.stateCode">{{ s.stateName }}</option> }
          </select>
          <select class="form-control" [(ngModel)]="statusFilter" (ngModelChange)="load()">
            <option value="">All Statuses</option>
            <option value="Active">Active</option>
            <option value="Terminated">Terminated</option>
            <option value="Suspended">Suspended</option>
            <option value="Pending">Pending</option>
          </select>
        </div>
        @if (loading()) {
          <div class="loading"><div class="spinner"></div></div>
        } @else if (items().length === 0) {
          <div class="empty-state"><h3>No appointments found</h3></div>
        } @else {
          <div class="table-wrapper">
            <table>
              <thead><tr><th>Producer</th><th>Carrier</th><th>State</th><th>Appt Date</th><th>Term Date</th><th>Status</th><th></th></tr></thead>
              <tbody>
                @for (a of items(); track a.id) {
                  <tr>
                    <td><a routerLink="/personnel/{{ a.producerId }}">Producer #{{ a.producerId }}</a></td>
                    <td>{{ a.carrierName }}</td>
                    <td>{{ a.stateCode }}</td>
                    <td>{{ (a.appointmentDate | date) || '—' }}</td>
                    <td>{{ (a.terminationDate | date) || '—' }}</td>
                    <td><span [class]="statusBadge(a.appointmentStatus)">{{ a.appointmentStatus }}</span></td>
                    <td>
                      @if (a.appointmentStatus === 'Active') {
                        <button class="btn btn-sm btn-danger" (click)="terminate(a.id)">Terminate</button>
                      }
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
          @if (pagination()) {
            <div class="pagination">
              <span class="page-info">{{ pagination()!.totalCount }} appointments</span>
              <button class="btn btn-sm btn-secondary" [disabled]="page <= 1" (click)="prevPage()">‹</button>
              <button class="btn btn-sm btn-secondary" [disabled]="page >= (pagination()?.totalPages ?? 1)" (click)="nextPage()">›</button>
            </div>
          }
        }
      </div>
    </div>
  `
})
export class AppointmentListComponent implements OnInit {
  private api = inject(ApiService);
  items = signal<Appointment[]>([]); pagination = signal<PaginationMeta | null>(null);
  carriers = signal<Carrier[]>([]); states = signal<State[]>([]);
  loading = signal(false); carrierFilter = ''; stateFilter = ''; statusFilter = ''; page = 1;

  ngOnInit() {
    this.api.getCarriers().subscribe(c => this.carriers.set(c));
    this.api.getStates().subscribe(s => this.states.set(s));
    this.load();
  }

  load() {
    this.loading.set(true);
    this.api.getAppointments({ carrierId: this.carrierFilter, stateCode: this.stateFilter, status: this.statusFilter, page: this.page, pageSize: 25 }).subscribe({
      next: r => { this.items.set(r.data); this.pagination.set(r.pagination ?? null); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  terminate(id: number) {
    this.api.terminateAppointment(id).subscribe(() => this.load());
  }

  prevPage() { if (this.page > 1) { this.page--; this.load(); } }
  nextPage() { this.page++; this.load(); }

  statusBadge(status: string): string {
    return { 'Active': 'badge badge-active', 'Terminated': 'badge badge-inactive', 'Suspended': 'badge badge-merged', 'Pending': 'badge badge-draft' }[status] ?? 'badge';
  }
}
