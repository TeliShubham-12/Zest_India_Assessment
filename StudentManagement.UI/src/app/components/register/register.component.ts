import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { RegisterDto } from '../../models/auth.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="auth-card">
      <h2>Create New Account</h2>
      <p class="subtitle">Register to get JWT access</p>

      <form (ngSubmit)="onSubmit()" #registerForm="ngForm">
        <div class="form-group">
          <label for="reg-username">Username</label>
          <input
            type="text"
            id="reg-username"
            name="username"
            [(ngModel)]="user.username"
            required
            placeholder="Username"
            class="form-control"
          />
        </div>

        <div class="form-group">
          <label for="reg-email">Email Address</label>
          <input
            type="email"
            id="reg-email"
            name="email"
            [(ngModel)]="user.email"
            required
            email
            placeholder="Email Address"
            class="form-control"
          />
        </div>

        <div class="form-group">
          <label for="reg-password">Password</label>
          <input
            type="password"
            id="reg-password"
            name="password"
            [(ngModel)]="user.password"
            required
            minlength="6"
            placeholder="At least 6 characters"
            class="form-control"
          />
        </div>

        <div *ngIf="errorMessage" class="alert alert-danger">
          {{ errorMessage }}
        </div>

        <button type="submit" [disabled]="!registerForm.form.valid || isLoading" class="btn btn-primary btn-block">
          {{ isLoading ? 'Registering...' : 'Register' }}
        </button>
      </form>
    </div>
  `,
  styles: [`
    .auth-card {
      background: #ffffff;
      border: 1px solid #e5e7eb;
      border-radius: 12px;
      padding: 24px;
      max-width: 420px;
      margin: 0 auto;
      box-shadow: 0 4px 12px rgba(0,0,0,0.05);
    }
    h2 { margin-top: 0; color: #111827; font-size: 1.35rem; }
    .subtitle { color: #6b7280; font-size: 0.85rem; margin-bottom: 20px; }
    .form-group { margin-bottom: 16px; }
    label { display: block; font-weight: 500; font-size: 0.85rem; margin-bottom: 6px; color: #374151; }
    .form-control { width: 100%; padding: 10px 12px; border: 1px solid #d1d5db; border-radius: 8px; font-size: 0.9rem; box-sizing: border-box; }
    .form-control:focus { outline: none; border-color: #4f46e5; }
    .btn-block { width: 100%; padding: 10px; font-weight: 600; background: #10b981; color: white; border: none; border-radius: 8px; cursor: pointer; }
    .btn-block:disabled { opacity: 0.6; cursor: not-allowed; }
    .alert-danger { background: #fee2e2; color: #dc2626; padding: 10px; border-radius: 6px; font-size: 0.85rem; margin-bottom: 16px; }
  `]
})
export class RegisterComponent {
  @Output() registered = new EventEmitter<void>();

  user: RegisterDto = {
    username: '',
    email: '',
    password: ''
  };

  isLoading = false;
  errorMessage = '';

  constructor(private authService: AuthService) {}

  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register(this.user).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) {
          this.registered.emit();
        } else {
          this.errorMessage = res.message || 'Registration failed';
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Registration failed';
      }
    });
  }
}
