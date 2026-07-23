import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Trash2, ShoppingBag } from 'lucide-angular';
import { CartStore } from '../../core/store/cart.store';
import { AuthStore } from '../../core/store/auth.store';
import { OrderSummaryComponent } from './order-summary/order-summary.component';
import { routes } from '../../app.routes';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, LucideAngularModule, OrderSummaryComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent {
  cartStore = inject(CartStore);
  authStore = inject(AuthStore);

  TrashIcon = Trash2;
  CartIcon = ShoppingBag;

  cart = computed(() => this.cartStore.cart());
  totalItems = computed(() => this.cartStore.totalItems());
  subTotal = computed(() => this.cartStore.subTotal());
  isAuthenticated = computed(() => this.authStore.isAuthenticated());

  updateQty(cartItemId: number, event: Event) {
    const select = event.target as HTMLSelectElement;
    const qty = Number(select.value);
    if (qty === 0) {
      this.cartStore.removeItem(cartItemId);
    } else {
      this.cartStore.updateQuantity(cartItemId, qty);
    }
  }

  removeItem(cartItemId: number) {
    this.cartStore.removeItem(cartItemId);
  }

  clearCart() {
    if (confirm('Are you sure you want to clear your entire cart?')) {
      this.cartStore.clearCart();
    }
  }
}
