import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SubscriberDto {
  id: number;
  email: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateSubscriberDto {
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class SubscriberService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/subscribers`;

  getAll(): Observable<SubscriberDto[]> {
    return this.http.get<SubscriberDto[]>(this.apiUrl);
  }

  create(subscriber: CreateSubscriberDto): Observable<string> {
    return this.http.post<string>(this.apiUrl, subscriber, { responseType: 'text' as 'json' });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
