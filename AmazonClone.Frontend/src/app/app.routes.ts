import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent),
    title: 'Sign In - Amazon Enterprise'
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent),
    title: 'Create Account - Amazon Enterprise'
  },
  {
    path: 'home',
    loadComponent: () => import('./features/catalog/home-page/home-page.component').then(m => m.HomePageComponent),
    canActivate: [authGuard],
    title: 'Amazon Enterprise - Clean Architecture & .NET 10 Electronics Store'
  },
  {
    path: 'search',
    loadComponent: () => import('./features/catalog/search-results/search-results.component').then(m => m.SearchResultsComponent),
    canActivate: [authGuard],
    title: 'Search Catalog - Amazon Enterprise'
  },
  {
    path: 'product/:id',
    loadComponent: () => import('./features/catalog/product-detail/product-detail.component').then(m => m.ProductDetailComponent),
    canActivate: [authGuard],
    title: 'Product Details - Amazon Enterprise'
  },
  {
    path: 'cart',
    loadComponent: () => import('./features/cart/cart.component').then(m => m.CartComponent),
    canActivate: [authGuard],
    title: 'Shopping Cart - Amazon Enterprise'
  },
  {
    path: 'wishlist',
    loadComponent: () => import('./features/wishlist/wishlist.component').then(m => m.WishlistComponent),
    canActivate: [authGuard],
    title: 'Customer Wish List - Amazon Enterprise'
  },
  {
    path: 'profile',
    loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [authGuard],
    title: 'Your Profile - Amazon Enterprise'
  },
  {
    path: 'addresses',
    loadComponent: () => import('./features/profile/addresses/addresses.component').then(m => m.AddressesComponent),
    canActivate: [authGuard],
    title: 'Your Addresses - Amazon Enterprise'
  },
  {
    path: '**',
    redirectTo: '',
    pathMatch: 'full'
  }
];
