import { Injectable, signal, computed, inject, effect } from '@angular/core';
import { AddressDto } from '../models/auth.models';
import { AuthService } from '../services/auth.service';
import { AuthStore } from './auth.store';

@Injectable({
  providedIn: 'root'
})
export class AddressStore {
  private authService = inject(AuthService);
  private authStore = inject(AuthStore);

  readonly addresses = signal<AddressDto[]>([]);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  readonly defaultAddress = computed(() => this.addresses().find(a => a.isDefault));

  constructor() {
    effect(() => {
      if (this.authStore.isAuthenticated()) {
        this.loadAddresses();
      } else {
        this.addresses.set([]);
      }
    });
  }

  loadAddresses() {
    this.loading.set(true);
    this.error.set(null);
    this.authService.getAddresses().subscribe({
      next: (res) => {
        this.addresses.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to load addresses.');
        this.loading.set(false);
      }
    });
  }

  addAddress(dto: AddressDto) {
    this.loading.set(true);
    this.error.set(null);
    this.authService.addAddress(dto).subscribe({
      next: (newAddress) => {
        this.addresses.update(list => [...list, newAddress]);
        if (newAddress.isDefault) {
          this.updateDefaultLocally(newAddress.addressId!);
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to add address.');
        this.loading.set(false);
      }
    });
  }

  updateAddress(id: number, dto: AddressDto) {
    this.loading.set(true);
    this.error.set(null);
    this.authService.updateAddress(id, dto).subscribe({
      next: (updatedAddress) => {
        this.addresses.update(list => list.map(a => a.addressId === id ? updatedAddress : a));
        if (updatedAddress.isDefault) {
          this.updateDefaultLocally(id);
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to update address.');
        this.loading.set(false);
      }
    });
  }

  deleteAddress(id: number) {
    this.loading.set(true);
    this.error.set(null);
    this.authService.deleteAddress(id).subscribe({
      next: () => {
        this.addresses.update(list => list.filter(a => a.addressId !== id));
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to delete address.');
        this.loading.set(false);
      }
    });
  }

  setDefaultAddress(id: number) {
    this.loading.set(true);
    this.error.set(null);
    this.authService.setDefaultAddress(id).subscribe({
      next: () => {
        this.updateDefaultLocally(id);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to set default address.');
        this.loading.set(false);
      }
    });
  }

  private updateDefaultLocally(id: number) {
    this.addresses.update(list => list.map(a => ({
      ...a,
      isDefault: a.addressId === id
    })));
  }
}
