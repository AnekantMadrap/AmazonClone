import { Injectable, signal, inject } from '@angular/core';
import { CategoryTreeDto, BrandDto, ProductDto } from '../models/catalog.models';
import { CatalogService } from '../services/catalog.service';

@Injectable({
  providedIn: 'root'
})
export class CatalogStore {
  private catalogService = inject(CatalogService);

  readonly categoryTree = signal<CategoryTreeDto[]>([]);
  readonly brands = signal<BrandDto[]>([]);
  readonly bestSellers = signal<ProductDto[]>([]);
  readonly loadingCategories = signal<boolean>(false);
  readonly loadingBrands = signal<boolean>(false);
  readonly loadingBestSellers = signal<boolean>(false);

  loadCategoryTree() {
    if (this.categoryTree().length > 0) return;
    this.loadingCategories.set(true);
    this.catalogService.getCategoryTree().subscribe({
      next: (list) => {
        this.categoryTree.set(list);
        this.loadingCategories.set(false);
      },
      error: () => this.loadingCategories.set(false)
    });
  }

  loadBrands() {
    if (this.brands().length > 0) return;
    this.loadingBrands.set(true);
    this.catalogService.getBrands().subscribe({
      next: (list) => {
        this.brands.set(list);
        this.loadingBrands.set(false);
      },
      error: () => this.loadingBrands.set(false)
    });
  }

  loadBestSellers() {
    if (this.bestSellers().length > 0) return;
    this.loadingBestSellers.set(true);
    this.catalogService.getBestSellers().subscribe({
      next: (list) => {
        this.bestSellers.set(list);
        this.loadingBestSellers.set(false);
      },
      error: () => this.loadingBestSellers.set(false)
    });
  }
}
