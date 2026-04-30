import { PartnerSidebarComponent } from '../../../shared/components/partner-sidebar/partner-sidebar.component';
import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { PartnerService, Restaurant } from '../../../core/services/partner.service';
import {
  PartnerOrderResponseDTO,
  UpdateRestaurantOrderStatusDTO,
  RestaurantOrderStatus
} from '../../../shared/models/order.model';
import { Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

interface StatusFilter {
  label: string;
  value: string;
  count: number;
}

@Component({
  selector: 'app-partner-orders',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, PartnerSidebarComponent],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class PartnerOrdersComponent implements OnInit, OnDestroy {

  isLoading = false;
  errorMessage = '';
  restaurantName = '';
  selectedFilter = 'all';
  showRestaurantDropdown = false;

  // Modal for rejecting/cancelling with reason
  showReasonModal = false;
  reasonInput = '';
  pendingAction: { subOrderId: string; status: RestaurantOrderStatus } | null = null;

  restaurants: Restaurant[] = [];
  selectedRestaurant: Restaurant | null = null;
  orders: PartnerOrderResponseDTO[] = [];

  readonly RestaurantOrderStatus = RestaurantOrderStatus;

  statusFilters: StatusFilter[] = [
    { label: 'All',           value: 'all',          count: 0 },
    { label: 'Pending',       value: 'Pending',       count: 0 },
    { label: 'Accepted',      value: 'Accepted',      count: 0 },
    { label: 'Preparing',     value: 'Preparing',     count: 0 },
    { label: 'Ready',         value: 'ReadyForPickup',count: 0 },
    { label: 'Completed',     value: 'PickedUp',      count: 0 }
  ];

  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private partnerService: PartnerService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.partnerService.getMyRestaurants().pipe(takeUntil(this.destroy$)).subscribe({
      next: (list) => { this.restaurants = list; this.cdr.markForCheck(); },
      error: () => { this.errorMessage = 'Failed to load restaurants.'; this.cdr.markForCheck(); }
    });

    this.partnerService.selectedRestaurant$.pipe(
      takeUntil(this.destroy$),
      filter(r => r !== null)
    ).subscribe(restaurant => {
      this.selectedRestaurant = restaurant;
      this.restaurantName = restaurant!.name;
      this.cdr.markForCheck();
      this.loadOrders(restaurant!.id);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  selectRestaurant(restaurant: Restaurant): void {
    this.partnerService.setSelectedRestaurant(restaurant);
    this.showRestaurantDropdown = false;
  }

  loadOrders(restaurantId: string): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.markForCheck();

    this.partnerService.getRestaurantSubOrders(restaurantId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (orders) => {
        this.orders = orders;
        this.updateFilterCounts();
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load orders.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  updateFilterCounts(): void {
    this.statusFilters[0].count = this.orders.length;
    for (let i = 1; i < this.statusFilters.length; i++) {
      this.statusFilters[i].count = this.orders.filter(o => o.status === this.statusFilters[i].value).length;
    }
  }

  get filteredOrders(): PartnerOrderResponseDTO[] {
    if (this.selectedFilter === 'all') return this.orders;
    return this.orders.filter(o => o.status === this.selectedFilter);
  }

  // ─── Status transitions ───────────────────────────────────────────────────

  getNextAction(status: string): { label: string; nextStatus: RestaurantOrderStatus } | null {
    const map: Record<string, { label: string; nextStatus: RestaurantOrderStatus }> = {
      [RestaurantOrderStatus.Pending]:   { label: 'Accept',         nextStatus: RestaurantOrderStatus.Accepted },
      [RestaurantOrderStatus.Accepted]:  { label: 'Start Preparing', nextStatus: RestaurantOrderStatus.Preparing },
      [RestaurantOrderStatus.Preparing]: { label: 'Mark Ready',     nextStatus: RestaurantOrderStatus.ReadyForPickup },
    };
    return map[status] ?? null;
  }

  canReject(status: string): boolean {
    return status === RestaurantOrderStatus.Pending;
  }

  canCancel(status: string): boolean {
    return status === RestaurantOrderStatus.Accepted || status === RestaurantOrderStatus.Preparing;
  }

  doNextAction(order: PartnerOrderResponseDTO): void {
    const action = this.getNextAction(order.status);
    if (!action || !this.selectedRestaurant) return;
    this.updateStatus(order.subOrderId, { status: action.nextStatus });
  }

  openReasonModal(subOrderId: string, status: RestaurantOrderStatus): void {
    this.pendingAction = { subOrderId, status };
    this.reasonInput = '';
    this.showReasonModal = true;
  }

  confirmWithReason(): void {
    if (!this.pendingAction || !this.selectedRestaurant || !this.reasonInput.trim()) return;
    this.updateStatus(this.pendingAction.subOrderId, {
      status: this.pendingAction.status,
      cancellationReason: this.reasonInput.trim()
    });
    this.showReasonModal = false;
    this.pendingAction = null;
  }

  updateStatus(subOrderId: string, dto: UpdateRestaurantOrderStatusDTO): void {
    if (!this.selectedRestaurant) return;
    this.partnerService.updateSubOrderStatus(this.selectedRestaurant.id, subOrderId, dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updated) => {
          const idx = this.orders.findIndex(o => o.subOrderId === subOrderId);
          if (idx > -1) {
            this.orders = [
              ...this.orders.slice(0, idx),
              updated,
              ...this.orders.slice(idx + 1)
            ];
            this.updateFilterCounts();
          }
          this.cdr.markForCheck();
        },
        error: (err) => {
          this.errorMessage = err?.error?.message || 'Failed to update status.';
          this.cdr.markForCheck();
        }
      });
  }

  // ─── UI helpers ───────────────────────────────────────────────────────────

  getStatusBadgeClass(status: string): string {
    const map: Record<string, string> = {
      [RestaurantOrderStatus.Pending]:       'bg-surface-container border-outline text-on-surface-variant',
      [RestaurantOrderStatus.Accepted]:      'bg-tertiary-container border-tertiary text-on-tertiary-container',
      [RestaurantOrderStatus.Preparing]:     'bg-tertiary-fixed border-tertiary text-tertiary',
      [RestaurantOrderStatus.ReadyForPickup]:'bg-secondary-fixed border-secondary text-secondary',
      [RestaurantOrderStatus.PickedUp]:      'bg-secondary-container border-secondary text-on-secondary-container',
      [RestaurantOrderStatus.Cancelled]:     'bg-error-container border-error text-on-error-container',
      [RestaurantOrderStatus.Rejected]:      'bg-error-container border-error text-on-error-container',
    };
    return map[status] ?? 'bg-surface-container border-outline text-on-surface-variant';
  }

  logout(): void {
    this.authService.logout().subscribe({ next: () => this.router.navigate(['/auth/login']) });
  }
}