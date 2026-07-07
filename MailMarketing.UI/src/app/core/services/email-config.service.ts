import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface EmailConfigDto {
  id: number;
  mailServer: string;
  smtpPort: number;
  useSsl: boolean;
  emailAddress: string;
  updatedAt: string;
}

export interface CreateUpdateEmailConfigDto {
  mailServer: string;
  smtpPort: number;
  useSsl: boolean;
  emailAddress: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class EmailConfigService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/emailconfigs`;

  getCurrent(): Observable<EmailConfigDto> {
    return this.http.get<EmailConfigDto>(this.apiUrl);
  }

  createOrUpdate(config: CreateUpdateEmailConfigDto): Observable<EmailConfigDto> {
    return this.http.post<EmailConfigDto>(this.apiUrl, config);
  }
}
