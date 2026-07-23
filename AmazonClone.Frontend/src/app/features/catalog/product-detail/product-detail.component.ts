import { Component, Input, OnChanges, SimpleChanges, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Star, ShoppingCart, Heart, ShieldCheck, Truck, RotateCcw } from 'lucide-angular';
import { ProductDetailDto, ProductVariantDto } from '../../../core/models/catalog.models';
import { CatalogService } from '../../../core/services/catalog.service';
import { CartStore } from '../../../core/store/cart.store';
import { WishlistStore } from '../../../core/store/wishlist.store';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnChanges
{
  @Input({ required: true }) id!: string;

  private catalogService = inject(CatalogService);
  cartStore = inject(CartStore);
  wishlistStore = inject(WishlistStore);

  StarIcon = Star;
  CartIcon = ShoppingCart;
  HeartIcon = Heart;
  ShieldIcon = ShieldCheck;
  TruckIcon = Truck;
  RotateIcon = RotateCcw;

  product = signal<ProductDetailDto | null>(null);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  selectedVariant = signal<ProductVariantDto | null>(null);
  selectedImage = signal<string>('');
  quantity = signal<number>(1);

  // Variant options
  colors = computed(() =>
  {
    const p = this.product();
    if (!p || !p.variants) return [];
    const set = new Set<string>();
    p.variants.forEach(v => { if (v.color) set.add(v.color); });
    return Array.from(set);
  });

  sizes = computed(() =>
  {
    const p = this.product();
    if (!p || !p.variants) return [];
    const set = new Set<string>();
    p.variants.forEach(v => { if (v.size) set.add(v.size); });
    return Array.from(set);
  });

  rams = computed(() =>
  {
    const p = this.product();
    if (!p || !p.variants) return [];
    const set = new Set<string>();
    p.variants.forEach(v => { if (v.ram) set.add(v.ram); });
    return Array.from(set);
  });

  // Display price & stock based on selected variant
  displayPrice = computed(() =>
  {
    const v = this.selectedVariant();
    if (v) return v.price;
    const p = this.product();
    return p?.discountPrice || p?.price || 0;
  });

  displayStock = computed(() =>
  {
    const v = this.selectedVariant();
    if (v) return v.stockQuantity;
    return this.product()?.stock ?? 0;
  });

  ngOnChanges(changes: SimpleChanges)
  {
    if (this.id)
    {
      this.loadProduct(Number(this.id));
    }
  }

  loadProduct(productId: number)
  {
    this.loading.set(true);
    this.error.set(null);
    this.catalogService.getProductById(productId).subscribe({
      next: (p) =>
      {
        this.product.set(p);
        this.selectedImage.set(p.primaryImageUrl || 'https://via.placeholder.com/500x500?text=Amazon+Product');
        if (p.variants && p.variants.length > 0)
        {
          this.selectedVariant.set(p.variants[0]);
        } else
        {
          this.selectedVariant.set(null);
        }
        this.loading.set(false);
      },
      error: (err) =>
      {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Product not found.');
      }
    });
  }

  selectColor(color: string)
  {
    const p = this.product();
    if (!p || !p.variants) return;
    const match = p.variants.find(v => v.color === color && (!this.selectedVariant()?.size || v.size === this.selectedVariant()?.size)) || p.variants.find(v => v.color === color);
    if (match)
    {
      this.selectedVariant.set(match);
      if (match.primaryImageUrl) this.selectedImage.set(match.primaryImageUrl);
    }
  }

  selectSize(size: string)
  {
    const p = this.product();
    if (!p || !p.variants) return;
    const match = p.variants.find(v => v.size === size && (!this.selectedVariant()?.color || v.color === this.selectedVariant()?.color)) || p.variants.find(v => v.size === size);
    if (match) this.selectedVariant.set(match);
  }

  selectRam(ram: string)
  {
    const p = this.product();
    if (!p || !p.variants) return;
    const match = p.variants.find(v => v.ram === ram);
    if (match) this.selectedVariant.set(match);
  }

  addToCart()
  {
    const p = this.product();
    if (!p) return;
    this.cartStore.addItem({
      productId: p.productId,
      variantId: this.selectedVariant()?.variantId || null,
      quantity: this.quantity()
    }, {
      productName: p.productName + (this.selectedVariant() ? ` (${this.selectedVariant()?.color || ''} ${this.selectedVariant()?.size || ''})`.trim() : ''),
      sku: this.selectedVariant()?.sku || p.sku,
      price: this.displayPrice(),
      primaryImageUrl: this.selectedVariant()?.primaryImageUrl || p.primaryImageUrl,
      availableStock: this.displayStock()
    });
  }

  addToWishlist()
  {
    const p = this.product();
    if (!p) return;
    this.wishlistStore.addItem({ productId: p.productId });
  }
}
