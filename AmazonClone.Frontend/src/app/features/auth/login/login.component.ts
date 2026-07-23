import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import {
  LucideAngularModule,
  Lock,
  Mail,
  ArrowRight,
  ShieldCheck,
  AlertCircle,
} from 'lucide-angular';
import { AuthStore } from '../../../core/store/auth.store';
import { LoginDto } from '../../../core/models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  authStore = inject(AuthStore);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Icons
  LockIcon = Lock;
  MailIcon = Mail;
  ArrowRightIcon = ArrowRight;
  ShieldCheckIcon = ShieldCheck;
  AlertIcon = AlertCircle;

  // State
  email = signal<string>('');
  password = signal<string>('');
  googleIdToken = signal<string>('');
  showGoogleInput = signal<boolean>(false);
  successMessage = signal<string | null>(null);

  ngOnInit() {
    // Check if redirect from register
    const registered = this.route.snapshot.queryParamMap.get('registered');
    if (registered) {
      this.successMessage.set(
        'Account created successfully! Please sign in below.',
      );
    }
  }

  onSubmit() {
    if (!this.email() || !this.password()) return;

    const dto: LoginDto = {
      email: this.email().trim(),
      password: this.password(),
    };

    this.authStore.login(dto).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/home';
        this.router.navigateByUrl(returnUrl === '/' ? '/home' : returnUrl);
      },
      error: () => {
        this.authStore.logout();
      },
    });
  }

  onGoogleSignIn() {
    if (!this.showGoogleInput()) {
      this.showGoogleInput.set(true);
      return;
    }
    const token = this.googleIdToken().trim();
    if (!token) return;

    this.authStore.googleLogin(token).subscribe({
      next: () => {
        const returnUrl =
          this.route.snapshot.queryParamMap.get('returnUrl') || '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: () => {},
    });
  }
}
