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
    path: 'auth/forgot-password',
    loadComponent: () => import('./features/auth/forgot-password/forgot-password').then(component => component.ForgotPassword)
  },
  {
    path: '',
    loadComponent: () => import('./features/landing/landing').then(component => component.Landing)
  },
  {
    path: 'dashboard',
    redirectTo: 'panel/dashboard'
  },
  {
    path: 'subscribers',
    redirectTo: 'panel/subscribers'
  },
  {
    path: 'templates',
    redirectTo: 'panel/templates'
  },
  {
    path: 'email-config',
    redirectTo: 'panel/email-config'
  },
  {
    path: 'campaigns',
    redirectTo: 'panel/campaigns'
  },
  {
    path: 'reports',
    redirectTo: 'panel/reports'
  },
  {
    path: 'profile',
    redirectTo: 'panel/profile'
  },
  {
    path: 'users',
    redirectTo: 'panel/users'
  },
  {
    path: 'panel',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(component => component.Dashboard)
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
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/profile').then(component => component.Profile)
      },
      {
        path: 'users',
        loadComponent: () => import('./features/users/users').then(component => component.Users)
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
