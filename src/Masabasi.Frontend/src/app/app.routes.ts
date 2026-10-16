import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'etkinlikler'
  },
  {
    path: 'etkinlikler',
    loadComponent: () =>
      import('./pages/events-page/events-page.component').then(
        (m) => m.EventsPageComponent
      )
  },
  {
    path: '**',
    redirectTo: 'etkinlikler'
  }
];
