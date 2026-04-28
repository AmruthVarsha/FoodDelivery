import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DeliveryService } from '../../../core/services/delivery.service';
import { AgentProfileResponseDTO, UpsertAgentProfileDTO } from '../../../shared/models/order.model';

@Component({
  selector: 'app-delivery-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './delivery-profile.html',
  styleUrl: './delivery-profile.css',
})
export class DeliveryProfile implements OnInit, OnDestroy {

  profile: AgentProfileResponseDTO | null = null;
  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  // Form fields
  pincodeInput = '';
  isActiveInput = false;

  private destroy$ = new Subject<void>();

  constructor(
    private deliveryService: DeliveryService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.deliveryService.getProfile().pipe(takeUntil(this.destroy$)).subscribe({
      next: (p) => {
        this.profile = p;
        this.pincodeInput = p.currentPincode;
        this.isActiveInput = p.isActive;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        // 404 = profile not registered yet — show the register form
        if (err.status === 404) {
          this.profile = null;
          this.isLoading = false;
        } else {
          this.errorMessage = 'Failed to load profile.';
          this.isLoading = false;
        }
        this.cdr.detectChanges();
      }
    });
  }

  saveProfile(): void {
    if (!this.pincodeInput.trim()) {
      this.errorMessage = 'Please enter your service pincode.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const dto: UpsertAgentProfileDTO = {
      currentPincode: this.pincodeInput.trim(),
      isActive: this.isActiveInput
    };

    this.deliveryService.upsertProfile(dto).pipe(takeUntil(this.destroy$)).subscribe({
      next: (p) => {
        this.profile = p;
        this.pincodeInput = p.currentPincode;
        this.isActiveInput = p.isActive;
        this.isSaving = false;
        this.successMessage = 'Profile updated! You will now be auto-assigned orders in pincode ' + p.currentPincode + '.';
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Failed to save profile.';
        this.isSaving = false;
        this.cdr.detectChanges();
      }
    });
  }

  goOnline(): void {
    this.isActiveInput = true;
    this.saveProfile();
  }

  goOffline(): void {
    this.isActiveInput = false;
    this.saveProfile();
  }
}
