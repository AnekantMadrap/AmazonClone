export interface ProductVariantDto {
  variantId?: number;
  productId?: number;
  sku: string;
  color?: string;
  size?: string;
  ram?: string;
  storage?: string;
  price: number;
  stockQuantity: number;
  primaryImageUrl?: string;
}

export interface ProductDto {
  productId: number;
  productName: string;
  sku: string;
  shortDescription?: string;
  price: number;
  discountPrice?: number;
  primaryImageUrl?: string;
  rating?: number;
  reviewCount?: number;
  availableStock: number;
  brandName?: string;
  categoryName?: string;
  isBestSeller?: boolean;
  stock?: number;
}

export interface ProductDetailDto extends ProductDto {
  longDescription?: string;
  brandId?: number;
  categoryId?: number;
  variants: ProductVariantDto[];
  imageGallery?: string[];
}

export interface CategoryTreeDto {
  categoryId: number;
  categoryName: string;
  parentCategoryId?: number | null;
  slug?: string;
  subCategories?: CategoryTreeDto[];
}

export interface BrandDto {
  brandId: number;
  brandName: string;
  logoUrl?: string;
}

export interface ProductSearchFilterDto {
  query?: string;
  categoryId?: number;
  brandId?: number;
  minPrice?: number;
  maxPrice?: number;
  minRating?: number;
  sortBy?: 'price_asc' | 'price_desc' | 'newest' | 'rating' | 'popularity';
  pageNumber?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
