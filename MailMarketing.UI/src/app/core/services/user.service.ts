import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

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

interface UpdateProfileResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
  private readonly apiUrl = `${environment.apiUrl}/users`;

  getProfile(): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.apiUrl}/profile`);
  }

  updateProfile(dto: UpdateUserProfileDto): Observable<UpdateProfileResponse> {
    return this.http.put<UpdateProfileResponse>(`${this.apiUrl}/profile`, dto).pipe(
      tap(response => {
        if (response.token) {
          this.authService.updateToken(response.token);
        }
      })
    );
  }
}
