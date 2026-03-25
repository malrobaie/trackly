import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { TrackedPackageDetails } from '../../models/tracked-package-details';
import { TrackingApiService } from '../../services/tracking-api.service';

@Component({
  selector: 'app-tracking-details-page',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink],
  templateUrl: './tracking-details-page.component.html',
  styleUrl: './tracking-details-page.component.css'
})
export class TrackingDetailsPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly trackingApi = inject(TrackingApiService);

  protected readonly trackingId = this.route.snapshot.paramMap.get('id');
  protected readonly packageDetails = signal<TrackedPackageDetails | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isRefreshing = signal(false);
  protected readonly isDeleting = signal(false);
  protected readonly errorMessage = signal('');

  ngOnInit(): void {
    if (!this.trackingId) {
      this.errorMessage.set('Tracking id is missing.');
      this.isLoading.set(false);
      return;
    }

    this.loadPackage(this.trackingId);
  }

  protected refresh(): void {
    if (!this.trackingId) {
      return;
    }

    this.isRefreshing.set(true);
    this.errorMessage.set('');

    this.trackingApi.refreshTracking(this.trackingId).subscribe({
      next: trackedPackage => {
        this.packageDetails.set(trackedPackage);
        this.isRefreshing.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not refresh the tracked package.');
        this.isRefreshing.set(false);
      }
    });
  }

  protected deletePackage(): void {
    if (!this.trackingId) {
      return;
    }

    this.isDeleting.set(true);
    this.errorMessage.set('');

    this.trackingApi.deleteTracking(this.trackingId).subscribe({
      next: () => {
        void this.router.navigate(['/']);
      },
      error: () => {
        this.errorMessage.set('Could not delete the tracked package.');
        this.isDeleting.set(false);
      }
    });
  }

  protected statusClass(status: string): string {
    return `status-${status.toLowerCase().replaceAll(' ', '-')}`;
  }

  private loadPackage(id: string): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.trackingApi.getTrackingById(id).subscribe({
      next: trackedPackage => {
        this.packageDetails.set(trackedPackage);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Tracked package not found.');
        this.isLoading.set(false);
      }
    });
  }
}
