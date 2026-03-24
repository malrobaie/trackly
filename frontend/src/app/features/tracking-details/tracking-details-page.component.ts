import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-tracking-details-page',
  standalone: true,
  imports: [],
  templateUrl: './tracking-details-page.component.html',
  styleUrl: './tracking-details-page.component.css'
})
export class TrackingDetailsPageComponent {
  protected readonly trackingId: string | null;

  constructor(private readonly route: ActivatedRoute) {
    this.trackingId = this.route.snapshot.paramMap.get('id');
  }
}
