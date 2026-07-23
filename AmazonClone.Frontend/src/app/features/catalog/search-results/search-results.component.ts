import { Component, Input, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductDto, ProductSearchFilterDto, PagedResult } from '../../../core/models/catalog.models';
import { CatalogService } from '../../../core/services/catalog.service';
import { ProductCardComponent } from '../../../shared/components/product-card/product-card.component';
import { FilterSidebarComponent } from './filter-sidebar/filter-sidebar.component';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, FormsModule, ProductCardComponent, FilterSidebarComponent],
  templateUrl: './search-results.component.html',
  styleUrl: './search-results.component.scss'
})
export class SearchResultsComponent implements OnChanges {
  @Input() q?: string;
  @Input() categoryId?: number;
  @Input() category?: number;
  @Input() brandId?: number;
  @Input() minPrice?: number;
  @Input() maxPrice?: number;
  @Input() minRating?: number;
  @Input() sortBy?: 'price_asc' | 'price_desc' | 'newest' | 'rating' | 'popularity';
  @Input() pageNumber?: number;

  private catalogService = inject(CatalogService);
  private router = inject(Router);

  Math = Math;
  results = signal<PagedResult<ProductDto> | null>(null);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  currentFilters = signal<ProductSearchFilterDto>({});

  ngOnChanges(changes: SimpleChanges) {
    const filters: ProductSearchFilterDto = {
      query: this.q,
      categoryId: (this.categoryId ?? this.category) ? Number(this.categoryId ?? this.category) : undefined,
      brandId: this.brandId ? Number(this.brandId) : undefined,
      minPrice: this.minPrice ? Number(this.minPrice) : undefined,
      maxPrice: this.maxPrice ? Number(this.maxPrice) : undefined,
      minRating: this.minRating ? Number(this.minRating) : undefined,
      sortBy: this.sortBy || 'popularity',
      pageNumber: this.pageNumber ? Number(this.pageNumber) : 1,
      pageSize: 12
    };

    this.currentFilters.set(filters);
    this.executeSearch(filters);
  }

  executeSearch(filters: ProductSearchFilterDto) {
    this.loading.set(true);
    this.error.set(null);
    this.catalogService.searchProducts(filters).subscribe({
      next: (res) => {
        this.results.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Error executing catalog search.');
      }
    });
  }

  onFilterChanged(updatedFilters: ProductSearchFilterDto) {
    const queryParams: any = {};
    if (updatedFilters.query) queryParams.q = updatedFilters.query;
    if (updatedFilters.categoryId) queryParams.categoryId = updatedFilters.categoryId;
    if (updatedFilters.brandId) queryParams.brandId = updatedFilters.brandId;
    if (updatedFilters.minPrice !== undefined) queryParams.minPrice = updatedFilters.minPrice;
    if (updatedFilters.maxPrice !== undefined) queryParams.maxPrice = updatedFilters.maxPrice;
    if (updatedFilters.minRating !== undefined) queryParams.minRating = updatedFilters.minRating;
    if (updatedFilters.sortBy) queryParams.sortBy = updatedFilters.sortBy;
    if (updatedFilters.pageNumber && updatedFilters.pageNumber > 1) queryParams.pageNumber = updatedFilters.pageNumber;

    this.router.navigate(['/search'], { queryParams });
  }

  onSortChanged(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.onFilterChanged({
      ...this.currentFilters(),
      sortBy: select.value as any,
      pageNumber: 1
    });
  }

  goToPage(page: number) {
    if (page < 1 || (this.results() && page > this.results()!.totalPages)) return;
    this.onFilterChanged({
      ...this.currentFilters(),
      pageNumber: page
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
