import { Routes } from '@angular/router';

import { AddTrackingPageComponent } from './features/add-tracking/add-tracking-page.component';
import { DashboardPageComponent } from './features/dashboard/dashboard-page.component';
import { TrackingDetailsPageComponent } from './features/tracking-details/tracking-details-page.component';

export const routes: Routes = [
  {
    path: '',
    component: DashboardPageComponent,
    title: 'Trackly | Dashboard'
  },
  {
    path: 'add',
    component: AddTrackingPageComponent,
    title: 'Trackly | Add Tracking'
  },
  {
    path: 'tracking/:id',
    component: TrackingDetailsPageComponent,
    title: 'Trackly | Tracking Details'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
