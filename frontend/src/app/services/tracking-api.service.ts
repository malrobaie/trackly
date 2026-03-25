import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { apiConfig } from '../core/api.config';
import { CreateTrackingRequest } from '../models/create-tracking-request';
import { TrackedPackageDetails } from '../models/tracked-package-details';
import { TrackedPackageSummary } from '../models/tracked-package-summary';

@Injectable({
  providedIn: 'root'
})
export class TrackingApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${apiConfig.baseUrl}/tracking`;

  createTracking(request: CreateTrackingRequest): Observable<TrackedPackageDetails> {
    return this.http.post<TrackedPackageDetails>(this.baseUrl, request);
  }

  getAllTracking(): Observable<TrackedPackageSummary[]> {
    return this.http.get<TrackedPackageSummary[]>(this.baseUrl);
  }

  getTrackingById(id: string): Observable<TrackedPackageDetails> {
    return this.http.get<TrackedPackageDetails>(`${this.baseUrl}/${id}`);
  }

  refreshTracking(id: string): Observable<TrackedPackageDetails> {
    return this.http.post<TrackedPackageDetails>(`${this.baseUrl}/${id}/refresh`, {});
  }

  deleteTracking(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
