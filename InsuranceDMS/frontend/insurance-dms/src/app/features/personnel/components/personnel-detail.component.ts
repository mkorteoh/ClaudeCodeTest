import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { Personnel } from '../../../core/models/api.models';

@Component({
  selector: 'app-personnel-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page">
      @if (person()) {
        <div class="breadcrumb"><a routerLink="/personnel">Personnel</a> / {{ person()!.firstName }} {{ person()!.lastName }}</div>
        <div class="page-header">
          <h1>{{ person()!.firstName }} {{ person()!.lastName }}</h1>
          <a class="btn btn-secondary" routerLink="/personnel/{{ person()!.id }}/edit">Edit</a>
        </div>
        <div class="card" style="margin-bottom:16px">
          <div class="card-header">Personnel Information</div>
          <div class="form-row">
            <div><div class="form-label">Type</div><div>{{ typeLabel(person()!.personnelType) }}</div></div>
            <div><div class="form-label">Agency</div><a routerLink="/agencies/{{ person()!.agencyId }}">{{ person()!.agencyName }}</a></div>
            <div><div class="form-label">Email</div><div>{{ person()!.email || '—' }}</div></div>
            <div><div class="form-label">Phone</div><div>{{ person()!.phone || '—' }}</div></div>
            <div><div class="form-label">Title</div><div>{{ person()!.title || '—' }}</div></div>
            <div><div class="form-label">Hire Date</div><div>{{ (person()!.hireDate | date) || '—' }}</div></div>
            <div><div class="form-label">Status</div>
              <span [class]="'badge ' + (person()!.isActive ? 'badge-active' : 'badge-inactive')">{{ person()!.isActive ? 'Active' : 'Inactive' }}</span>
            </div>
          </div>
        </div>

        @if (person()!.producer) {
          <div class="card" style="margin-bottom:16px">
            <div class="card-header">Producer Information</div>
            <div class="form-row">
              <div><div class="form-label">NPN</div><div>{{ person()!.producer!.npn || '—' }}</div></div>
              <div><div class="form-label">Resident State</div><div>{{ person()!.producer!.residentState || '—' }}</div></div>
              <div><div class="form-label">E&O Expiration</div><div>{{ (person()!.producer!.eoExpirationDate | date) || '—' }}</div></div>
            </div>

            @if (person()!.producer!.licenses && person()!.producer!.licenses!.length > 0) {
              <div style="margin-top:16px">
                <div class="card-header">Licenses</div>
                <table>
                  <thead><tr><th>State</th><th>Type</th><th>License #</th><th>Expiration</th><th>Status</th></tr></thead>
                  <tbody>
                    @for (l of person()!.producer!.licenses!; track l.id) {
                      <tr>
                        <td>{{ l.stateCode }}</td>
                        <td>{{ l.licenseTypeCode }}</td>
                        <td>{{ l.licenseNumber }}</td>
                        <td>{{ (l.expirationDate | date) || '—' }}</td>
                        <td><span [class]="'badge ' + (l.isActive ? 'badge-active' : 'badge-inactive')">{{ l.isActive ? 'Active' : 'Inactive' }}</span></td>
                      </tr>
                    }
                  </tbody>
                </table>
              </div>
            }
          </div>
        }
      } @else {
        <div class="loading"><div class="spinner"></div></div>
      }
    </div>
  `
})
export class PersonnelDetailComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  person = signal<Personnel | null>(null);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getPersonnelById(id).subscribe(r => this.person.set(r.data));
  }

  typeLabel(t: number) { return { 1: 'Producer', 2: 'CSR', 3: 'Manager', 4: 'Principal' }[t] ?? t; }
}
