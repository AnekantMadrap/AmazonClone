import { Component, Input, Output, EventEmitter, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Star } from 'lucide-angular';
import { CategoryTreeDto, BrandDto, ProductSearchFilterDto } from '../../../../core/models/catalog.models';
import { CatalogStore } from '../../../../core/store/catalog.store';

@Component({
  selector: 'app-filter-sidebar',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './filter-sidebar.component.html',
  styleUrl: './filter-sidebar.component.scss'
})
export class FilterSidebarComponent {
  @Input() currentFilters: ProductSearchFilterDto = {};
  @Output() filterChange = new EventEmitter<ProductSearchFilterDto>();

  catalogStore = inject(CatalogStore);
  StarIcon = Star;

  categories = computed(() => this.catalogStore.categoryTree());
  brands = computed(() => this.catalogStore.brands());
  minPriceInput = signal<number | null>(null);
  maxPriceInput = signal<number | null>(null);

  ngOnInit() {
    this.catalogStore.loadCategoryTree();
    this.catalogStore.loadBrands();
    if (this.currentFilters.minPrice !== undefined) this.minPriceInput.set(this.currentFilters.minPrice);
    if (this.currentFilters.maxPrice !== undefined) this.maxPriceInput.set(this.currentFilters.maxPrice);
  }

  selectCategory(categoryId?: number) {
    this.filterChange.emit({
      ...this.currentFilters,
      categoryId: categoryId,
      pageNumber: 1
    });
  }

  selectBrand(brandId?: number) {
    this.filterChange.emit({
      ...this.currentFilters,
      brandId: brandId,
      pageNumber: 1
    });
  }

  selectRating(minRating: number) {
    const updated = this.currentFilters.minRating === minRating ? undefined : minRating;
    this.filterChange.emit({
      ...this.currentFilters,
      minRating: updated,
      pageNumber: 1
    });
  }

  applyPriceFilter() {
    this.filterChange.emit({
      ...this.currentFilters,
      minPrice: this.minPriceInput() ?? undefined,
      maxPrice: this.maxPriceInput() ?? undefined,
      pageNumber: 1
    });
  }

  clearAllFilters() {
    this.minPriceInput.set(null);
    this.maxPriceInput.set(null);
    this.filterChange.emit({
      query: this.currentFilters.query,
      sortBy: this.currentFilters.sortBy,
      pageNumber: 1,
      pageSize: 12
    });
  }
}
