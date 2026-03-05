import { Routes } from '@angular/router';

export const mergersRoutes: Routes = [
  { path: '', loadComponent: () => import('./components/merger-list.component').then(m => m.MergerListComponent) },
  { path: 'new', loadComponent: () => import('./components/merger-wizard.component').then(m => m.MergerWizardComponent) },
  { path: ':id', loadComponent: () => import('./components/merger-detail.component').then(m => m.MergerDetailComponent) },
];
