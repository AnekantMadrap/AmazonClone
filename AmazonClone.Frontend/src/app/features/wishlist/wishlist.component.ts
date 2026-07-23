import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, Heart, ShoppingCart, Trash2 } from 'lucide-angular';
import { WishlistStore } from '../../core/store/wishlist.store';
import { AuthStore } from '../../core/store/auth.store';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterLink, LucideAngularModule],
  templateUrl: './wishlist.component.html',
  styleUrl: './wishlist.component.scss'
})
export class WishlistComponent {
  wishlistStore = inject(WishlistStore);
  authStore = inject(AuthStore);

  HeartIcon = Heart;
  CartIcon = ShoppingCart;
  TrashIcon = Trash2;

  wishlist = computed(() => this.wishlistStore.wishlist());
  count = computed(() => this.wishlistStore.count());
  isAuthenticated = computed(() => this.authStore.isAuthenticated());

  moveToCart(productId: number) {
    this.wishlistStore.moveToCart(productId);
  }

  removeItem(productId: number) {
    this.wishlistStore.removeItem(productId);
  }
}
