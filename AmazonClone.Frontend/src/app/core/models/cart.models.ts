export interface CartItemDto
{
  cartItemId: number;
  productId: number;
  variantId?: number | null;
  productName: string;
  sku: string;
  primaryImageUrl?: string;
  color?: string;
  size?: string;
  unitPrice: number;
  totalPrice: number;
  quantity: number;
  availableStock: number;
}

export interface CartDto
{
  cartId: number;
  userId: number;
  items: CartItemDto[];
  totalItems: number;
  subtotal: number;
}

export interface AddCartItemDto
{
  productId: number;
  variantId?: number | null;
  quantity: number;
}

export interface UpdateCartItemDto
{
  quantity: number;
}

export interface GuestCartItemDto
{
  productId: number;
  variantId?: number | null;
  quantity: number;
}
