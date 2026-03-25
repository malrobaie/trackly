import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { TrackedPackageSummary } from '../../models/tracked-package-summary';
import { TrackingApiService } from '../../services/tracking-api.service';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css'
})
export class DashboardPageComponent implements OnInit {
  private readonly trackingApi = inject(TrackingApiService);

  protected readonly packages = signal<TrackedPackageSummary[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal('');
  protected readonly deletingId = signal<string | null>(null);

  ngOnInit(): void {
    this.loadPackages();
  }

  protected deletePackage(id: string): void {
    this.deletingId.set(id);
    this.errorMessage.set('');

    this.trackingApi.deleteTracking(id).subscribe({
      next: () => {
        this.packages.update(packages => packages.filter(packageItem => packageItem.id !== id));
        this.deletingId.set(null);
      },
      error: () => {
        this.errorMessage.set('Could not delete the tracked package.');
        this.deletingId.set(null);
      }
    });
  }

  protected trackById(_: number, packageItem: TrackedPackageSummary): string {
    return packageItem.id;
  }

  private loadPackages(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.trackingApi.getAllTracking().subscribe({
      next: packages => {
        this.packages.set(packages);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load tracked packages.');
        this.isLoading.set(false);
      }
    });
  }
}
