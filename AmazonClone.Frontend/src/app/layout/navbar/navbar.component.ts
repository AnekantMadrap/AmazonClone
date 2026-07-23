import { Component, inject, computed, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Search, ShoppingCart, Heart, MapPin, User, ChevronDown, Menu } from 'lucide-angular';
import { Subject, debounceTime, distinctUntilChanged, Subscription } from 'rxjs';
import { AuthStore } from '../../core/store/auth.store';
import { CartStore } from '../../core/store/cart.store';
import { WishlistStore } from '../../core/store/wishlist.store';
import { CatalogService } from '../../core/services/catalog.service';
import { AddressStore } from '../../core/store/address.store';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, LucideAngularModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit, OnDestroy
{
  authStore = inject(AuthStore);
  cartStore = inject(CartStore);
  wishlistStore = inject(WishlistStore);
  addressStore = inject(AddressStore);
  private catalogService = inject(CatalogService);
  private router = inject(Router);
  

  // Icons
  SearchIcon = Search;
  CartIcon = ShoppingCart;
  HeartIcon = Heart;
  PinIcon = MapPin;
  UserIcon = User;
  ChevronIcon = ChevronDown;
  MenuIcon = Menu;

  // Search state
  searchQuery = signal<string>('');
  selectedCategory = signal<number>(0);
  suggestions = signal<string[]>([]);
  showSuggestions = signal<boolean>(false);
  private searchInputSub = new Subject<string>();
  private sub?: Subscription;

  // Derived state
  cartItemCount = computed(() => this.cartStore.totalItems());
  wishlistCount = computed(() => this.wishlistStore.count());
  userDisplayName = computed(() =>
  {
    const u = this.authStore.user();
    if (!u) return 'Sign in';
    return u.firstName || 'User';
  });
  addresses = computed(() => 
  {
    const list = this.addressStore.addresses();
    if (list && list.length > 0) {
      const defaultAddr = list.find(a => a.isDefault) || list[0];
      return `${defaultAddr.city} ${defaultAddr.zipCode}`;
    }
    return 'Select Address';
  }); 

  ngOnInit()
  {
    this.sub = this.searchInputSub.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query =>
    {
      if (query && query.trim().length >= 2)
      {
        this.catalogService.autocomplete(query).subscribe({
          next: (list) =>
          {
            this.suggestions.set(list);
            this.showSuggestions.set(true);
          },
          error: () => this.suggestions.set([])
        });
      } else
      {
        this.suggestions.set([]);
        this.showSuggestions.set(false);
      }
    });
  }

  ngOnDestroy()
  {
    this.sub?.unsubscribe();
  }

  onSearchInput(value: string)
  {
    this.searchQuery.set(value);
    this.searchInputSub.next(value);
  }

  submitSearch()
  {
    this.showSuggestions.set(false);
    const q = this.searchQuery().trim();
    if (!q && this.selectedCategory() === 0) return;

    const queryParams: any = {};
    if (q) queryParams.q = q;
    if (this.selectedCategory() > 0) queryParams.category = this.selectedCategory();

    this.router.navigate(['/search'], { queryParams });
  }

  selectSuggestion(item: string)
  {
    this.searchQuery.set(item);
    this.showSuggestions.set(false);
    this.router.navigate(['/search'], { queryParams: { q: item } });
  }
}
