import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, Star, ShoppingCart, Heart } from 'lucide-angular';
import { ProductDto } from '../../../core/models/catalog.models';
import { CartStore } from '../../../core/store/cart.store';
import { WishlistStore } from '../../../core/store/wishlist.store';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterLink, LucideAngularModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  @Input({ required: true }) product!: ProductDto;

  cartStore = inject(CartStore);
  wishlistStore = inject(WishlistStore);

  StarIcon = Star;
  CartIcon = ShoppingCart;
  HeartIcon = Heart;

  addToCart(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.cartStore.addItem({
      productId: this.product.productId,
      quantity: 1
    }, {
      productName: this.product.productName,
      sku: this.product.sku,
      price: this.product.discountPrice || this.product.price,
      primaryImageUrl: this.product.primaryImageUrl,
      availableStock: this.product.availableStock
    });
  }

  addToWishlist(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.wishlistStore.addItem({ productId: this.product.productId });
  }
}
