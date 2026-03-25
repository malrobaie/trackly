import { Carrier } from './carrier';

export interface CreateTrackingRequest {
  carrier: Carrier;
  trackingNumber: string;
}
