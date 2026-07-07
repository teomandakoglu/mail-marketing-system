import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, tap } from 'rxjs';

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

interface LoginResponse {
  token?: string;
  Token?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5281/api/auth';
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

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}
