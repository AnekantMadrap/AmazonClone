import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CategoryTreeDto, BrandDto, ProductDto, ProductDetailDto, ProductSearchFilterDto, PagedResult } from '../models/catalog.models';

@Injectable({
  providedIn: 'root'
})
export class CatalogService
{
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getCategoryTree(): Observable<CategoryTreeDto[]>
  {
    return this.http.get<CategoryTreeDto[]>(`${this.apiUrl}/api/category`);
  }

  getBrands(): Observable<BrandDto[]>
  {
    return this.http.get<BrandDto[]>(`${this.apiUrl}/api/brands`);
  }

  getBestSellers(): Observable<ProductDto[]>
  {
    return this.http.get<ProductDto[]>(`${this.apiUrl}/api/products/bestsellers`);
  }

  getProductById(productId: number): Observable<ProductDetailDto>
  {
    return this.http.get<ProductDetailDto>(`${this.apiUrl}/api/products/${productId}`);
  }

  searchProducts(filters: ProductSearchFilterDto): Observable<PagedResult<ProductDto>>
  {
    let params = new HttpParams();
    if (filters.query) params = params.set('query', filters.query);
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId.toString());
    if (filters.brandId) params = params.set('brandId', filters.brandId.toString());
    if (filters.minPrice !== undefined) params = params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice !== undefined) params = params.set('maxPrice', filters.maxPrice.toString());
    if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
    if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber.toString());
    if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());

    return this.http.get<PagedResult<ProductDto>>(`${this.apiUrl}/api/products/search`, { params });
  }

  autocomplete(query: string): Observable<string[]>
  {
    return this.http.get<string[]>(`${this.apiUrl}/api/search/autocomplete`, {
      params: new HttpParams().set('query', query)
    });
  }
}
