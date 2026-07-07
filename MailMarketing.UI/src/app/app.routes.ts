import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { MainLayoutComponent } from './layout/main-layout/main-layout';

export const routes: Routes = [
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/login/login').then(component => component.Login)
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./features/auth/register/register').then(component => component.Register)
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'subscribers',
        pathMatch: 'full'
      },
      {
        path: 'subscribers',
        loadComponent: () => import('./features/subscribers/subscribers').then(component => component.Subscribers)
      },
      {
        path: 'templates',
        loadComponent: () => import('./features/templates/templates').then(component => component.Templates)
      },
      {
        path: 'email-config',
        loadComponent: () => import('./features/email-config/email-config').then(component => component.EmailConfig)
      },
      {
        path: 'campaigns',
        loadComponent: () => import('./features/campaigns/campaigns').then(component => component.Campaigns)
      },
      {
        path: 'reports',
        loadComponent: () => import('./features/reports/reports').then(component => component.Reports)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'auth/login'
  }
];
