import { Injectable, signal, computed, inject } from '@angular/core';
import { UserDto, AddressDto, LoginDto, RegisterDto, TokenResponseDto, UpdateProfileDto } from '../models/auth.models';
import { AuthService } from '../services/auth.service';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthStore
{
  private authService = inject(AuthService);

  // Signals
  readonly user = signal<UserDto | null>(this.getInitialUser());
  readonly isAuthenticated = computed(() => !!this.user() && !!localStorage.getItem('access_token'));
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  private getInitialUser(): UserDto | null
  {
    const saved = localStorage.getItem('user_profile');
    try
    {
      return saved ? JSON.parse(saved) : null;
    } catch
    {
      return null;
    }
  }

  login(dto: LoginDto)
  {
    this.loading.set(true);
    this.error.set(null);
    return this.authService.login(dto).pipe(
      tap({
        next: (res: TokenResponseDto) =>
        {
          if (res.user)
          {
            this.user.set(res.user);
          } else
          {
            this.loadProfile();
          }
          this.loading.set(false);
        },
        error: (err) =>
        {
          this.loading.set(false);
          this.error.set(err.error?.message || 'Login failed. Please check credentials.');
        }
      })
    );
  }

  googleLogin(idToken: string)
  {
    this.loading.set(true);
    this.error.set(null);
    return this.authService.googleLogin(idToken).pipe(
      tap({
        next: (res: TokenResponseDto) =>
        {
          if (res.user)
          {
            this.user.set(res.user);
          } else
          {
            this.loadProfile();
          }
          this.loading.set(false);
        },
        error: (err) =>
        {
          this.loading.set(false);
          this.error.set(err.error?.message || 'Google Sign-in failed.');
        }
      })
    );
  }

  register(dto: RegisterDto)
  {
    this.loading.set(true);
    this.error.set(null);
    return this.authService.register(dto).pipe(
      tap({
        next: () => this.loading.set(false),
        error: (err) =>
        {
          this.loading.set(false);
          this.error.set(err.error?.message || 'Registration failed.');
        }
      })
    );
  }

  loadProfile()
  {
    if (!localStorage.getItem('access_token')) return;
    this.authService.getProfile().subscribe({
      next: (u) =>
      {
        this.user.set(u);
        localStorage.setItem('user_profile', JSON.stringify(u));
      },
      error: () => this.logout()
    });
  }

  updateProfile(dto: UpdateProfileDto)
  {
    this.loading.set(true);
    return this.authService.updateProfile(dto).pipe(
      tap({
        next: (u) =>
        {
          this.user.set(u);
          localStorage.setItem('user_profile', JSON.stringify(u));
          this.loading.set(false);
        },
        error: (err) =>
        {
          this.loading.set(false);
          this.error.set(err.error?.message || 'Profile update failed.');
        }
      })
    );
  }

  logout()
  {
    this.authService.logout();
    this.user.set(null);
  }
}
