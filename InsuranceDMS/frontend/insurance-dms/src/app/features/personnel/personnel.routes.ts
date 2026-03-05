import { Routes } from '@angular/router';

export const personnelRoutes: Routes = [
  { path: '', loadComponent: () => import('./components/personnel-list.component').then(m => m.PersonnelListComponent) },
  { path: 'new', loadComponent: () => import('./components/personnel-form.component').then(m => m.PersonnelFormComponent) },
  { path: ':id', loadComponent: () => import('./components/personnel-detail.component').then(m => m.PersonnelDetailComponent) },
  { path: ':id/edit', loadComponent: () => import('./components/personnel-form.component').then(m => m.PersonnelFormComponent) },
];
