import { PartnerSidebarComponent } from '../../../shared/components/partner-sidebar/partner-sidebar.component';
import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

import { PartnerService, Restaurant } from '../../../core/services/partner.service';
import { PartnerOrderResponseDTO } from '../../../shared/models/order.model';
import { forkJoin, Subject, EMPTY } from 'rxjs';
import { catchError, finalize, takeUntil, filter } from 'rxjs/operators';

interface DashboardStats {
  totalOrdersToday: number;
  activeOrders: number;
  menuItems: number;
  restaurantStatus: boolean;
}

@Component({
  selector: 'app-partner-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, PartnerSidebarComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class PartnerDashboardComponent implements OnInit, OnDestroy {
  restaurantName = '';
  restaurantId: string | null = null;
  isLoading = true;
  isTogglingStatus = false;
  errorMessage = '';
  showRestaurantDropdown = false;

  restaurants: Restaurant[] = [];
  selectedRestaurant: Restaurant | null = null;

  stats: DashboardStats = {
    totalOrdersToday: 0,
    activeOrders: 0,
    menuItems: 0,
    restaurantStatus: true
  };

  recentOrders: PartnerOrderResponseDTO[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private partnerService: PartnerService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Load all restaurants once, which also sets selectedRestaurant$ in the service
    this.partnerService.getMyRestaurants().pipe(takeUntil(this.destroy$)).subscribe({
      next: (list) => {
        this.restaurants = list;
        if (list.length === 0) {
          this.isLoading = false;
        }
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load restaurants.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });

    // React every time the selected restaurant changes
    this.partnerService.selectedRestaurant$.pipe(
      takeUntil(this.destroy$),
      filter(r => r !== null)
    ).subscribe(restaurant => {
      this.selectedRestaurant = restaurant;
      this.restaurantId = restaurant!.id;
      this.restaurantName = restaurant!.name;
      this.stats.restaurantStatus = restaurant!.isOpen;
      this.cdr.markForCheck();
      this.loadDashboardData(restaurant!.id);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  selectRestaurant(restaurant: Restaurant): void {
    this.partnerService.setSelectedRestaurant(restaurant);
    this.showRestaurantDropdown = false;
    this.cdr.markForCheck();
  }

  loadDashboardData(restaurantId: string): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.markForCheck();

    forkJoin({
      menuItems: this.partnerService.getMenuItems(restaurantId),
      orders: this.partnerService.getRestaurantSubOrders(restaurantId)
    }).pipe(
      catchError(error => {
        console.error('Dashboard load error:', error);
        this.errorMessage = 'Failed to load dashboard data. Please try again.';
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.isLoading = false;
        this.cdr.markForCheck();
      })
    ).subscribe(data => {
      if (!data) return;
      this.stats.menuItems = data.menuItems.length;
      this.recentOrders = data.orders.slice(0, 5);

      const today = new Date();
      today.setHours(0, 0, 0, 0);
      const todayOrders = data.orders.filter((order: PartnerOrderResponseDTO) => {
        const d = new Date(order.createdAt);
        d.setHours(0, 0, 0, 0);
        return d.getTime() === today.getTime();
      });
      this.stats.totalOrdersToday = todayOrders.length;
      this.stats.activeOrders = data.orders.filter((o: PartnerOrderResponseDTO) =>
        o.status === 'Pending' || o.status === 'Accepted' ||
        o.status === 'Preparing' || o.status === 'ReadyForPickup'
      ).length;

      this.cdr.markForCheck();
    });
  }

  getStatusLabel(status: any): string {
    if (typeof status === 'string') return status;
    const labels: { [key: number]: string } = { 0: 'Pending', 1: 'Preparing', 2: 'Ready', 3: 'Completed', 4: 'Cancelled' };
    return labels[status] || 'Unknown';
  }

  getStatusClass(status: any): string {
    if (typeof status === 'string') {
      if (status === 'Pending') return 'bg-yellow-500/10 border-yellow-500/30 text-yellow-400';
      if (status === 'Preparing' || status === 'RestaurantAccepted') return 'bg-primary-container/10 border-primary-container/30 text-primary-container';
      if (status === 'ReadyForPickup' || status === 'OutForDelivery') return 'bg-secondary-container/10 border-secondary-container/30 text-secondary';
      if (status === 'Delivered' || status === 'Completed' || status === 'PickedUp') return 'bg-green-500/10 border-green-500/30 text-green-400';
      if (status.includes('Cancel') || status.includes('Reject')) return 'bg-red-500/10 border-red-500/30 text-red-400';
      return 'bg-gray-500/10 border-gray-500/30 text-gray-400';
    }
    const statusClasses: { [key: number]: string } = {
      0: 'bg-yellow-500/10 border-yellow-500/30 text-yellow-400',
      1: 'bg-primary-container/10 border-primary-container/30 text-primary-container',
      2: 'bg-secondary-container/10 border-secondary-container/30 text-secondary',
      3: 'bg-green-500/10 border-green-500/30 text-green-400',
      4: 'bg-red-500/10 border-red-500/30 text-red-400'
    };
    return statusClasses[status] || statusClasses[0];
  }

  getItemsSummary(order: PartnerOrderResponseDTO): string {
    return order.items.map(item => `${item.quantity}x ${item.menuItemName}`).join(', ');
  }

  getTimeAgo(date: Date): string {
    const now = new Date();
    const orderDate = new Date(date);
    const diffMs = now.getTime() - orderDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins} mins ago`;
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `${diffHours} hours ago`;
    return `${Math.floor(diffHours / 24)} days ago`;
  }

  toggleRestaurantStatus(): void {
    if (!this.restaurantId || !this.selectedRestaurant || this.isTogglingStatus) return;

    const newStatus = !this.stats.restaurantStatus;
    this.isTogglingStatus = true;
    this.errorMessage = '';
    this.cdr.markForCheck();

    // PATCH only { isOpen } — the new dedicated status endpoint.
    this.partnerService.patchRestaurantStatus(this.restaurantId, newStatus).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: () => {
        this.stats.restaurantStatus = newStatus;
        this.isTogglingStatus = false;

        // Sync the BehaviorSubject so selectedRestaurant$ doesn't overwrite
        // stats.restaurantStatus back to the old value on its next emission.
        const synced = { ...this.selectedRestaurant!, isOpen: newStatus };
        this.selectedRestaurant = synced;
        // Update the subject directly without re-triggering loadDashboardData:
        // patch just the in-memory object; setSelectedRestaurant would re-fire
        // the ngOnInit subscription and call loadDashboardData unnecessarily.
        this.partnerService.patchSelectedRestaurant(synced);
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error updating restaurant status:', error);
        this.errorMessage = 'Failed to update restaurant status.';
        this.isTogglingStatus = false;
        this.cdr.markForCheck();
      }
    });
  }

  viewOrderDetails(order: PartnerOrderResponseDTO): void {
    this.router.navigate(['/partner/orders'], { queryParams: { orderId: order.subOrderId } });
  }

  navigateToOrders(): void { this.router.navigate(['/partner/orders']); }
  navigateToMenuItems(): void { this.router.navigate(['/partner/menu-items']); }
  navigateToSettings(): void { this.router.navigate(['/partner/settings']); }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/auth/login']),
      error: () => this.router.navigate(['/auth/login'])
    });
  }
}
