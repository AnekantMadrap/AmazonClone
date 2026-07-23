import { Component, inject, computed, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AddressStore } from '../../../core/store/address.store';
import { AuthStore } from '../../../core/store/auth.store';
import { AddressDto } from '../../../core/models/auth.models';
import { LucideAngularModule, User, MapPin, Plus, Trash2, Edit2, CheckCircle2, AlertCircle, LogOut } from 'lucide-angular';
import { Router } from '@angular/router';

@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './addresses.component.html',
  styleUrl: './addresses.component.scss'
})
export class AddressesComponent implements OnInit {
  addressStore = inject(AddressStore);
  authStore = inject(AuthStore);
  private router = inject(Router);

  // Icons
  UserIcon = User;
  MapPinIcon = MapPin;
  PlusIcon = Plus;
  TrashIcon = Trash2;
  EditIcon = Edit2;
  CheckIcon = CheckCircle2;
  AlertIcon = AlertCircle;
  LogOutIcon = LogOut;

  addresses = computed(() => this.addressStore.addresses());
  loading = computed(() => this.addressStore.loading());
  error = computed(() => this.addressStore.error());
  user = computed(() => this.authStore.user());

  showForm = signal(false);
  editingAddressId = signal<number | null>(null);

  // Form State
  formData: AddressDto = this.getEmptyForm();

  ngOnInit() {
        console.log(this.addresses());

    // Already loaded by effect in store, but we can ensure it's there
  }

  getEmptyForm(): AddressDto {
    return {
      fullName: '',
      phoneNumber: '',
      streetAddress: '',
      city: '',
      state: '',
      zipCode: '',
      country: 'USA',
      isDefault: false
    };
  }

  openAddForm() {
    this.editingAddressId.set(null);
    this.formData = this.getEmptyForm();
    this.showForm.set(true);
  }

  openEditForm(address: AddressDto) {
    this.editingAddressId.set(address.addressId!);
    this.formData = { ...address };
    this.showForm.set(true);
  }

  cancelForm() {
    this.showForm.set(false);
    this.formData = this.getEmptyForm();
  }

  onSubmit() {
    if (this.editingAddressId()) {
      this.addressStore.updateAddress(this.editingAddressId()!, this.formData);
    } else {
      this.addressStore.addAddress(this.formData);
    }
    this.showForm.set(false);
  }

  deleteAddress(id: number) {
    if (confirm('Are you sure you want to remove this address?')) {
      this.addressStore.deleteAddress(id);
    }
  }

  setDefault(id: number) {
    this.addressStore.setDefaultAddress(id);
  }

  logout() {
    this.authStore.logout();
    this.router.navigate(['/login']);
  }
}
