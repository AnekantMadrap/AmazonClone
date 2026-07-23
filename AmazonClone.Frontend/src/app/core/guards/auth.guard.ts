import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../store/auth.store';

export const authGuard: CanActivateFn = (route, state) =>
{
  const authStore = inject(AuthStore);
  const router = inject(Router);

  if (authStore.isAuthenticated())
  {
    return true;
  }

  // Redirect to login preserving the requested destination as returnUrl
  return router.createUrlTree(['/login']);
};
