import { Routes } from '@angular/router';

export const agenciesRoutes: Routes = [
  { path: '', loadComponent: () => import('./components/agency-list.component').then(m => m.AgencyListComponent) },
  { path: 'new', loadComponent: () => import('./components/agency-form.component').then(m => m.AgencyFormComponent) },
  { path: ':id', loadComponent: () => import('./components/agency-detail.component').then(m => m.AgencyDetailComponent) },
  { path: ':id/edit', loadComponent: () => import('./components/agency-form.component').then(m => m.AgencyFormComponent) },
];
