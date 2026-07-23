import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorRetryInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('/api/auth/login') && !req.url.includes('/api/auth/refresh')) {
        return authService.refreshToken().pipe(
          switchMap((newTokenResponse) => {
            if (newTokenResponse?.accessToken) {
              const retriedReq = req.clone({
                setHeaders: { Authorization: `Bearer ${newTokenResponse.accessToken}` }
              });
              return next(retriedReq);
            }
            authService.logout();
            return throwError(() => error);
          }),
          catchError((refreshErr) => {
            authService.logout();
            return throwError(() => refreshErr);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
