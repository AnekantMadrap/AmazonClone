import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, of, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginDto, RegisterDto, TokenResponseDto, UserDto, UpdateProfileDto, AddressDto } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api`;

  login(dto: LoginDto): Observable<TokenResponseDto> {
    return this.http.post<TokenResponseDto>(`${this.apiUrl}/Auth/login`, dto).pipe(
      tap(res => {
        if (res.accessToken) {
          localStorage.setItem('access_token', res.accessToken);
          if (res.refreshToken) {
            localStorage.setItem('refresh_token', res.refreshToken);
          }
          if (!res.user && res.userId) {
            res.user = {
              userId: res.userId,
              firstName: res.firstName || '',
              lastName: res.lastName || '',
              email: res.email || '',
              role: res.roles && res.roles.length > 0 ? res.roles[0] : undefined
            };
          }
          if (res.user) {
            localStorage.setItem('user_profile', JSON.stringify(res.user));
          }
        }
      })
    );
  }

  register(dto: RegisterDto): Observable<any> {
    const parts = (dto.fullName || '').trim().split(' ');
    const payload = {
      Email: dto.email,
      Password: dto.password,
      FirstName: dto.firstName || parts[0] || 'User',
      LastName: dto.lastName || parts.slice(1).join(' ') || 'Customer',
      PhoneNumber: dto.phoneNumber
    };
    return this.http.post(`${this.apiUrl}/auth/register`, payload);
  }

  googleLogin(idToken: string): Observable<TokenResponseDto> {
    return this.http.post<TokenResponseDto>(`${this.apiUrl}/auth/google-login`, { idToken }).pipe(
      tap(res => {
        if (res.accessToken) {
          localStorage.setItem('access_token', res.accessToken);
          if (res.refreshToken) {
            localStorage.setItem('refresh_token', res.refreshToken);
          }
          if (!res.user && res.userId) {
            res.user = {
              userId: res.userId,
              firstName: res.firstName || '',
              lastName: res.lastName || '',
              email: res.email || '',
              role: res.roles && res.roles.length > 0 ? res.roles[0] : undefined
            };
          }
          if (res.user) {
            localStorage.setItem('user_profile', JSON.stringify(res.user));
          }
        }
      })
    );
  }

  refreshToken(): Observable<TokenResponseDto> {
    const refreshToken = localStorage.getItem('refresh_token');
    const accessToken = localStorage.getItem('access_token');
    if (!refreshToken) {
      return of({ accessToken: '' });
    }
    return this.http.post<TokenResponseDto>(`${this.apiUrl}/auth/refresh`, {
      accessToken: accessToken,
      refreshToken: refreshToken
    }).pipe(
      tap(res => {
        if (res.accessToken) {
          localStorage.setItem('access_token', res.accessToken);
          if (res.refreshToken) {
            localStorage.setItem('refresh_token', res.refreshToken);
          }
          if (!res.user && res.userId) {
            res.user = {
              userId: res.userId,
              firstName: res.firstName || '',
              lastName: res.lastName || '',
              email: res.email || '',
              role: res.roles && res.roles.length > 0 ? res.roles[0] : undefined
            };
          }
          if (res.user) {
            localStorage.setItem('user_profile', JSON.stringify(res.user));
          }
        }
      })
    );
  }

  getProfile(): Observable<UserDto> {
    return this.http.get<UserDto>(`${environment.apiUrl}/api/Account/profile`);
  }

  updateProfile(dto: UpdateProfileDto): Observable<UserDto> {
    return this.http.put<{message: string, data: UserDto}>(`${environment.apiUrl}/api/Account/profile`, dto).pipe(
      map(res => res.data)
    );
  }

  logout(): void {
    const refreshToken = localStorage.getItem('refresh_token');
    if (refreshToken) {
      this.http.post(`${this.apiUrl}/auth/logout`, { refreshToken }).subscribe({ error: () => {} });
    }
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user_profile');
  }

  // Address endpoints
  getAddresses(): Observable<AddressDto[]> {
    return this.http.get<AddressDto[]>(`${this.apiUrl}/addresses`);
  }

  addAddress(dto: AddressDto): Observable<AddressDto> {
    return this.http.post<{message: string, data: AddressDto}>(`${this.apiUrl}/addresses`, dto).pipe(
      map(res => res.data)
    );
  }

  updateAddress(id: number, dto: AddressDto): Observable<AddressDto> {
    return this.http.put<{message: string, data: AddressDto}>(`${this.apiUrl}/addresses/${id}`, dto).pipe(
      map(res => res.data)
    );
  }

  deleteAddress(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/addresses/${id}`);
  }

  setDefaultAddress(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/addresses/${id}/default`, {});
  }
}
