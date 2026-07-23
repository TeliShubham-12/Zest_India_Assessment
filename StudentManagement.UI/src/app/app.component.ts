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
      <!-- Unauthenticated View: Single Auth Card -->
      <div *ngIf="!isLoggedIn" class="auth-wrapper">
        <app-login
          *ngIf="activeTab === 'login'"
          (loggedIn)="onAuthSuccess()"
          (switchToRegister)="activeTab = 'register'"
        ></app-login>

        <app-register
          *ngIf="activeTab === 'register'"
          (registered)="onAuthSuccess()"
          (switchToLogin)="activeTab = 'login'"
        ></app-register>
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
