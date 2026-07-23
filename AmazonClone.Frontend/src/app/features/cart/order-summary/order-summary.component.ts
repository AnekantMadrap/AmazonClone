import { Component, Input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, ShieldCheck, Lock } from 'lucide-angular';

@Component({
  selector: 'app-order-summary',
  standalone: true,
  imports: [CommonModule, RouterLink, LucideAngularModule],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss'
})
export class OrderSummaryComponent {
  @Input({ required: true }) subTotal!: number;
  @Input({ required: true }) totalItems!: number;
  @Input() isAuthenticated = false;

  ShieldIcon = ShieldCheck;
  LockIcon = Lock;

  estimatedTax = computed(() => Number((this.subTotal * 0.08).toFixed(2)));
  shippingCost = computed(() => (this.subTotal > 35 || this.subTotal === 0 ? 0 : 5.99));
  orderTotal = computed(() => Number((this.subTotal + this.estimatedTax() + this.shippingCost()).toFixed(2)));
}
