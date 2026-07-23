import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { WishlistDto, AddWishlistItemDto } from '../models/wishlist.models';
import { CartDto } from '../models/cart.models';

@Injectable({
  providedIn: 'root'
})
export class WishlistService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Wishlist`;

  getWishlist(): Observable<WishlistDto> {
    return this.http.get<WishlistDto>(this.apiUrl);
  }

  addItem(dto: AddWishlistItemDto): Observable<WishlistDto> {
    return this.http.post<WishlistDto>(`${this.apiUrl}/items`, dto);
  }

  removeItem(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${productId}`);
  }

  moveToCart(productId: number): Observable<CartDto> {
    return this.http.post<CartDto>(`${this.apiUrl}/items/${productId}/move-to-cart`, {});
  }
}
