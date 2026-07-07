import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface ResetPasswordDto {
  email: string;
  newPassword: string;
}

interface LoginResponse {
  token?: string;
  Token?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly tokenKey = 'mailMarketingToken';

  login(credentials: LoginDto): Observable<string> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      map(response => response.token ?? response.Token ?? ''),
      tap(token => {
        if (token) {
          localStorage.setItem(this.tokenKey, token);
        }
      })
    );
  }

  register(user: RegisterDto): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/register`, user, { responseType: 'text' });
  }

  checkEmail(email: string): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/check-email`, { email }, { responseType: 'text' });
  }

  resetPassword(dto: ResetPasswordDto): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/reset-password`, dto, { responseType: 'text' });
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getCurrentUserDisplayName(): string {
    const payload = this.getTokenPayload();
    const name = payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    const email = payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];

    return name ?? email ?? 'Kullanıcı';
  }

  private getTokenPayload(): Record<string, string> | null {
    const token = this.getToken();

    if (!token) {
      return null;
    }

    const [, payload] = token.split('.');

    if (!payload) {
      return null;
    }

    try {
      const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
      const decodedPayload = decodeURIComponent(
        atob(normalizedPayload)
          .split('')
          .map(character => `%${(`00${character.charCodeAt(0).toString(16)}`).slice(-2)}`)
          .join('')
      );

      return JSON.parse(decodedPayload) as Record<string, string>;
    } catch {
      return null;
    }
  }
}
