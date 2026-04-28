import { AdminSidebarComponent } from '../../../shared/components/admin-sidebar/admin-sidebar.component';
import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../core/services/admin.service';
import { Subject, EMPTY } from 'rxjs';
import { takeUntil, finalize, catchError } from 'rxjs/operators';

export interface AdminOrder {
  orderId: string;
  customerId: string;
  customerName: string;
  restaurantName: string;
  totalAmount: number;
  status: any;
  placedAt: Date;
}

// Must match the UpdateOrderStatusDto on the backend (enum values)
export const ORDER_STATUS_OPTIONS = [
  { label: 'Pending',          value: 0 },
  { label: 'Preparing',        value: 1 },
  { label: 'Ready for Pickup', value: 2 },
  { label: 'Out for Delivery', value: 3 },
  { label: 'Delivered',        value: 4 },
  { label: 'Cancelled',        value: 5 },
];


@Component({
  selector: 'app-admin-order-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AdminSidebarComponent],
  templateUrl: './order-management.component.html',
  styleUrls: ['./order-management.component.css']
})
export class AdminOrderManagementComponent implements OnInit, OnDestroy {

  searchQuery = '';
  selectedStatus = 'all';
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  orders: AdminOrder[] = [];

  statusFilters = ['all', 'Pending', 'Preparing', 'ReadyForPickup', 'OutForDelivery', 'Delivered', 'Cancelled'];
  statusOptions = ORDER_STATUS_OPTIONS;

  // Status update modal state
  showStatusModal = false;
  updatingOrder: AdminOrder | null = null;
  selectedNewStatus: number = 0;
  isUpdatingStatus = false;
  statusUpdateError = '';

  // Track which order rows are in-flight
  updatingOrderId: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private router: Router,
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.markForCheck();

    this.adminService.getAllOrders().pipe(
      takeUntil(this.destroy$),
      catchError(err => {
        console.error('Failed to load orders', err);
        this.errorMessage = 'Failed to load orders.';
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.isLoading = false;
        this.cdr.markForCheck();
      })
    ).subscribe(data => {
      if (data) {
        this.orders = data;
        this.cdr.markForCheck();
      }
    });
  }

  get filteredOrders(): AdminOrder[] {
    let filtered = this.orders;

    if (this.selectedStatus !== 'all') {
      filtered = filtered.filter(o => this.getStatusLabel(o.status).replace(/\s+/g, '') === this.selectedStatus.replace(/\s+/g, ''));
    }

    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      filtered = filtered.filter(o =>
        (o.orderId && o.orderId.toLowerCase().includes(query)) ||
        (o.customerName && o.customerName.toLowerCase().includes(query)) ||
        (o.restaurantName && o.restaurantName.toLowerCase().includes(query))
      );
    }

    return filtered;
  }

  getStatusLabel(status: any): string {
    if (typeof status === 'string') return status;
    const labels: { [key: number]: string } = {
      0: 'Pending', 1: 'Preparing', 2: 'Ready for Pickup',
      3: 'Out for Delivery', 4: 'Delivered', 5: 'Cancelled'
    };
    return labels[status] ?? 'Unknown';
  }

  getStatusValue(status: any): number {
    if (typeof status === 'number') return status;
    const map: { [key: string]: number } = {
      'Pending': 0, 'Preparing': 1, 'Ready for Pickup': 2,
      'Out for Delivery': 3, 'Delivered': 4, 'Cancelled': 5
    };
    return map[status] ?? 0;
  }

  getStatusColorClasses(status: any): string {
    const label = this.getStatusLabel(status);
    if (label === 'Delivered') return 'border-emerald-400/30 text-emerald-400 bg-emerald-400/10';
    if (label === 'Pending') return 'border-yellow-400/30 text-yellow-400 bg-yellow-400/10';
    if (label === 'Preparing') return 'border-blue-400/30 text-blue-400 bg-blue-400/10';
    if (label === 'Cancelled') return 'border-red-400/30 text-red-400 bg-red-400/10';
    return 'border-gray-400/30 text-gray-400 bg-gray-400/10';
  }

  // ── STATUS UPDATE MODAL ────────────────────────────────────────────────────
  openStatusModal(order: AdminOrder): void {
    this.updatingOrder = order;
    this.selectedNewStatus = this.getStatusValue(order.status);
    this.statusUpdateError = '';
    this.showStatusModal = true;
    this.cdr.markForCheck();
  }

  cancelStatusUpdate(): void {
    this.showStatusModal = false;
    this.updatingOrder = null;
    this.statusUpdateError = '';
    this.cdr.markForCheck();
  }

  saveStatusUpdate(): void {
    if (!this.updatingOrder) return;

    this.isUpdatingStatus = true;
    this.statusUpdateError = '';
    this.updatingOrderId = this.updatingOrder.orderId;

    this.adminService.updateOrderStatus(this.updatingOrder.orderId, { status: this.selectedNewStatus }).pipe(
      takeUntil(this.destroy$),
      catchError(err => {
        console.error('Failed to update order status', err);
        this.statusUpdateError = err?.error?.message || 'Failed to update status. Please try again.';
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.isUpdatingStatus = false;
        this.updatingOrderId = null;
        this.cdr.markForCheck();
      })
    ).subscribe(() => {
      const idx = this.orders.findIndex(o => o.orderId === this.updatingOrder!.orderId);
      if (idx !== -1) {
        this.orders[idx] = { ...this.orders[idx], status: this.selectedNewStatus };
      }
      this.successMessage = `Order status updated to "${this.getStatusLabel(this.selectedNewStatus)}".`;
      this.showStatusModal = false;
      this.updatingOrder = null;
      this.cdr.markForCheck();
      setTimeout(() => { this.successMessage = ''; this.cdr.markForCheck(); }, 3000);
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/dashboard']);
  }
}