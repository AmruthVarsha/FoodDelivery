import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';

interface OrderItem {
  name: string;
  quantity: number;
  price: number;
}

interface OrderStatus {
  step: number;
  title: string;
  description: string;
  time: string;
  icon: string;
  completed: boolean;
  active: boolean;
}

interface DeliveryAgent {
  name: string;
  rating: number;
  totalDeliveries: string;
  vehicle: string;
  licensePlate: string;
  imageUrl: string;
}

@Component({
  selector: 'app-order-tracking',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent],
  templateUrl: './order-tracking.component.html',
  styleUrls: ['./order-tracking.component.css']
})
export class OrderTrackingComponent implements OnInit {
  orderId!: string;
  orderNumber: string = 'QB-8829';
  restaurantName: string = 'The Burger Theory';
  
  orderItems: OrderItem[] = [];
  orderStatuses: OrderStatus[] = [];
  deliveryAgent: DeliveryAgent | null = null;
  
  subtotal: number = 0;
  deliveryFee: number = 0;
  total: number = 0;
  
  estimatedArrival: string = '12:45';
  currentLocation: string = 'Sunset Boulevard, 42';
  distanceFromLocation: string = '2.4 miles';
  
  isLoading: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.orderId = params['id'];
      this.loadOrderDetails();
    });
  }

  loadOrderDetails(): void {
    this.isLoading = true;

    // TODO: Implement actual API call to fetch order details
    // For now, showing empty state
    this.isLoading = false;
    this.cdr.detectChanges();
    
    // Example of what the API call should look like:
    // this.orderService.getOrderById(this.orderId).subscribe({
    //   next: (order) => {
    //     this.orderItems = order.items;
    //     this.subtotal = order.subtotal;
    //     this.deliveryFee = order.deliveryFee;
    //     this.total = order.totalAmount;
    //     this.deliveryAgent = order.deliveryAgent;
    //     this.orderStatuses = this.mapOrderStatus(order.status);
    //     this.isLoading = false;
    //     this.cdr.detectChanges();
    //   },
    //   error: (error) => {
    //     console.error('Error loading order:', error);
    //     this.isLoading = false;
    //     this.cdr.detectChanges();
    //   }
    // });
  }

  callDeliveryAgent(): void {
    // TODO: Implement call functionality
    console.log('Calling delivery agent...');
  }

  chatWithAgent(): void {
    // TODO: Implement chat functionality
    console.log('Opening chat with agent...');
  }

  contactSupport(): void {
    // TODO: Implement support contact
    console.log('Contacting support...');
  }

  getProgressPercentage(): number {
    const completedSteps = this.orderStatuses.filter(s => s.completed).length;
    return (completedSteps / this.orderStatuses.length) * 100;
  }
}
