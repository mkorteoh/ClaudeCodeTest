import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.dashboardRoutes) },
  { path: 'agencies', loadChildren: () => import('./features/agencies/agencies.routes').then(m => m.agenciesRoutes) },
  { path: 'personnel', loadChildren: () => import('./features/personnel/personnel.routes').then(m => m.personnelRoutes) },
  { path: 'licensing', loadChildren: () => import('./features/licensing/licensing.routes').then(m => m.licensingRoutes) },
  { path: 'appointments', loadChildren: () => import('./features/appointments/appointments.routes').then(m => m.appointmentsRoutes) },
  { path: 'mergers', loadChildren: () => import('./features/mergers/mergers.routes').then(m => m.mergersRoutes) },
  { path: '**', redirectTo: 'dashboard' }
];
