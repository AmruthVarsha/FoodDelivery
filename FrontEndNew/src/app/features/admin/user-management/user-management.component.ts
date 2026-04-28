import { AdminSidebarComponent } from '../../../shared/components/admin-sidebar/admin-sidebar.component';
import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../core/services/admin.service';
import { Subject, EMPTY } from 'rxjs';
import { takeUntil, finalize, catchError, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

export interface User {
  userId: string;
  fullName: string;
  email: string;
  phoneNo: string;
  role: string;
  isActive: boolean;
  emailConfirmed: boolean;
  twoFactorEnabled: boolean;
  createdAt: Date;
}

export interface EditUserForm {
  fullName: string;
  phoneNo: string;
  isActive: boolean;
}


@Component({
  selector: 'app-admin-user-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AdminSidebarComponent],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class AdminUserManagementComponent implements OnInit, OnDestroy {

  searchQuery = '';
  selectedRole = 'all';
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  users: User[] = [];

  // Edit modal state
  showEditModal = false;
  editingUser: User | null = null;
  editForm: EditUserForm = { fullName: '', phoneNo: '', isActive: true };
  isSaving = false;
  editError = '';

  // Delete state
  deletingUserId: string | null = null;

  // Status toggle state
  togglingUserId: string | null = null;

  roleFilters = ['all', 'Customer', 'Partner', 'DeliveryAgent', 'Admin'];

  private destroy$ = new Subject<void>();

  constructor(
    private router: Router,
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.markForCheck();

    this.adminService.getAllUsers().pipe(
      takeUntil(this.destroy$),
      catchError(err => {
        console.error('Failed to load users', err);
        this.errorMessage = 'Failed to load users.';
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.isLoading = false;
        this.cdr.markForCheck();
      })
    ).subscribe(data => {
      if (data) {
        this.users = data;
        this.cdr.markForCheck();
      }
    });
  }

  get filteredUsers(): User[] {
    let filtered = this.users;

    if (this.selectedRole !== 'all') {
      filtered = filtered.filter(u => u.role === this.selectedRole);
    }

    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      filtered = filtered.filter(u =>
        (u.fullName && u.fullName.toLowerCase().includes(query)) ||
        (u.email && u.email.toLowerCase().includes(query))
      );
    }

    return filtered;
  }

  // ── STATUS TOGGLE ──────────────────────────────────────────────────────────
  toggleUserStatus(user: User): void {
    if (this.togglingUserId === user.userId) return; // prevent double-click

    const newStatus = !user.isActive;
    this.togglingUserId = user.userId;
    this.clearMessages();

    this.adminService.updateUser(user.userId, {
      fullName: user.fullName,
      phoneNo: user.phoneNo,
      isActive: newStatus
    }).pipe(
      takeUntil(this.destroy$),
      catchError(err => {
        console.error('Failed to update status', err);
        this.errorMessage = `Failed to update status for ${user.fullName}.`;
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.togglingUserId = null;
        this.cdr.markForCheck();
      })
    ).subscribe(() => {
      user.isActive = newStatus;
      this.successMessage = `${user.fullName} is now ${newStatus ? 'Active' : 'Inactive'}.`;
      this.cdr.markForCheck();
      setTimeout(() => { this.successMessage = ''; this.cdr.markForCheck(); }, 3000);
    });
  }

  // ── EDIT MODAL ─────────────────────────────────────────────────────────────
  editUser(user: User): void {
    this.editingUser = user;
    this.editForm = {
      fullName: user.fullName,
      phoneNo: user.phoneNo,
      isActive: user.isActive
    };
    this.editError = '';
    this.showEditModal = true;
    this.cdr.markForCheck();
  }

  cancelEdit(): void {
    this.showEditModal = false;
    this.editingUser = null;
    this.editError = '';
    this.cdr.markForCheck();
  }

  saveUser(): void {
    if (!this.editingUser) return;
    if (!this.editForm.fullName.trim() || !this.editForm.phoneNo.trim()) {
      this.editError = 'Full name and phone number are required.';
      return;
    }

    this.isSaving = true;
    this.editError = '';

    this.adminService.updateUser(this.editingUser.userId, this.editForm).pipe(
      takeUntil(this.destroy$),
      catchError(err => {
        console.error('Failed to save user', err);
        this.editError = err?.error?.message || 'Failed to save changes. Please try again.';
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.isSaving = false;
        this.cdr.markForCheck();
      })
    ).subscribe(() => {
      // Update local record
      const idx = this.users.findIndex(u => u.userId === this.editingUser!.userId);
      if (idx !== -1) {
        this.users[idx] = {
          ...this.users[idx],
          fullName: this.editForm.fullName,
          phoneNo: this.editForm.phoneNo,
          isActive: this.editForm.isActive
        };
      }
      this.showEditModal = false;
      this.editingUser = null;
      this.successMessage = 'User updated successfully.';
      this.cdr.markForCheck();
      setTimeout(() => { this.successMessage = ''; this.cdr.markForCheck(); }, 3000);
    });
  }

  // ── DELETE ─────────────────────────────────────────────────────────────────
  deleteUser(userId: string): void {
    const user = this.users.find(u => u.userId === userId);
    if (!user) return;

    // Use a native confirm (browser's built-in dialog) – works, just not pretty
    if (!confirm(`Delete "${user.fullName}"?\nThis action cannot be undone.`)) return;

    this.deletingUserId = userId;
    this.clearMessages();
    this.cdr.markForCheck();

    this.adminService.deleteUser(userId).pipe(
      takeUntil(this.destroy$),
      catchError(err => {
        console.error('Failed to delete user', err);
        this.errorMessage = `Failed to delete ${user.fullName}.`;
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.deletingUserId = null;
        this.cdr.markForCheck();
      })
    ).subscribe(() => {
      this.users = this.users.filter(u => u.userId !== userId);
      this.successMessage = `${user.fullName} was deleted.`;
      this.cdr.markForCheck();
      setTimeout(() => { this.successMessage = ''; this.cdr.markForCheck(); }, 3000);
    });
  }

  // ── HELPERS ────────────────────────────────────────────────────────────────
  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  goBack(): void {
    this.router.navigate(['/admin/dashboard']);
  }
}