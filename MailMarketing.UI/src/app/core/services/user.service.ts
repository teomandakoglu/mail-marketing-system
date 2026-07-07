import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserProfileDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

export interface UpdateUserProfileDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/users`;

  getProfile(): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.apiUrl}/profile`);
  }

  updateProfile(dto: UpdateUserProfileDto): Observable<string> {
    return this.http.put<string>(`${this.apiUrl}/profile`, dto, { responseType: 'text' as 'json' });
  }
}
