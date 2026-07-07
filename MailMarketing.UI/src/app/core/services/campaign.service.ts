import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SendCampaignDto {
  templateId: number;
  subscriberIds: number[];
}

@Injectable({
  providedIn: 'root'
})
export class CampaignService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/campaigns`;

  send(campaign: SendCampaignDto): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/send`, campaign);
  }
}
