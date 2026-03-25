import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Carrier } from '../../models/carrier';
import { TrackingApiService } from '../../services/tracking-api.service';

@Component({
  selector: 'app-add-tracking-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './add-tracking-page.component.html',
  styleUrl: './add-tracking-page.component.css'
})
export class AddTrackingPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly trackingApi = inject(TrackingApiService);
  private readonly router = inject(Router);

  protected readonly carrierOptions = [Carrier.Usps, Carrier.Ups];
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal('');

  protected readonly form = this.formBuilder.nonNullable.group({
    carrier: [Carrier.Usps, Validators.required],
    trackingNumber: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(64)]]
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const value = this.form.getRawValue();

    this.trackingApi.createTracking({
      carrier: value.carrier,
      trackingNumber: value.trackingNumber
    }).subscribe({
      next: trackedPackage => {
        void this.router.navigate(['/tracking', trackedPackage.id]);
      },
      error: () => {
        this.errorMessage.set('Could not save the tracking number.');
        this.isSubmitting.set(false);
      }
    });
  }
}
