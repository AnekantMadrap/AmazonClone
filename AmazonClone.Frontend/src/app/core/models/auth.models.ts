export interface UserDto {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  role?: string;
  createdDate?: string;
}

export interface LoginDto {
  email: string;
  password?: string;
}

export interface RegisterDto {
  fullName?: string;
  firstName?: string;
  lastName?: string;
  email: string;
  password?: string;
  confirmPassword?: string;
  phoneNumber?: string;
}

export interface TokenResponseDto {
  accessToken: string;
  refreshToken?: string;
  expiresAt?: string;
  userId?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  roles?: string[];
  user?: UserDto;
}

export interface UpdateProfileDto {
  firstName: string;
  lastName: string;
  email: string;
}

export interface AddressDto {
  addressId?: number;
  userId?: number;
  fullName: string;
  phoneNumber: string;
  streetAddress: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  isDefault?: boolean;
}
