import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { NavbarComponent } from './layout/navbar/navbar.component';
import { FooterComponent } from './layout/footer/footer.component';
import { AuthStore } from './core/store/auth.store';
import { CartStore } from './core/store/cart.store';
import { WishlistStore } from './core/store/wishlist.store';
import { CatalogStore } from './core/store/catalog.store';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  private authStore = inject(AuthStore);
  private router = inject(Router);

  isAuthPage = signal<boolean>(false);

  ngOnInit() {
    this.authStore.loadProfile();

    // Check initial route and track route transitions
    this.checkAuthRoute(this.router.url);
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.checkAuthRoute(event.urlAfterRedirects);
      });
  }

  private checkAuthRoute(url: string) {
    const cleanUrl = url.split('?')[0].toLowerCase();
    const isAuth =
      cleanUrl === '/login' ||
      cleanUrl === '/register' ||
      cleanUrl.startsWith('/login/') ||
      cleanUrl.startsWith('/register/');
    this.isAuthPage.set(isAuth);
  }
}
