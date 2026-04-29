import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DeliveryService } from '../../../core/services/delivery.service';
import {
  DeliveryOrderResponseDTO,
  DeliveryStatus,
  UpdateDeliveryStatusDTO,
  RestaurantOrderStatus
} from '../../../shared/models/order.model';

/** Delivery agent earns 8% of the order total per delivery. */
const DELIVERY_COMMISSION_RATE = 0.08;

@Component({
  selector: 'app-delivery-assigned-tasks',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './delivery-assigned-tasks.html',
  styleUrl: './delivery-assigned-tasks.css',
})
export class DeliveryAssignedTasks implements OnInit, OnDestroy {

  assignments: DeliveryOrderResponseDTO[] = [];
  isLoading = true;
  errorMessage = '';
  updatingId: string | null = null;

  readonly DeliveryStatus = DeliveryStatus;
  readonly RestaurantOrderStatus = RestaurantOrderStatus;

  private destroy$ = new Subject<void>();

  constructor(
    private deliveryService: DeliveryService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadAssignments();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAssignments(): void {
    this.isLoading = true;
    this.deliveryService.getAssignments().pipe(takeUntil(this.destroy$)).subscribe({
      next: (list) => {
        this.assignments = list;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load assignments.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  canPickUp(assignment: DeliveryOrderResponseDTO): boolean {
    return assignment.assignmentStatus === DeliveryStatus.Assigned;
  }

  canDeliver(assignment: DeliveryOrderResponseDTO): boolean {
    return assignment.assignmentStatus === DeliveryStatus.PickedUp;
  }

  updateStatus(assignment: DeliveryOrderResponseDTO, status: DeliveryStatus): void {
    this.updatingId = assignment.assignmentId;
    const dto: UpdateDeliveryStatusDTO = { status };

    this.deliveryService.updateDeliveryStatus(assignment.assignmentId, dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updated) => {
          const idx = this.assignments.findIndex(a => a.assignmentId === assignment.assignmentId);
          if (idx > -1) {
            this.assignments = [
              ...this.assignments.slice(0, idx),
              updated,
              ...this.assignments.slice(idx + 1)
            ];
          }
          this.updatingId = null;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = err?.error?.message || 'Failed to update status.';
          this.updatingId = null;
          this.cdr.detectChanges();
        }
      });
  }

  markPaymentPaid(assignment: DeliveryOrderResponseDTO): void {
    if (this.updatingId === assignment.assignmentId) return;
    this.updatingId = assignment.assignmentId;
    
    this.deliveryService.updatePaymentStatus(assignment.assignmentId, { status: 'Completed' })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updated) => {
          const idx = this.assignments.findIndex(a => a.assignmentId === assignment.assignmentId);
          if (idx > -1) {
            this.assignments = [
              ...this.assignments.slice(0, idx),
              updated,
              ...this.assignments.slice(idx + 1)
            ];
          }
          this.updatingId = null;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = err?.error?.message || 'Failed to update payment status.';
          this.updatingId = null;
          this.cdr.detectChanges();
        }
      });
  }

  getStopStatusClass(status: string): string {
    const map: Record<string, string> = {
      [RestaurantOrderStatus.ReadyForPickup]: 'text-green-400',
      [RestaurantOrderStatus.Preparing]:      'text-yellow-400',
      [RestaurantOrderStatus.Pending]:        'text-zinc-400',
      [RestaurantOrderStatus.PickedUp]:       'text-purple-400',
    };
    return map[status] ?? 'text-zinc-400';
  }

  getAssignmentStatusClass(status: string): string {
    const map: Record<string, string> = {
      [DeliveryStatus.Assigned]:  'bg-blue-500/15 border-blue-500/40 text-blue-300',
      [DeliveryStatus.PickedUp]:  'bg-purple-500/15 border-purple-500/40 text-purple-300',
      [DeliveryStatus.Delivered]: 'bg-green-500/15 border-green-500/40 text-green-300',
    };
    return map[status] ?? 'bg-zinc-500/20 border-zinc-500/40 text-zinc-300';
  }

  /** Returns the agent's earning for a single assignment (exposed for template). */
  deliveryEarning(totalAmount: number): number {
    return Math.round(totalAmount * DELIVERY_COMMISSION_RATE * 100) / 100;
  }
}
