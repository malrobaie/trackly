import { Carrier } from './carrier';

export interface TrackedPackageSummary {
  id: string;
  carrier: Carrier;
  trackingNumber: string;
  status: string;
  estimatedDelivery?: string | null;
  lastUpdated: string;
}
