import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface MailLogDto {
  id: number;
  templateId: number;
  templateTitle: string;
  subscriberId: number;
  subscriberEmail: string;
  status: string;
  errorMessage?: string | null;
  sentAt: string;
}

export interface ReportFilterDto {
  templateId?: number | null;
  startDate?: string | null;
  endDate?: string | null;
  status?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/reports`;

  getFilteredLogs(filter: ReportFilterDto = {}): Observable<MailLogDto[]> {
    let params = new HttpParams();

    Object.entries(filter).forEach(([key, value]) => {
      if (key === 'status') {
        if (value === 'Success' || value === 'Failed') {
          params = params.set(key, value);
        }

        return;
      }

      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    });

    return this.http.get<MailLogDto[]>(this.apiUrl, { params });
  }
}
