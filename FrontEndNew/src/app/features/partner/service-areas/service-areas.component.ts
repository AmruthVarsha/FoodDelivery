import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { PartnerService, Restaurant, ServiceArea } from '../../../core/services/partner.service';
import { PartnerSidebarComponent } from '../../../shared/components/partner-sidebar/partner-sidebar.component';

@Component({
  selector: 'app-partner-service-areas',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, RouterModule, PartnerSidebarComponent],
  templateUrl: './service-areas.component.html',
})
export class PartnerServiceAreasComponent implements OnInit, OnDestroy {
  selectedRestaurant: Restaurant | null = null;
  restaurants: Restaurant[] = [];

  serviceAreas: ServiceArea[] = [];
  isLoading = true;
  isAdding = false;
  errorMessage = '';
  successMessage = '';

  newPincode = '';
  pincodeError = '';
  deletingId: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private partnerService: PartnerService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.partnerService.getMyRestaurants().pipe(takeUntil(this.destroy$)).subscribe({
      next: list => {
        this.restaurants = list;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load restaurants.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });

    this.partnerService.selectedRestaurant$.pipe(
      takeUntil(this.destroy$),
      filter(r => r !== null)
    ).subscribe(restaurant => {
      this.selectedRestaurant = restaurant;
      this.loadServiceAreas(restaurant!.id);
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  selectRestaurant(r: Restaurant): void {
    this.partnerService.setSelectedRestaurant(r);
  }

  loadServiceAreas(restaurantId: string): void {
    this.isLoading = true;
    this.clearMessages();
    this.cdr.markForCheck();

    this.partnerService.getServiceAreas(restaurantId).pipe(takeUntil(this.destroy$)).subscribe({
      next: areas => {
        this.serviceAreas = areas;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load service areas.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  validatePincode(): boolean {
    this.pincodeError = '';
    const trimmed = this.newPincode.trim();
    if (!trimmed) {
      this.pincodeError = 'Pincode is required.';
      return false;
    }
    if (!/^\d{6}$/.test(trimmed)) {
      this.pincodeError = 'Pincode must be exactly 6 digits.';
      return false;
    }
    if (this.serviceAreas.some(a => a.pincode === trimmed)) {
      this.pincodeError = 'This pincode is already in your service area.';
      return false;
    }
    return true;
  }

  addPincode(): void {
    if (!this.validatePincode() || !this.selectedRestaurant || this.isAdding) return;

    this.isAdding = true;
    this.clearMessages();
    this.cdr.markForCheck();

    this.partnerService.addServiceArea(this.selectedRestaurant.id, this.newPincode.trim())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = `Pincode ${this.newPincode.trim()} added successfully.`;
          this.newPincode = '';
          this.isAdding = false;
          this.loadServiceAreas(this.selectedRestaurant!.id);
        },
        error: (err) => {
          this.errorMessage = err?.error?.message || 'Failed to add pincode.';
          this.isAdding = false;
          this.cdr.markForCheck();
        }
      });
  }

  removePincode(area: ServiceArea): void {
    if (this.deletingId === area.id) return;
    this.deletingId = area.id;
    this.clearMessages();
    this.cdr.markForCheck();

    this.partnerService.removeServiceArea(area.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = `Pincode ${area.pincode} removed.`;
        this.deletingId = null;
        this.serviceAreas = this.serviceAreas.filter(a => a.id !== area.id);
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Failed to remove pincode.';
        this.deletingId = null;
        this.cdr.markForCheck();
      }
    });
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  get restaurantName(): string {
    return this.selectedRestaurant?.name ?? '';
  }

  logout(): void {
    // handled by sidebar
  }
}
