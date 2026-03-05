import { Routes } from '@angular/router';

export const licensingRoutes: Routes = [
  { path: '', loadComponent: () => import('./components/license-list.component').then(m => m.LicenseListComponent) },
];
