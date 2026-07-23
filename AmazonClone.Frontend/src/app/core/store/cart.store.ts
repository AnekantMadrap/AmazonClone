import { Injectable, signal, computed, inject, effect } from '@angular/core';
import { CartDto, CartItemDto, AddCartItemDto, UpdateCartItemDto, GuestCartItemDto } from '../models/cart.models';
import { CartService } from '../services/cart.service';
import { AuthStore } from './auth.store';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartStore {
  private cartService = inject(CartService);
  private authStore = inject(AuthStore);

  readonly cart = signal<CartDto | null>(null);
  readonly totalItems = computed(() => this.cart()?.totalItems ?? 0);
  readonly subTotal = computed(() => this.cart()?.subtotal ?? 0);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  constructor() {
    effect(() => {
      if (this.authStore.isAuthenticated()) {
        this.syncGuestCartOnLogin();
      } else {
        this.loadGuestCartFromStorage();
      }
    });
  }

  private loadGuestCartFromStorage() {
    const saved = localStorage.getItem('amazon_guest_cart');
    if (saved) {
      try {
        const items: CartItemDto[] = JSON.parse(saved);
        const totalItems = items.reduce((sum, item) => sum + item.quantity, 0);
        const subtotal = items.reduce((sum, item) => sum + item.totalPrice, 0);
        this.cart.set({ cartId: 0, userId: 0, items, totalItems, subtotal });
      } catch {
        this.cart.set(null);
      }
    } else {
      this.cart.set(null);
    }
  }

  private saveGuestCartToStorage(items: CartItemDto[]) {
    localStorage.setItem('amazon_guest_cart', JSON.stringify(items));
    const totalItems = items.reduce((sum, item) => sum + item.quantity, 0);
    const subtotal = items.reduce((sum, item) => sum + item.totalPrice, 0);
    this.cart.set({ cartId: 0, userId: 0, items, totalItems, subtotal });
  }

  loadCart() {
    if (!this.authStore.isAuthenticated()) {
      this.loadGuestCartFromStorage();
      return;
    }
    this.loading.set(true);
    this.cartService.getCart().subscribe({
      next: (c) => {
        this.cart.set(c);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  addItem(dto: AddCartItemDto, productDetails?: { productName: string; sku: string; price: number; primaryImageUrl?: string; availableStock: number; color?: string; size?: string }) {
    this.error.set(null);
    if (!this.authStore.isAuthenticated()) {
      const currentItems = [...(this.cart()?.items || [])];
      const existingIdx = currentItems.findIndex(i => i.productId === dto.productId && i.variantId === dto.variantId);
      if (existingIdx >= 0) {
        currentItems[existingIdx].quantity += dto.quantity;
        currentItems[existingIdx].totalPrice = currentItems[existingIdx].quantity * currentItems[existingIdx].unitPrice;
      } else if (productDetails) {
        currentItems.push({
          cartItemId: Date.now(),
          productId: dto.productId,
          variantId: dto.variantId,
          productName: productDetails.productName,
          sku: productDetails.sku,
          primaryImageUrl: productDetails.primaryImageUrl,
          color: productDetails.color,
          size: productDetails.size,
          unitPrice: productDetails.price,
          quantity: dto.quantity,
          totalPrice: productDetails.price * dto.quantity,
          availableStock: productDetails.availableStock
        });
      }
      this.saveGuestCartToStorage(currentItems);
      return;
    }

    this.loading.set(true);
    this.cartService.addItem(dto).subscribe({
      next: (c) => {
        this.cart.set(c);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Failed to add item to cart.');
      }
    });
  }

  updateQuantity(cartItemId: number, quantity: number) {
    if (!this.authStore.isAuthenticated()) {
      let currentItems = [...(this.cart()?.items || [])];
      if (quantity <= 0) {
        currentItems = currentItems.filter(i => i.cartItemId !== cartItemId);
      } else {
        const idx = currentItems.findIndex(i => i.cartItemId === cartItemId);
        if (idx >= 0) {
          currentItems[idx].quantity = quantity;
          currentItems[idx].totalPrice = currentItems[idx].quantity * currentItems[idx].unitPrice;
        }
      }
      this.saveGuestCartToStorage(currentItems);
      return;
    }

    this.loading.set(true);
    this.cartService.updateQuantity(cartItemId, { quantity }).subscribe({
      next: (c) => {
        this.cart.set(c);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Failed to update quantity.');
      }
    });
  }

  removeItem(cartItemId: number) {
    if (!this.authStore.isAuthenticated()) {
      const currentItems = (this.cart()?.items || []).filter(i => i.cartItemId !== cartItemId);
      this.saveGuestCartToStorage(currentItems);
      return;
    }

    this.loading.set(true);
    this.cartService.removeItem(cartItemId).subscribe({
      next: (c) => {
        this.cart.set(c);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  clearCart() {
    if (!this.authStore.isAuthenticated()) {
      localStorage.removeItem('amazon_guest_cart');
      this.cart.set(null);
      return;
    }

    this.loading.set(true);
    this.cartService.clearCart().subscribe({
      next: () => {
        this.cart.set({ cartId: 0, userId: 0, items: [], totalItems: 0, subtotal: 0 });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  syncGuestCartOnLogin() {
    const saved = localStorage.getItem('amazon_guest_cart');
    if (saved) {
      try {
        const items: CartItemDto[] = JSON.parse(saved);
        if (items.length > 0) {
          const guestItems: GuestCartItemDto[] = items.map(i => ({
            productId: i.productId,
            variantId: i.variantId,
            quantity: i.quantity
          }));
          this.cartService.mergeGuestCart(guestItems).subscribe({
            next: (mergedCart) => {
              localStorage.removeItem('amazon_guest_cart');
              this.cart.set(mergedCart);
            },
            error: () => {
              localStorage.removeItem('amazon_guest_cart');
              this.loadCart();
            }
          });
          return;
        }
      } catch {
        localStorage.removeItem('amazon_guest_cart');
      }
    }
    this.loadCart();
  }
}
