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
    path: 'oyun-kutuphanesi',
    loadComponent: () =>
      import('./pages/game-catalog/game-catalog.component').then(
        (m) => m.GameCatalogComponent
      )
  },
  {
    path: 'oyunlar/:slug',
    loadComponent: () =>
      import('./pages/game-detail/game-detail.component').then(
        (m) => m.GameDetailComponent
      )
  },
  {
    path: 'frp-dnd-rehberi',
    loadComponent: () =>
      import('./pages/frp-guide/frp-guide.component').then(
        (m) => m.FrpGuideComponent
      )
  },
  {
    path: 'blog',
    loadComponent: () =>
      import('./pages/blog-cluster/blog-cluster.component').then(
        (m) => m.BlogClusterComponent
      )
  },
  {
    path: 'blog/:category/:slug',
    loadComponent: () =>
      import('./pages/article-detail/article-detail.component').then(
        (m) => m.ArticleDetailComponent
      )
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./pages/admin/admin-dashboard.component').then(
        (m) => m.AdminDashboardComponent
      )
  },
  {
    path: '**',
    redirectTo: 'etkinlikler'
  }
];
