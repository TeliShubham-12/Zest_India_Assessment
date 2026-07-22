import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { AuthResponse } from '../../models/auth.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <header class="navbar">
      <div class="container">
        <div class="brand">
          🎓 <span class="title">Student Management Portal</span>
        </div>

        <div class="user-section">
          <span *ngIf="user" class="user-text">
            Logged in as: <strong>{{ user.username }}</strong> ({{ user.role }})
          </span>
          <span *ngIf="!user" class="user-text text-muted">
            Not Logged In
          </span>

          <button *ngIf="!user" class="btn btn-outline-sm" (click)="quickLogin()">Demo Admin Login</button>
          <button *ngIf="user" class="btn btn-danger-sm" (click)="logout()">Logout</button>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .navbar { background: #1f2937; color: white; padding: 14px 0; border-bottom: 1px solid #374151; }
    .container { max-width: 1100px; margin: 0 auto; padding: 0 20px; display: flex; justify-content: space-between; align-items: center; }
    .brand { font-size: 1.15rem; font-weight: 700; display: flex; align-items: center; gap: 8px; }
    .user-section { display: flex; align-items: center; gap: 14px; font-size: 0.88rem; }
    .text-muted { color: #9ca3af; }
    .btn-outline-sm { background: transparent; border: 1px solid #6366f1; color: #818cf8; padding: 5px 10px; border-radius: 6px; font-size: 0.8rem; cursor: pointer; }
    .btn-outline-sm:hover { background: #6366f1; color: white; }
    .btn-danger-sm { background: #ef4444; color: white; border: none; padding: 5px 12px; border-radius: 6px; font-size: 0.8rem; cursor: pointer; }
  `]
})
export class NavbarComponent implements OnInit {
  user: AuthResponse | null = null;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(u => this.user = u);
  }

  quickLogin(): void {
    this.authService.login({ username: 'admin', password: 'Admin@123' }).subscribe();
  }

  logout(): void {
    this.authService.logout();
  }
}
