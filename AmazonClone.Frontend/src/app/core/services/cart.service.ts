import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CartDto, AddCartItemDto, UpdateCartItemDto, GuestCartItemDto } from '../models/cart.models';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Cart`;

  getCart(): Observable<CartDto> {
    return this.http.get<CartDto>(this.apiUrl);
  }

  addItem(dto: AddCartItemDto): Observable<CartDto> {
    return this.http.post<{message: string, data: CartDto}>(`${this.apiUrl}/items`, dto).pipe(
      map(res => res.data)
    );
  }

  updateQuantity(cartItemId: number, dto: UpdateCartItemDto): Observable<CartDto> {
    return this.http.put<{message: string, data: CartDto}>(`${this.apiUrl}/items/${cartItemId}`, dto).pipe(
      map(res => res.data)
    );
  }

  removeItem(cartItemId: number): Observable<CartDto> {
    return this.http.delete<{message: string, data: CartDto}>(`${this.apiUrl}/items/${cartItemId}`).pipe(
      map(res => res.data)
    );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/clear`);
  }

  mergeGuestCart(guestItems: GuestCartItemDto[]): Observable<CartDto> {
    return this.http.post<{message: string, data: CartDto}>(`${this.apiUrl}/merge`, guestItems).pipe(
      map(res => res.data)
    );
  }
}
