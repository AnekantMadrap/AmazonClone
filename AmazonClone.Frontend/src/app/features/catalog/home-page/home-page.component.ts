import { Component, OnInit, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, ChevronRight, Zap, ShieldCheck, Truck, RefreshCw } from 'lucide-angular';
import { CatalogStore } from '../../../core/store/catalog.store';
import { ProductCardComponent } from '../../../shared/components/product-card/product-card.component';
import { WishlistStore } from '../../../core/store/wishlist.store';
import { CartStore } from '../../../core/store/cart.store';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule, RouterLink, LucideAngularModule, ProductCardComponent],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss'
})
export class HomePageComponent implements OnInit {
  catalogStore = inject(CatalogStore);
  private cartStore = inject(CartStore);
    private wishlistStore = inject(WishlistStore);

  ChevronRightIcon = ChevronRight;
  ZapIcon = Zap;
  ShieldIcon = ShieldCheck;
  TruckIcon = Truck;
  RefreshIcon = RefreshCw;

  categories = computed(() => this.catalogStore.categoryTree());
  bestSellers = computed(() => this.catalogStore.bestSellers());

  ngOnInit() {
    this.catalogStore.loadCategoryTree();
    this.catalogStore.loadBestSellers();
    this.cartStore.loadCart();
    this.wishlistStore.loadWishlist();
    this.catalogStore.loadCategoryTree();
  }
}
