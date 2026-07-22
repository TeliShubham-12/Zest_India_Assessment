import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService } from '../../services/student.service';
import { Student, CreateStudentDto, UpdateStudentDto } from '../../models/student.model';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="student-card">
      <div class="header">
        <div>
          <h2>Student Records</h2>
          <p class="subtitle">Generic repository CRUD operations</p>
        </div>
        <button class="btn btn-success" (click)="openAddModal()">+ Add New Student</button>
      </div>

      <div class="search-bar">
        <input
          type="text"
          placeholder="Search by name, email, or course..."
          [(ngModel)]="searchQuery"
          (input)="onSearch()"
          class="form-control"
        />
      </div>

      <div *ngIf="isLoading" class="text-center p-4">
        Loading student records...
      </div>

      <div *ngIf="errorMessage" class="alert alert-danger">
        {{ errorMessage }}
      </div>

      <div *ngIf="!isLoading && filteredStudents.length === 0" class="empty-state">
        No students found.
      </div>

      <table *ngIf="!isLoading && filteredStudents.length > 0" class="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Email</th>
            <th>Age</th>
            <th>Course</th>
            <th>Created Date</th>
            <th class="text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let student of filteredStudents">
            <td>#{{ student.id }}</td>
            <td><strong>{{ student.name }}</strong></td>
            <td>{{ student.email }}</td>
            <td>{{ student.age }} yrs</td>
            <td><span class="badge">{{ student.course }}</span></td>
            <td>{{ student.createdDate | date:'mediumDate' }}</td>
            <td class="text-right">
              <button class="btn-sm btn-edit" (click)="openEditModal(student)">Edit</button>
              <button class="btn-sm btn-delete" (click)="deleteStudent(student)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Add/Edit Modal -->
    <div *ngIf="isModalOpen" class="modal-backdrop">
      <div class="modal-box">
        <div class="modal-header">
          <h3>{{ isEditMode ? 'Edit Student #' + selectedId : 'Add New Student' }}</h3>
          <button class="close-btn" (click)="closeModal()">&times;</button>
        </div>

        <form (ngSubmit)="saveStudent()" #studentForm="ngForm">
          <div class="form-group">
            <label>Full Name *</label>
            <input type="text" [(ngModel)]="formData.name" name="name" required class="form-control" placeholder="Full Name" />
          </div>

          <div class="form-group">
            <label>Email Address *</label>
            <input type="email" [(ngModel)]="formData.email" name="email" required email class="form-control" placeholder="Email Address" />
          </div>

          <div class="form-group">
            <label>Age *</label>
            <input type="number" [(ngModel)]="formData.age" name="age" required min="1" max="120" class="form-control" placeholder="Age" />
          </div>

          <div class="form-group">
            <label>Course *</label>
            <input type="text" [(ngModel)]="formData.course" name="course" required class="form-control" placeholder="Course Name" />
          </div>

          <div *ngIf="modalError" class="alert alert-danger">
            {{ modalError }}
          </div>

          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" (click)="closeModal()">Cancel</button>
            <button type="submit" [disabled]="!studentForm.form.valid || isSaving" class="btn btn-primary">
              {{ isSaving ? 'Saving...' : 'Save Student' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .student-card { background: #ffffff; border: 1px solid #e5e7eb; border-radius: 12px; padding: 24px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    h2 { margin: 0; color: #111827; font-size: 1.35rem; }
    .subtitle { color: #6b7280; font-size: 0.85rem; margin: 4px 0 0; }
    .search-bar { margin-bottom: 20px; }
    .form-control { width: 100%; padding: 10px 12px; border: 1px solid #d1d5db; border-radius: 8px; font-size: 0.9rem; box-sizing: border-box; }
    .data-table { width: 100%; border-collapse: collapse; margin-top: 10px; font-size: 0.9rem; }
    .data-table th, .data-table td { padding: 12px 14px; text-align: left; border-bottom: 1px solid #e5e7eb; }
    .data-table th { background: #f9fafb; font-weight: 600; color: #4b5563; }
    .text-right { text-align: right; }
    .badge { background: #e0e7ff; color: #4338ca; padding: 4px 8px; border-radius: 6px; font-size: 0.8rem; font-weight: 500; }
    .btn { padding: 8px 16px; font-weight: 600; border-radius: 8px; border: none; cursor: pointer; }
    .btn-success { background: #10b981; color: white; }
    .btn-primary { background: #4f46e5; color: white; }
    .btn-secondary { background: #9ca3af; color: white; margin-right: 8px; }
    .btn-sm { padding: 4px 10px; border-radius: 6px; font-size: 0.8rem; border: none; cursor: pointer; margin-left: 6px; }
    .btn-edit { background: #3b82f6; color: white; }
    .btn-delete { background: #ef4444; color: white; }
    .empty-state { text-align: center; padding: 30px; color: #6b7280; }
    .alert-danger { background: #fee2e2; color: #dc2626; padding: 10px; border-radius: 6px; font-size: 0.85rem; margin-bottom: 16px; }

    /* Modal Backdrop */
    .modal-backdrop { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; }
    .modal-box { background: white; border-radius: 12px; padding: 24px; width: 440px; max-width: 90vw; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
    .modal-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
    .close-btn { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #6b7280; }
    .modal-footer { display: flex; justify-content: flex-end; margin-top: 20px; }
    .form-group { margin-bottom: 14px; }
    .form-group label { display: block; font-weight: 500; font-size: 0.85rem; margin-bottom: 4px; color: #374151; }
  `]
})
export class StudentListComponent implements OnInit {
  students: Student[] = [];
  filteredStudents: Student[] = [];
  searchQuery = '';

  isLoading = false;
  errorMessage = '';

  // Modal State
  isModalOpen = false;
  isEditMode = false;
  selectedId: number | null = null;
  isSaving = false;
  modalError = '';

  formData: CreateStudentDto = {
    name: '',
    email: '',
    age: 20,
    course: ''
  };

  constructor(private studentService: StudentService) {}

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.studentService.getAll().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success && res.data) {
          this.students = res.data;
          this.onSearch();
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Failed to load students. Make sure you are logged in.';
      }
    });
  }

  onSearch(): void {
    const q = this.searchQuery.toLowerCase().trim();
    if (!q) {
      this.filteredStudents = [...this.students];
    } else {
      this.filteredStudents = this.students.filter(s =>
        s.name.toLowerCase().includes(q) ||
        s.email.toLowerCase().includes(q) ||
        s.course.toLowerCase().includes(q)
      );
    }
  }

  openAddModal(): void {
    this.isEditMode = false;
    this.selectedId = null;
    this.formData = { name: '', email: '', age: 21, course: '' };
    this.modalError = '';
    this.isModalOpen = true;
  }

  openEditModal(student: Student): void {
    this.isEditMode = true;
    this.selectedId = student.id;
    this.formData = {
      name: student.name,
      email: student.email,
      age: student.age,
      course: student.course
    };
    this.modalError = '';
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
  }

  saveStudent(): void {
    this.isSaving = true;
    this.modalError = '';

    if (this.isEditMode && this.selectedId) {
      this.studentService.update(this.selectedId, this.formData).subscribe({
        next: (res) => {
          this.isSaving = false;
          if (res.success) {
            this.closeModal();
            this.loadStudents();
          } else {
            this.modalError = res.message || 'Failed to update student';
          }
        },
        error: (err) => {
          this.isSaving = false;
          this.modalError = err.error?.message || 'Error updating student';
        }
      });
    } else {
      this.studentService.create(this.formData).subscribe({
        next: (res) => {
          this.isSaving = false;
          if (res.success) {
            this.closeModal();
            this.loadStudents();
          } else {
            this.modalError = res.message || 'Failed to create student';
          }
        },
        error: (err) => {
          this.isSaving = false;
          this.modalError = err.error?.message || 'Error creating student';
        }
      });
    }
  }

  deleteStudent(student: Student): void {
    if (confirm(`Are you sure you want to delete '${student.name}'?`)) {
      this.studentService.delete(student.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.loadStudents();
          }
        },
        error: (err) => {
          alert(err.error?.message || 'Failed to delete student');
        }
      });
    }
  }
}
