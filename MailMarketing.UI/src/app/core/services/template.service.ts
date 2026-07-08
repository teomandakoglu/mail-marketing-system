import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface TemplateDto {
  id: number;
  title: string;
  content: string;
  isActive: boolean;
  createdAt: string;
  createdByUserName: string;
}

export interface CreateTemplateDto {
  title: string;
  content: string;
}

export interface UpdateTemplateDto {
  title: string;
  content: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TemplateService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/templates`;

  getAll(): Observable<TemplateDto[]> {
    return this.http.get<TemplateDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<TemplateDto> {
    return this.http.get<TemplateDto>(`${this.apiUrl}/${id}`);
  }

  create(template: CreateTemplateDto): Observable<TemplateDto> {
    return this.http.post<TemplateDto>(this.apiUrl, template);
  }

  update(id: number, template: UpdateTemplateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, template);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
