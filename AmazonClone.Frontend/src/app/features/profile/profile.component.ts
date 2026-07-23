import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthStore } from '../../core/store/auth.store';
import { UpdateProfileDto } from '../../core/models/auth.models';
import { LucideAngularModule, User, Mail, Save, AlertCircle, CheckCircle2, LogOut } from 'lucide-angular';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  authStore = inject(AuthStore);
  private router = inject(Router);

  UserIcon = User;
  MailIcon = Mail;
  SaveIcon = Save;
  AlertIcon = AlertCircle;
  CheckIcon = CheckCircle2;
  LogOutIcon = LogOut;

  // Form State
  firstName = signal('');
  lastName = signal('');
  email = signal('');
  
  successMessage = signal<string | null>(null);
  
  user = computed(() => this.authStore.user());
  loading = computed(() => this.authStore.loading());
  error = computed(() => this.authStore.error());

  ngOnInit() {
    const currentUser = this.user();
    if (currentUser) {
      this.firstName.set(currentUser.firstName || '');
      this.lastName.set(currentUser.lastName || '');
      this.email.set(currentUser.email || '');
    }
  }

  onSubmit() {
    if (!this.firstName().trim() || !this.lastName().trim() || !this.email().trim()) {
      return;
    }

    const dto: UpdateProfileDto = {
      firstName: this.firstName().trim(),
      lastName: this.lastName().trim(),
      email: this.email().trim()
    };

    this.successMessage.set(null);
    this.authStore.updateProfile(dto).subscribe({
      next: () => {
        this.successMessage.set('Your profile has been updated successfully.');
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: () => {
        // error handled by store
      }
    });
  }

  logout() {
    this.authStore.logout();
    this.router.navigate(['/login']);
  }
}
