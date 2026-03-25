import { Carrier } from './carrier';
import { TrackingEvent } from './tracking-event';

export interface CarrierTrackingSnapshot {
  carrier: Carrier;
  trackingNumber: string;
  status: string;
  estimatedDelivery?: string | null;
  retrievedAt: string;
  events: TrackingEvent[];
}
