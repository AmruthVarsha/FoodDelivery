import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CatalogService, Restaurant as BackendRestaurant } from '../../../core/services/catalog.service';
import { AuthService } from '../../../core/services/auth.service';
import { CartService, CartItem } from '../../../core/services/cart.service';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';

interface Restaurant extends BackendRestaurant {
  isFavorite?: boolean;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, NavbarComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  searchQuery: string = '';
  deliveryAddress: string = '';
  selectedCuisine: string = 'All Cuisines';
  isLoading: boolean = false;
  errorMessage: string = '';
  isAuthenticated: boolean = false;
  
  cuisines: string[] = [
    'All Cuisines',
    'Sushi',
    'Italian',
    'Burgers',
    'Vegan',
    'Desserts'
  ];

  restaurants: Restaurant[] = [];
  filteredRestaurants: Restaurant[] = [];

  cartItems: CartItem[] = [];

  constructor(
    private catalogService: CatalogService, 
    private cartService: CartService,
    private router: Router,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Check authentication status
    this.checkAuthentication();
    
    this.loadRestaurants();
    
    // Subscribe to cart changes
    this.cartService.cartItems$.subscribe(items => {
      this.cartItems = items;
      this.cdr.detectChanges();
    });
  }

  checkAuthentication(): void {
    // Subscribe to authentication state
    this.authService.isAuthenticated$.subscribe(isAuth => {
      this.isAuthenticated = isAuth;
      this.cdr.detectChanges();
    });

    // If token exists but no user profile, fetch it
    if (this.authService.getAccessToken() && !this.authService.currentUserValue) {
      this.authService.getProfile().subscribe({
        next: () => {
          this.isAuthenticated = true;
          this.cdr.detectChanges();
        },
        error: () => {
          this.isAuthenticated = false;
          this.cdr.detectChanges();
        }
      });
    }
  }

  loadRestaurants(): void {
    this.isLoading = true;
    this.errorMessage = '';
    console.log('Loading restaurants...');

    this.catalogService.getRestaurants().subscribe({
      next: (data) => {
        console.log('Raw data from API:', data);
        this.restaurants = data.map(restaurant => ({
          ...restaurant,
          // Map backend fields to display fields
          imageUrl: restaurant.logoUrl,
          cuisineType: restaurant.cuisines?.join(', ') || 'Various',
          deliveryTime: `${restaurant.prepTimeMinutes || 20}-${(restaurant.prepTimeMinutes || 20) + 10} min`,
          minimumOrder: 10, // Default value since backend doesn't provide this
          isFavorite: false
        }));
        console.log('Mapped restaurants:', this.restaurants);
        this.filteredRestaurants = [...this.restaurants];
        console.log('Filtered restaurants:', this.filteredRestaurants);
        console.log('Setting isLoading to false');
        this.isLoading = false;
        this.cdr.detectChanges(); // Manually trigger change detection
        console.log('Change detection triggered');
      },
      error: (error) => {
        console.error('Error in subscribe:', error);
        this.errorMessage = 'Failed to load restaurants. Please try again.';
        this.isLoading = false;
        this.cdr.detectChanges();
        console.error('Error loading restaurants:', error);
      }
    });
  }

  selectCuisine(cuisine: string): void {
    this.selectedCuisine = cuisine;
    this.filterRestaurants();
  }

  filterRestaurants(): void {
    if (this.selectedCuisine === 'All Cuisines') {
      this.filteredRestaurants = [...this.restaurants];
    } else {
      this.filteredRestaurants = this.restaurants.filter(r => 
        r.cuisines?.some(cuisine => 
          cuisine.toLowerCase().includes(this.selectedCuisine.toLowerCase())
        ) || r.cuisineType?.toLowerCase().includes(this.selectedCuisine.toLowerCase())
      );
    }

    // Apply search filter if search query exists
    if (this.searchQuery) {
      this.filteredRestaurants = this.filteredRestaurants.filter(r =>
        r.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        r.cuisineType?.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        r.cuisines?.some(cuisine => 
          cuisine.toLowerCase().includes(this.searchQuery.toLowerCase())
        )
      );
    }
  }

  toggleFavorite(restaurant: Restaurant): void {
    restaurant.isFavorite = !restaurant.isFavorite;
  }

  addToCart(restaurant: Restaurant): void {
    // Navigate to restaurant detail page
    this.router.navigate(['/customer/restaurant', restaurant.id]);
  }

  updateCartQuantity(item: CartItem, change: number): void {
    const newQuantity = item.quantity + change;
    this.cartService.updateQuantity(item.menuItem.id, newQuantity);
  }

  removeFromCart(item: CartItem): void {
    this.cartService.removeItem(item.menuItem.id);
  }

  get cartSubtotal(): number {
    return this.cartItems.reduce((sum, item) => sum + (item.menuItem.price * item.quantity), 0);
  }

  get deliveryFee(): number {
    return this.cartSubtotal > 500 ? 0 : 40;
  }

  get cartTotal(): number {
    return this.cartSubtotal + this.deliveryFee;
  }

  get cartItemCount(): number {
    return this.cartItems.reduce((sum, item) => sum + item.quantity, 0);
  }

  findFood(): void {
    this.filterRestaurants();
  }

  goToCheckout(): void {
    this.router.navigate(['/customer/checkout']);
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        console.error('Logout error:', error);
        // Even if API call fails, redirect to login
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
