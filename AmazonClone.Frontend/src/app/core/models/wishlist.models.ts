export interface WishlistItemDto {
  wishlistItemId: number;
  productId: number;
  productName: string;
  sku: string;
  price: number;
  discountPrice?: number;
  imageUrl?: string;
  addedDate?: string;
  availableStock: number;
  primaryImageUrl?: string;
}

export interface WishlistDto {
  wishlistId: number;
  userId: number;
  items: WishlistItemDto[];
}

export interface AddWishlistItemDto {
  productId: number;
}
