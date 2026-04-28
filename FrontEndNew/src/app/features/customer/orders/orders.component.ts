import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { OrderResponseDTO } from '../../../shared/models/order.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent],
  templateUrl: './orders.component.html',
})
export class OrdersComponent implements OnInit {
  orders: OrderResponseDTO[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(
    private orderService: OrderService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.orderService.getMyOrders().subscribe({
      next: (orders) => {
        // Sort newest first
        this.orders = orders.sort(
          (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load your orders. Please try again.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  viewOrder(orderId: string): void {
    this.router.navigate(['/customer/order-tracking', orderId]);
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Placed':              'bg-yellow-500/10 text-yellow-400 border-yellow-500/30',
      'Paid':                'bg-blue-500/10 text-blue-400 border-blue-500/30',
      'InProgress':          'bg-purple-500/10 text-purple-400 border-purple-500/30',
      'OutForDelivery':      'bg-cyan-500/10 text-cyan-400 border-cyan-500/30',
      'Delivered':           'bg-emerald-500/10 text-emerald-400 border-emerald-500/30',
      'CancelledByCustomer': 'bg-red-500/10 text-red-400 border-red-500/30',
      'CancelledBySystem':   'bg-red-500/10 text-red-400 border-red-500/30',
    };
    return map[status] ?? 'bg-slate-500/10 text-slate-400 border-slate-500/30';
  }

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      'Placed':              'Placed',
      'Paid':                'Paid',
      'InProgress':          'In Progress',
      'OutForDelivery':      'Out for Delivery',
      'Delivered':           'Delivered',
      'CancelledByCustomer': 'Cancelled',
      'CancelledBySystem':   'Cancelled',
    };
    return map[status] ?? status;
  }

  getStatusIcon(status: string): string {
    const map: Record<string, string> = {
      'Placed':              'receipt',
      'Paid':                'payments',
      'InProgress':          'restaurant',
      'OutForDelivery':      'delivery_dining',
      'Delivered':           'check_circle',
      'CancelledByCustomer': 'cancel',
      'CancelledBySystem':   'cancel',
    };
    return map[status] ?? 'info';
  }

  isActiveOrder(status: string): boolean {
    return ['Placed', 'Paid', 'InProgress', 'OutForDelivery'].includes(status);
  }

  formatDate(date: Date | string): string {
    return new Date(date).toLocaleDateString('en-IN', {
      day: 'numeric', month: 'short', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }
}
