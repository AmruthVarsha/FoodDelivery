# Script to seed restaurants and menu items
# Make sure you have a Partner user account created first

$baseUrl = "http://localhost:5000"
$gatewayAuthUrl = "$baseUrl/gateway/auth"
$gatewayCatalogUrl = "$baseUrl/gateway/catalog"

# Partner credentials - UPDATE THESE WITH YOUR PARTNER ACCOUNT
$partnerEmail = "partner@test.com"
$partnerPassword = "Partner@123"

Write-Host "=== Restaurant & Menu Seeding Script ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Login as Partner
Write-Host "Step 1: Logging in as Partner..." -ForegroundColor Yellow
$loginBody = @{
    email = $partnerEmail
    password = $partnerPassword
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$gatewayAuthUrl/Auth/Login" -Method Post -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "✓ Login successful!" -ForegroundColor Green
    Write-Host "Token: $($token.Substring(0, 20))..." -ForegroundColor Gray
} catch {
    Write-Host "✗ Login failed: $_" -ForegroundColor Red
    Write-Host "Please make sure you have a Partner account with email: $partnerEmail" -ForegroundColor Yellow
    exit 1
}

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Step 2: Create Restaurants
Write-Host ""
Write-Host "Step 2: Creating restaurants..." -ForegroundColor Yellow

$restaurants = @(
    @{
        name = "Sushi Paradise"
        description = "Authentic Japanese sushi and sashimi with fresh ingredients"
        logoUrl = "https://images.unsplash.com/photo-1579584425555-c3ce17fd4351?w=400"
        phoneNumber = "1234567890"
        email = "contact@sushiparadise.com"
        openingTime = "11:00"
        closingTime = "22:00"
        prepTimeMinutes = 25
        address = @{
            street = "123 Sushi Street"
            city = "Tokyo District"
            state = "California"
            pincode = "123456"
        }
        cuisineNames = @("Japanese", "Sushi", "Asian")
        menuItems = @(
            @{
                categoryName = "Signature Rolls"
                name = "Dragon Roll"
                description = "Eel, cucumber, avocado topped with sliced avocado"
                imageUrl = "https://images.unsplash.com/photo-1579584425555-c3ce17fd4351?w=300"
                price = 14.99
                isVeg = $false
                prepTimeMinutes = 15
            },
            @{
                categoryName = "Signature Rolls"
                name = "California Roll"
                description = "Crab, avocado, cucumber with sesame seeds"
                imageUrl = "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?w=300"
                price = 9.99
                isVeg = $false
                prepTimeMinutes = 10
            },
            @{
                categoryName = "Appetizers"
                name = "Edamame"
                description = "Steamed soybeans with sea salt"
                imageUrl = "https://images.unsplash.com/photo-1583623025817-d180a2221d0a?w=300"
                price = 5.99
                isVeg = $true
                prepTimeMinutes = 5
            }
        )
    },
    @{
        name = "Bella Italia"
        description = "Traditional Italian cuisine with homemade pasta and wood-fired pizza"
        logoUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=400"
        phoneNumber = "2345678901"
        email = "info@bellaitalia.com"
        openingTime = "12:00"
        closingTime = "23:00"
        prepTimeMinutes = 30
        address = @{
            street = "456 Pasta Lane"
            city = "Little Italy"
            state = "New York"
            pincode = "234567"
        }
        cuisineNames = @("Italian", "Pizza", "Pasta")
        menuItems = @(
            @{
                categoryName = "Pizza"
                name = "Margherita Pizza"
                description = "Fresh mozzarella, tomato sauce, basil"
                imageUrl = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=300"
                price = 12.99
                isVeg = $true
                prepTimeMinutes = 20
            },
            @{
                categoryName = "Pasta"
                name = "Spaghetti Carbonara"
                description = "Creamy sauce with pancetta and parmesan"
                imageUrl = "https://images.unsplash.com/photo-1612874742237-6526221588e3?w=300"
                price = 15.99
                isVeg = $false
                prepTimeMinutes = 25
            },
            @{
                categoryName = "Desserts"
                name = "Tiramisu"
                description = "Classic Italian coffee-flavored dessert"
                imageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=300"
                price = 7.99
                isVeg = $true
                prepTimeMinutes = 5
            }
        )
    },
    @{
        name = "Burger Haven"
        description = "Gourmet burgers with premium ingredients and craft beers"
        logoUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400"
        phoneNumber = "3456789012"
        email = "hello@burgerhaven.com"
        openingTime = "10:00"
        closingTime = "22:00"
        prepTimeMinutes = 20
        address = @{
            street = "789 Burger Boulevard"
            city = "Downtown"
            state = "Texas"
            pincode = "345678"
        }
        cuisineNames = @("American", "Burgers", "Fast Food")
        menuItems = @(
            @{
                categoryName = "Burgers"
                name = "Classic Cheeseburger"
                description = "Angus beef, cheddar, lettuce, tomato, special sauce"
                imageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=300"
                price = 11.99
                isVeg = $false
                prepTimeMinutes = 15
            },
            @{
                categoryName = "Burgers"
                name = "Veggie Burger"
                description = "Plant-based patty with avocado and sprouts"
                imageUrl = "https://images.unsplash.com/photo-1520072959219-c595dc870360?w=300"
                price = 10.99
                isVeg = $true
                prepTimeMinutes = 15
            },
            @{
                categoryName = "Sides"
                name = "Truffle Fries"
                description = "Crispy fries with truffle oil and parmesan"
                imageUrl = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=300"
                price = 6.99
                isVeg = $true
                prepTimeMinutes = 10
            }
        )
    },
    @{
        name = "Green Garden"
        description = "100% plant-based vegan cuisine with organic ingredients"
        logoUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400"
        phoneNumber = "4567890123"
        email = "contact@greengarden.com"
        openingTime = "09:00"
        closingTime = "21:00"
        prepTimeMinutes = 25
        address = @{
            street = "321 Organic Avenue"
            city = "Green Valley"
            state = "Oregon"
            pincode = "456789"
        }
        cuisineNames = @("Vegan", "Healthy", "Organic")
        menuItems = @(
            @{
                categoryName = "Bowls"
                name = "Buddha Bowl"
                description = "Quinoa, roasted vegetables, tahini dressing"
                imageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=300"
                price = 13.99
                isVeg = $true
                prepTimeMinutes = 20
            },
            @{
                categoryName = "Smoothies"
                name = "Green Power Smoothie"
                description = "Spinach, banana, mango, chia seeds"
                imageUrl = "https://images.unsplash.com/photo-1505252585461-04db1eb84625?w=300"
                price = 7.99
                isVeg = $true
                prepTimeMinutes = 5
            },
            @{
                categoryName = "Main Courses"
                name = "Vegan Lasagna"
                description = "Layers of pasta with cashew ricotta and vegetables"
                imageUrl = "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=300"
                price = 14.99
                isVeg = $true
                prepTimeMinutes = 30
            }
        )
    },
    @{
        name = "Sweet Treats"
        description = "Artisan desserts, cakes, and pastries made fresh daily"
        logoUrl = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=400"
        phoneNumber = "5678901234"
        email = "orders@sweettreats.com"
        openingTime = "08:00"
        closingTime = "20:00"
        prepTimeMinutes = 15
        address = @{
            street = "654 Dessert Drive"
            city = "Sugar City"
            state = "Florida"
            pincode = "567890"
        }
        cuisineNames = @("Desserts", "Bakery", "Cafe")
        menuItems = @(
            @{
                categoryName = "Cakes"
                name = "Chocolate Lava Cake"
                description = "Warm chocolate cake with molten center"
                imageUrl = "https://images.unsplash.com/photo-1624353365286-3f8d62daad51?w=300"
                price = 8.99
                isVeg = $true
                prepTimeMinutes = 10
            },
            @{
                categoryName = "Pastries"
                name = "Croissant"
                description = "Buttery, flaky French pastry"
                imageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=300"
                price = 4.99
                isVeg = $true
                prepTimeMinutes = 5
            },
            @{
                categoryName = "Ice Cream"
                name = "Artisan Gelato"
                description = "Italian-style ice cream, various flavors"
                imageUrl = "https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=300"
                price = 6.99
                isVeg = $true
                prepTimeMinutes = 5
            }
        )
    }
)

