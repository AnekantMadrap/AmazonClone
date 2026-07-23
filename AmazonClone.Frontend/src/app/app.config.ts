import { ApplicationConfig, provideZoneChangeDetection, importProvidersFrom } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { LucideAngularModule, Search, ShoppingCart, Heart, MapPin, User, ChevronDown, Menu, Star, ChevronRight, Zap, ShieldCheck, Truck, RefreshCw, RotateCcw, Trash2, ShoppingBag, Lock } from 'lucide-angular';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorRetryInterceptor } from './core/interceptors/error-retry.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([authInterceptor, errorRetryInterceptor])),
    importProvidersFrom(LucideAngularModule.pick({ Search, ShoppingCart, Heart, MapPin, User, ChevronDown, Menu, Star, ChevronRight, Zap, ShieldCheck, Truck, RefreshCw, RotateCcw, Trash2, ShoppingBag, Lock }))
  ]
};
