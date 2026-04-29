import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../../../core/services/auth.service';
import { LoginDTO } from '../../../shared/models/auth.model';

/**
 * Login Component
 * 
 * Handles user login with email and password.
 * Supports Two-Factor Authentication flow.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  
  loginForm!: FormGroup;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  rememberMe = false;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Initialize the login form with validation
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    console.log('Login component initialized');
    console.log('Form valid:', this.loginForm.valid);
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    console.log('=== LOGIN FORM SUBMITTED ===');
    console.log('Form valid:', this.loginForm.valid);
    
    // Clear previous messages
    this.errorMessage = '';
    this.successMessage = '';

    // Check if form is valid
    if (this.loginForm.invalid) {
      console.log('Form is INVALID, marking fields as touched');
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    console.log('Form is VALID, proceeding with login...');
    this.isSubmitting = true;
    
    const credentials: LoginDTO = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };
    
    console.log('Calling authService.login with:', { email: credentials.email, password: '***' });

    this.authService.login(credentials)
      .pipe(
        finalize(() => {
          console.log('=== LOGIN REQUEST COMPLETED (Success or Error) ===');
          this.isSubmitting = false;
          this.cdr.markForCheck();
        })
      )
      .subscribe({
        next: (response) => {
          console.log('=== LOGIN SUCCESS ===');
          // Check if 2FA is required
          if (response.requireTwoFactor === true) {
            this.successMessage = 'Please check your email for the verification code.';
            this.router.navigate(['/auth/two-factor'], { 
              queryParams: { email: credentials.email } 
            });
            return;
          }
          
          // Check if token is present (normal login)
          if (response.token) {
            this.successMessage = 'Login successful! Redirecting...';
            this.cdr.markForCheck();
            
            // Give auth service a moment to process the token and load profile
            setTimeout(() => {
              const currentUser = this.authService.currentUserValue;
              if (currentUser) {
                this.redirectBasedOnRole(currentUser.role);
              } else {
                const userSub = this.authService.currentUser$.subscribe(user => {
                  if (user) {
                    this.redirectBasedOnRole(user.role);
                    userSub.unsubscribe();
                  }
                });
                
                // Fallback timeout
                setTimeout(() => {
                  const user = this.authService.currentUserValue;
                  if (user) {
                    this.redirectBasedOnRole(user.role);
                  } else {
                    this.router.navigate(['/customer/dashboard']);
                  }
                  userSub.unsubscribe();
                }, 3000);
              }
            }, 500);
          } else {
            this.errorMessage = 'Unexpected response from server. Please try again.';
            this.cdr.markForCheck();
          }
        },
        error: (error) => {
          console.error('=== LOGIN ERROR ===');
          console.error('Error object:', error);
          
          // Extract error message using our robust logic
          let errorMsg = 'Login failed. Please check your credentials and try again.';
          
          if (error.error) {
            const errBody = error.error;
            if (typeof errBody === 'string') {
              errorMsg = errBody;
            } else {
              errorMsg = errBody.error || errBody.message || errBody.title;
            }
          } else if (error.message) {
            errorMsg = error.message;
          }
          
          this.errorMessage = errorMsg;
          console.log('Error message set to:', this.errorMessage);
          this.cdr.markForCheck();
        }
      });
    
    console.log('Subscribe called, waiting for response...');
  }

  /**
   * Redirect user based on their role
   * Handles both string and number role types
   */
  private redirectBasedOnRole(role: string | number): void {
    console.log('Redirecting based on role:', role);
    
    // Convert string role to number if needed
    let roleNum: number;
    
    if (typeof role === 'string') {
      const roleMap: { [key: string]: number } = {
        'Customer': 0,
        'Partner': 1,
        'DeliveryAgent': 2,
        'Delivery Agent': 2,
        'Admin': 3
      };
      roleNum = roleMap[role] ?? 0; // Default to Customer if unknown
    } else {
      roleNum = role;
    }
    
    console.log('Role number:', roleNum);
    
    switch (roleNum) {
      case 0: // Customer
        console.log('Navigating to customer dashboard');
        this.router.navigate(['/customer/dashboard']);
        break;
      case 1: // Partner
        console.log('Navigating to partner dashboard');
        this.router.navigate(['/partner/dashboard']);
        break;
      case 2: // DeliveryAgent
        console.log('Navigating to delivery dashboard');
        this.router.navigate(['/delivery/dashboard']);
        break;
      case 3: // Admin
        console.log('Navigating to admin dashboard');
        this.router.navigate(['/admin/dashboard']);
        break;
      default:
        console.log('Unknown role, navigating to customer dashboard');
        this.router.navigate(['/customer/dashboard']);
    }
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  /**
   * Check if a form field has an error
   */
  hasError(fieldName: string, errorType: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field?.hasError(errorType) && field?.touched);
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    
    if (field?.hasError('required')) {
      return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
    }
    
    if (field?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `Password must be at least ${minLength} characters`;
    }
    
    return '';
  }

  /**
   * Toggle password visibility
   */
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
    console.log('Password visibility toggled:', this.showPassword);
  }
}
