using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OrderService.Application.DTOs.Cart;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICatalogRepository _catalogRepository;

        public CartService(ICartItemRepository cartItemRepository, ICartRepository cartRepository, ICatalogRepository catalogRepository)
        {
            _cartItemRepository = cartItemRepository;
            _cartRepository = cartRepository;
            _catalogRepository = catalogRepository;
        }

        public async Task<IEnumerable<DisplayCartDTO>> GetCartItems(Guid id)
        {
            var cart = await _cartRepository.GetById(id);
            if (cart == null)
                throw new NotFoundException("Cart", id);

            return cart.CartItems.Select(ci => new DisplayCartDTO
            {
                Id = ci.Id,
                MenuItemId = ci.MenuItemId,
                MenuItemName = ci.MenuItemName,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                TotalPrice = ci.Quantity * ci.UnitPrice
            });
        }

        public async Task<CartResponseDTO> GetCartInfo(Guid id)
        {
            var cart = await _cartRepository.GetById(id);
            if (cart == null)
                throw new NotFoundException("Cart", id);

            return new CartResponseDTO
            {
                Id = cart.Id,
                RestaurantId = cart.RestaurantId,
                CustomerId = cart.CustomerId,
                Status = cart.Status
            };
        }

        public async Task<IEnumerable<CartResponseDTO>> GetUserActiveCarts(string userId)
        {
            var carts = await _cartRepository.GetByCustomerId(userId);
            return carts
                .Where(c => c.Status == CartStatus.Active)
                .Select(c => new CartResponseDTO
                {
                    Id = c.Id,
                    RestaurantId = c.RestaurantId,
                    CustomerId = c.CustomerId,
                    Status = c.Status
                });
        }

        public async Task<Guid> AddCartAsync(CartDTO cartDTO,string userId)
        {
            var restaurant = await _catalogRepository.GetRestaurantById(cartDTO.RestaurantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant", cartDTO.RestaurantId);

            // 1. Check if an active cart already exists for this user and restaurant
            var userCarts = await _cartRepository.GetByCustomerId(userId);
            var existingCart = userCarts.FirstOrDefault(c => c.RestaurantId == cartDTO.RestaurantId && c.Status == CartStatus.Active);

            if (existingCart != null)
            {
                // If a first item is provided, add it to the existing cart if not already there
                if (cartDTO.MenuItemId.HasValue)
                {
                    var fullCart = await _cartRepository.GetById(existingCart.Id);
                    if (fullCart != null && !fullCart.CartItems.Any(ci => ci.MenuItemId == cartDTO.MenuItemId.Value))
                    {
                        var menuItem = await _catalogRepository.GetItemById(cartDTO.MenuItemId.Value);
                        if (menuItem != null)
                        {
                            fullCart.CartItems.Add(new CartItem
                            {
                                Id = Guid.NewGuid(),
                                CartId = fullCart.Id,
                                MenuItemId = menuItem.Id,
                                MenuItemName = menuItem.Name,
                                Quantity = cartDTO.Quantity ?? 1,
                                UnitPrice = menuItem.Price
                            });
                            await _cartRepository.UpdateAsync(fullCart);
                        }
                    }
                }
                return existingCart.Id;
            }

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                RestaurantId = cartDTO.RestaurantId,
                CustomerId = userId,
                Status = CartStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };

            // If a first item is provided, add it atomically
            if (cartDTO.MenuItemId.HasValue)
            {
                var menuItem = await _catalogRepository.GetItemById(cartDTO.MenuItemId.Value);
                if (menuItem != null)
                {
                    cart.CartItems.Add(new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        MenuItemId = menuItem.Id,
                        MenuItemName = menuItem.Name,
                        Quantity = cartDTO.Quantity ?? 1,
                        UnitPrice = menuItem.Price
                    });
                }
            }

            await _cartRepository.AddAsync(cart);
            return cart.Id;
        }

        public async Task<CartItemResponseDTO> AddCartItem(CartItemDTO cartItemDTO, string userId)
        {
            if (cartItemDTO.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0.");
 
            if (cartItemDTO.Quantity > 100)
                throw new BadRequestException("Quantity cannot exceed 100 items.");
 
            var menuItem = await _catalogRepository.GetItemById(cartItemDTO.MenuItemId);
            if (menuItem == null)
                throw new NotFoundException("MenuItem", cartItemDTO.MenuItemId);
 
            if (!menuItem.IsAvailable)
                throw new BadRequestException("The requested menu item is currently unavailable.");

            Cart? cart = null;
            if (cartItemDTO.CartId != Guid.Empty)
            {
                cart = await _cartRepository.GetById(cartItemDTO.CartId);
            }
            else if (cartItemDTO.RestaurantId.HasValue)
            {
                // Auto-provision cart if not provided
                var userCarts = await _cartRepository.GetByCustomerId(userId);
                cart = userCarts.FirstOrDefault(c => c.RestaurantId == cartItemDTO.RestaurantId.Value && c.Status == CartStatus.Active);
                
                if (cart == null)
                {
                    // Create new cart
                    cart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        RestaurantId = cartItemDTO.RestaurantId.Value,
                        CustomerId = userId,
                        Status = CartStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItem>()
                    };
                    await _cartRepository.AddAsync(cart);
                }
                else
                {
                    // Re-fetch with items to avoid tracking issues
                    cart = await _cartRepository.GetById(cart.Id);
                }
            }

            if (cart == null)
                throw new NotFoundException("Cart", cartItemDTO.CartId);

            // MIXED CART PREVENTION: Check if cart already has items from a different restaurant
            if (cart.CartItems.Any())
            {
                var existingRestaurantId = cart.RestaurantId;
                if (menuItem.RestaurantId != existingRestaurantId)
                {
                    throw new BadRequestException(
                        "Cannot add items from different restaurants. Please clear your cart or create a new cart for this restaurant.");
                }
            }

            if (cart.RestaurantId != menuItem.RestaurantId)
                throw new BadRequestException("Menu item does not belong to the cart's restaurant.");

            if (cart.Status != CartStatus.Active)
                throw new BadRequestException("Cannot add items to an inactive cart.");

            // CHECK IF ITEM ALREADY EXISTS IN CART
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.MenuItemId == cartItemDTO.MenuItemId);
            if (existingItem != null)
            {
                if (existingItem.Quantity + cartItemDTO.Quantity > 100)
                    throw new BadRequestException("Total quantity for this item cannot exceed 100.");

                existingItem.Quantity += cartItemDTO.Quantity;
                await _cartItemRepository.UpdateAsync(existingItem);

                return new CartItemResponseDTO
                {
                    Id = existingItem.Id,
                    MenuItemId = existingItem.MenuItemId,
                    MenuItemName = existingItem.MenuItemName,
                    CartId = existingItem.CartId,
                    Quantity = existingItem.Quantity,
                    UnitPrice = existingItem.UnitPrice
                };
            }

            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = cartItemDTO.MenuItemId,
                MenuItemName = menuItem.Name,
                CartId = cartItemDTO.CartId,
                Quantity = cartItemDTO.Quantity,
                UnitPrice = menuItem.Price
            };

            await _cartItemRepository.AddAsync(cartItem);

            return new CartItemResponseDTO
            {
                Id = cartItem.Id,
                MenuItemId = cartItem.MenuItemId,
                MenuItemName = cartItem.MenuItemName,
                CartId = cartItem.CartId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice
            };
        }

        public async Task<CartItemResponseDTO> UpdateCartItem(UpdateCartItemDTO cartItemDTO)
        {
            if (cartItemDTO.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0.");

            if (cartItemDTO.Quantity > 100)
                throw new BadRequestException("Quantity cannot exceed 100 items.");

            CartItem? cartItem = null;
            if (cartItemDTO.Id.HasValue && cartItemDTO.Id.Value != Guid.Empty)
            {
                cartItem = await _cartItemRepository.GetById(cartItemDTO.Id.Value);
            }
            else
            {
                var items = await _cartItemRepository.GetAllByCartId(cartItemDTO.CartId);
                cartItem = items.FirstOrDefault(i => i.MenuItemId == cartItemDTO.MenuItemId);
            }

            if (cartItem == null)
                throw new NotFoundException("CartItem", cartItemDTO.MenuItemId);

            cartItem.Quantity = cartItemDTO.Quantity;
            await _cartItemRepository.UpdateAsync(cartItem);

            return new CartItemResponseDTO
            {
                Id = cartItem.Id,
                MenuItemId = cartItem.MenuItemId,
                MenuItemName = cartItem.MenuItemName,
                CartId = cartItem.CartId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice
            };
        }

        public async Task<bool> DeleteCartItem(Guid id,Guid cartId,string userId)
        {
            var item = await _cartItemRepository.GetById(id);
            if (item == null)
                throw new NotFoundException("CartItem", id);
            if (item.CartId != cartId)
            {
                throw new ForbiddenException("invalid cart id item does not exist in this cart");
            }
            var cart = await _cartRepository.GetById(cartId);
            if (cart.CustomerId != userId)
            {
                throw new ForbiddenException("Access Denied this is not your cart");
            }

            await _cartItemRepository.DeleteAsync(id);

            var remaining = await _cartItemRepository.GetAllByCartId(cartId);
            if (!remaining.Any())
            {
                
                if (cart != null)
                {
                    cart.Status = CartStatus.Abandoned;
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _cartRepository.UpdateAsync(cart);
                }
            }

            return true;
        }
    }
}
