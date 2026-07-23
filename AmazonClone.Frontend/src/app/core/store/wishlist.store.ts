import { Injectable, signal, computed, inject, effect } from '@angular/core';
import { WishlistDto, AddWishlistItemDto } from '../models/wishlist.models';
import { WishlistService } from '../services/wishlist.service';
import { AuthStore } from './auth.store';
import { CartStore } from './cart.store';

@Injectable({
  providedIn: 'root'
})
export class WishlistStore {
  private wishlistService = inject(WishlistService);
  private authStore = inject(AuthStore);
  private cartStore = inject(CartStore);

  readonly wishlist = signal<WishlistDto | null>(null);
  readonly count = computed(() => this.wishlist()?.items?.length ?? 0);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  constructor() {
    effect(() => {
      if (this.authStore.isAuthenticated()) {
        this.loadWishlist();
      } else {
        this.wishlist.set(null);
      }
    });
  }

  loadWishlist() {
    if (!this.authStore.isAuthenticated()) return;
    this.loading.set(true);
    this.wishlistService.getWishlist().subscribe({
      next: (w) => {
        this.wishlist.set(w);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  addItem(dto: AddWishlistItemDto) {
    if (!this.authStore.isAuthenticated()) {
      this.error.set('Please sign in to add items to your Wish List.');
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    this.wishlistService.addItem(dto).subscribe({
      next: (w) => {
        this.wishlist.set(w);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Failed to add item to wishlist.');
      }
    });
  }

  removeItem(productId: number) {
    if (!this.authStore.isAuthenticated()) return;
    this.loading.set(true);
    this.wishlistService.removeItem(productId).subscribe({
      next: () => {
        const current = this.wishlist();
        if (current) {
          this.wishlist.set({
            ...current,
            items: current.items.filter(i => i.productId !== productId)
          });
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  moveToCart(productId: number) {
    if (!this.authStore.isAuthenticated()) return;
    this.loading.set(true);
    this.error.set(null);
    this.wishlistService.moveToCart(productId).subscribe({
      next: (updatedCart) => {
        this.cartStore.cart.set(updatedCart);
        const current = this.wishlist();
        if (current) {
          this.wishlist.set({
            ...current,
            items: current.items.filter(i => i.productId !== productId)
          });
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Failed to move item to cart. Insufficient stock.');
      }
    });
  }
}
