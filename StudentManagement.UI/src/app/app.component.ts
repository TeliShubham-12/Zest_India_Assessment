import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './components/navbar/navbar.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { StudentListComponent } from './components/student-list/student-list.component';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, NavbarComponent, LoginComponent, RegisterComponent, StudentListComponent],
  template: `
    <app-navbar></app-navbar>

    <main class="main-container">
      <!-- Unauthenticated View: Auth Tabs (Login / Register) -->
      <div *ngIf="!isLoggedIn" class="auth-wrapper">
        <div class="tab-buttons">
          <button [class.active]="activeTab === 'login'" (click)="activeTab = 'login'">Login</button>
          <button [class.active]="activeTab === 'register'" (click)="activeTab = 'register'">Register Account</button>
        </div>

        <app-login *ngIf="activeTab === 'login'" (loggedIn)="onAuthSuccess()"></app-login>
        <app-register *ngIf="activeTab === 'register'" (registered)="onAuthSuccess()"></app-register>
      </div>

      <!-- Authenticated View: Student CRUD Directory -->
      <div *ngIf="isLoggedIn" class="student-wrapper">
        <app-student-list></app-student-list>
      </div>
    </main>
  `,
  styles: [`
    .main-container { max-width: 1100px; margin: 30px auto; padding: 0 20px; font-family: 'Inter', system-ui, sans-serif; }
    .auth-wrapper { max-width: 440px; margin: 20px auto; }
    .tab-buttons { display: flex; gap: 8px; margin-bottom: 20px; background: #e5e7eb; padding: 4px; border-radius: 10px; }
    .tab-buttons button { flex: 1; padding: 10px; border: none; background: transparent; font-weight: 500; border-radius: 8px; cursor: pointer; color: #4b5563; }
    .tab-buttons button.active { background: white; color: #111827; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
  `]
})
export class AppComponent implements OnInit {
  isLoggedIn = false;
  activeTab: 'login' | 'register' = 'login';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = !!user;
    });
  }

  onAuthSuccess(): void {
    this.isLoggedIn = true;
  }
}
