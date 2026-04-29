import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from '../../../core/services/auth.service';
import { DeliveryService } from '../../../core/services/delivery.service';
import { AgentProfileResponseDTO } from '../../../shared/models/order.model';

@Component({
  selector: 'app-delivery-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './delivery-layout.html',
  styleUrl: './delivery-layout.css',
})
export class DeliveryLayoutComponent implements OnInit, OnDestroy {

  agentName = '';
  agentInitials = '';
  profile: AgentProfileResponseDTO | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private deliveryService: DeliveryService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Load user name from auth
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        if (user) {
          this.agentName = user.fullName || user.email || 'Agent';
          const parts = this.agentName.trim().split(' ');
          this.agentInitials = parts.length >= 2
            ? (parts[0][0] + parts[1][0]).toUpperCase()
            : this.agentName.substring(0, 2).toUpperCase();
        }
        this.cdr.markForCheck();
      });

    // Load profile for online status reactively
    this.deliveryService.profile$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (p) => {
          this.profile = p;
          this.cdr.markForCheck();
        }
      });

    // Initial fetch to populate the subject
    this.deliveryService.getProfile().subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/auth/login']),
      error: () => this.router.navigate(['/auth/login'])
    });
  }
}
