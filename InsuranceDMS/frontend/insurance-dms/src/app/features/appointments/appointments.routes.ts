import { Routes } from '@angular/router';

export const appointmentsRoutes: Routes = [
  { path: '', loadComponent: () => import('./components/appointment-list.component').then(m => m.AppointmentListComponent) },
];
