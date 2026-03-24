import { Carrier } from './carrier';
import { TrackingEvent } from './tracking-event';

export interface TrackedPackageDetails {
  id: string;
  carrier: Carrier;
  trackingNumber: string;
  status: string;
  estimatedDelivery?: string | null;
  lastUpdated: string;
  events: TrackingEvent[];
}
