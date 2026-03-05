import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiResponse, Agency, AgencySummary, Personnel, Producer, License, Appointment, Merger, MergerPreview, EntityLineage, Carrier, State, LicenseType } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/v1';

  // Agencies
  getAgencies(params: Record<string, any> = {}): Observable<ApiResponse<AgencySummary[]>> {
    return this.http.get<ApiResponse<AgencySummary[]>>(`${this.base}/agencies`, { params: this.toParams(params) });
  }
  getAgency(id: number): Observable<ApiResponse<Agency>> {
    return this.http.get<ApiResponse<Agency>>(`${this.base}/agencies/${id}`);
  }
  getAgencyHierarchy(id: number): Observable<ApiResponse<AgencySummary[]>> {
    return this.http.get<ApiResponse<AgencySummary[]>>(`${this.base}/agencies/${id}/hierarchy`);
  }
  getAgencyChildren(id: number): Observable<ApiResponse<AgencySummary[]>> {
    return this.http.get<ApiResponse<AgencySummary[]>>(`${this.base}/agencies/${id}/children`);
  }
  getAgencyPersonnel(id: number): Observable<ApiResponse<Personnel[]>> {
    return this.http.get<ApiResponse<Personnel[]>>(`${this.base}/agencies/${id}/personnel`);
  }
  getAgencyLineage(id: number): Observable<ApiResponse<EntityLineage[]>> {
    return this.http.get<ApiResponse<EntityLineage[]>>(`${this.base}/agencies/${id}/lineage`);
  }
  createAgency(body: Partial<Agency>): Observable<ApiResponse<Agency>> {
    return this.http.post<ApiResponse<Agency>>(`${this.base}/agencies`, body);
  }
  updateAgency(id: number, body: Partial<Agency>): Observable<ApiResponse<Agency>> {
    return this.http.put<ApiResponse<Agency>>(`${this.base}/agencies/${id}`, body);
  }
  deleteAgency(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/agencies/${id}`);
  }

  // Personnel
  getPersonnel(params: Record<string, any> = {}): Observable<ApiResponse<Personnel[]>> {
    return this.http.get<ApiResponse<Personnel[]>>(`${this.base}/personnel`, { params: this.toParams(params) });
  }
  getPersonnelById(id: number): Observable<ApiResponse<Personnel>> {
    return this.http.get<ApiResponse<Personnel>>(`${this.base}/personnel/${id}`);
  }
  createPersonnel(body: Partial<Personnel & Producer>): Observable<ApiResponse<Personnel>> {
    return this.http.post<ApiResponse<Personnel>>(`${this.base}/personnel`, body);
  }
  updatePersonnel(id: number, body: Partial<Personnel>): Observable<ApiResponse<Personnel>> {
    return this.http.put<ApiResponse<Personnel>>(`${this.base}/personnel/${id}`, body);
  }
  deletePersonnel(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/personnel/${id}`);
  }

  // Producers
  getProducers(params: Record<string, any> = {}): Observable<ApiResponse<Producer[]>> {
    return this.http.get<ApiResponse<Producer[]>>(`${this.base}/producers`, { params: this.toParams(params) });
  }
  getProducer(id: number): Observable<ApiResponse<Producer>> {
    return this.http.get<ApiResponse<Producer>>(`${this.base}/producers/${id}`);
  }
  getProducerLicenses(id: number): Observable<ApiResponse<License[]>> {
    return this.http.get<ApiResponse<License[]>>(`${this.base}/producers/${id}/licenses`);
  }
  getProducerAppointments(id: number): Observable<ApiResponse<Appointment[]>> {
    return this.http.get<ApiResponse<Appointment[]>>(`${this.base}/producers/${id}/appointments`);
  }
  updateProducer(id: number, body: Partial<Producer>): Observable<ApiResponse<Producer>> {
    return this.http.put<ApiResponse<Producer>>(`${this.base}/producers/${id}`, body);
  }

  // Licenses
  getLicenses(params: Record<string, any> = {}): Observable<ApiResponse<License[]>> {
    return this.http.get<ApiResponse<License[]>>(`${this.base}/licenses`, { params: this.toParams(params) });
  }
  createLicense(body: Partial<License>): Observable<ApiResponse<License>> {
    return this.http.post<ApiResponse<License>>(`${this.base}/licenses`, body);
  }
  updateLicense(id: number, body: Partial<License>): Observable<ApiResponse<License>> {
    return this.http.put<ApiResponse<License>>(`${this.base}/licenses/${id}`, body);
  }
  deleteLicense(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/licenses/${id}`);
  }

  // Appointments
  getAppointments(params: Record<string, any> = {}): Observable<ApiResponse<Appointment[]>> {
    return this.http.get<ApiResponse<Appointment[]>>(`${this.base}/appointments`, { params: this.toParams(params) });
  }
  createAppointment(body: Partial<Appointment>): Observable<ApiResponse<Appointment>> {
    return this.http.post<ApiResponse<Appointment>>(`${this.base}/appointments`, body);
  }
  terminateAppointment(id: number): Observable<ApiResponse<Appointment>> {
    return this.http.patch<ApiResponse<Appointment>>(`${this.base}/appointments/${id}/terminate`, {});
  }

  // Mergers
  getMergers(params: Record<string, any> = {}): Observable<ApiResponse<Merger[]>> {
    return this.http.get<ApiResponse<Merger[]>>(`${this.base}/mergers`, { params: this.toParams(params) });
  }
  getMerger(id: number): Observable<ApiResponse<Merger>> {
    return this.http.get<ApiResponse<Merger>>(`${this.base}/mergers/${id}`);
  }
  getMergerPreview(id: number): Observable<ApiResponse<MergerPreview>> {
    return this.http.get<ApiResponse<MergerPreview>>(`${this.base}/mergers/${id}/preview`);
  }
  getMergerLineage(id: number): Observable<ApiResponse<EntityLineage[]>> {
    return this.http.get<ApiResponse<EntityLineage[]>>(`${this.base}/mergers/${id}/lineage`);
  }
  createMerger(body: { survivingAgencyId: number; absorbedAgencyIds: number[]; notes?: string }): Observable<ApiResponse<Merger>> {
    return this.http.post<ApiResponse<Merger>>(`${this.base}/mergers`, body);
  }
  executeMerger(id: number): Observable<ApiResponse<Merger>> {
    return this.http.post<ApiResponse<Merger>>(`${this.base}/mergers/${id}/execute`, {});
  }
  cancelMerger(id: number): Observable<ApiResponse<Merger>> {
    return this.http.post<ApiResponse<Merger>>(`${this.base}/mergers/${id}/cancel`, {});
  }

  // Lookups
  getStates(): Observable<State[]> {
    return this.http.get<ApiResponse<State[]>>(`${this.base}/lookup/states`).pipe(map(r => r.data));
  }
  getLicenseTypes(): Observable<LicenseType[]> {
    return this.http.get<ApiResponse<LicenseType[]>>(`${this.base}/lookup/license-types`).pipe(map(r => r.data));
  }
  getCarriers(): Observable<Carrier[]> {
    return this.http.get<ApiResponse<Carrier[]>>(`${this.base}/carriers`).pipe(map(r => r.data));
  }

  private toParams(obj: Record<string, any>): HttpParams {
    let params = new HttpParams();
    for (const key of Object.keys(obj)) {
      if (obj[key] !== null && obj[key] !== undefined && obj[key] !== '') {
        params = params.set(key, String(obj[key]));
      }
    }
    return params;
  }
}
