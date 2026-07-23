import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { LucideAngularModule, User, Mail, Lock, Phone, ArrowRight, ShieldAlert } from 'lucide-angular';
import { AuthStore } from '../../../core/store/auth.store';
import { RegisterDto } from '../../../core/models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  authStore = inject(AuthStore);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Icons
  UserIcon = User;
  MailIcon = Mail;
  LockIcon = Lock;
  PhoneIcon = Phone;
  ArrowRightIcon = ArrowRight;
  AlertIcon = ShieldAlert;

  // Form Signals
  fullName = signal<string>('');
  email = signal<string>('');
  phoneNumber = signal<string>('');
  password = signal<string>('');
  confirmPassword = signal<string>('');
  validationError = signal<string | null>(null);

  onSubmit() {
    this.validationError.set(null);
    if (!this.fullName() || !this.email() || !this.password() || !this.confirmPassword()) {
      this.validationError.set('Please fill out all required fields.');
      return;
    }

    if (this.password() !== this.confirmPassword()) {
      this.validationError.set('Passwords do not match. Please verify and try again.');
      return;
    }

    if (this.password().length < 6) {
      this.validationError.set('Password must be at least 6 characters long.');
      return;
    }

    const dto: RegisterDto = {
      fullName: this.fullName().trim(),
      email: this.email().trim(),
      password: this.password(),
      confirmPassword: this.confirmPassword(),
      phoneNumber: this.phoneNumber().trim() || undefined
    };

    this.authStore.register(dto).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        const queryParams: any = { registered: 'true' };
        if (returnUrl) queryParams.returnUrl = returnUrl;
        this.router.navigate(['/login'], { queryParams });
      },
      error: () => {}
    });
  }
}