$createdRestaurants = @()

foreach ($restaurant in $restaurants) {
    Write-Host ""
    Write-Host "Creating restaurant: $($restaurant.name)..." -ForegroundColor Cyan
    
    # Prepare restaurant data (without menu items)
    $restaurantData = @{
        name = $restaurant.name
        description = $restaurant.description
        logoUrl = $restaurant.logoUrl
        phoneNumber = $restaurant.phoneNumber
        email = $restaurant.email
        openingTime = $restaurant.openingTime
        closingTime = $restaurant.closingTime
        prepTimeMinutes = $restaurant.prepTimeMinutes
        address = $restaurant.address
        cuisineNames = $restaurant.cuisineNames
    } | ConvertTo-Json -Depth 10
    
    try {
        $createResponse = Invoke-RestMethod -Uri "$gatewayCatalogUrl/Restaurant/restaurant" -Method Post -Body $restaurantData -Headers $headers
        $restaurantId = $createResponse
        Write-Host "✓ Restaurant created with ID: $restaurantId" -ForegroundColor Green
        
        $createdRestaurants += @{
            id = $restaurantId
            name = $restaurant.name
            menuItems = $restaurant.menuItems
        }
    } catch {
        Write-Host "✗ Failed to create restaurant: $_" -ForegroundColor Red
        Write-Host "Response: $($_.Exception.Response)" -ForegroundColor Red
    }
}

# Step 3: Create Menu Items
Write-Host ""
Write-Host "Step 3: Creating menu items..." -ForegroundColor Yellow

foreach ($restaurant in $createdRestaurants) {
    Write-Host ""
    Write-Host "Adding menu items for: $($restaurant.name)" -ForegroundColor Cyan
    
    foreach ($menuItem in $restaurant.menuItems) {
        $menuItemData = @{
            restaurantId = $restaurant.id
            categoryName = $menuItem.categoryName
            name = $menuItem.name
            description = $menuItem.description
            imageUrl = $menuItem.imageUrl
            price = $menuItem.price
            isVeg = $menuItem.isVeg
            prepTimeMinutes = $menuItem.prepTimeMinutes
        } | ConvertTo-Json
        
        try {
            $menuResponse = Invoke-RestMethod -Uri "$gatewayCatalogUrl/MenuItem" -Method Post -Body $menuItemData -Headers $headers
            Write-Host "  ✓ Created: $($menuItem.name)" -ForegroundColor Green
        } catch {
            Write-Host "  ✗ Failed to create $($menuItem.name): $_" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "=== Seeding Complete! ===" -ForegroundColor Cyan
Write-Host "Created $($createdRestaurants.Count) restaurants with menu items" -ForegroundColor Green
Write-Host ""
Write-Host "You can now view them at: http://localhost:4200/customer/dashboard" -ForegroundColor Yellow
