# Quick Start - Seed Restaurant Data

## 🚀 Three Simple Steps

### 1️⃣ Check Services
```powershell
.\check-services.ps1
```
Verifies that Gateway, AuthService, and CatalogService are running.

### 2️⃣ Create Partner Account
```powershell
.\create-partner-user.ps1
```
Creates a Partner user account (partner@test.com / Partner@123).

### 3️⃣ Seed Data
```powershell
.\seed-restaurants.ps1
```
Creates 5 restaurants with 15 menu items (3 items per restaurant).

## 📋 What You'll Get

### 5 Restaurants:
1. **Sushi Paradise** 🍣 - Japanese/Sushi cuisine
2. **Bella Italia** 🍕 - Italian/Pizza/Pasta
3. **Burger Haven** 🍔 - American/Burgers
4. **Green Garden** 🥗 - Vegan/Healthy
5. **Sweet Treats** 🍰 - Desserts/Bakery

### Each Restaurant Has:
- ✅ Professional food images (from Unsplash)
- ✅ 3 menu items with images
- ✅ Realistic prices and descriptions
- ✅ Operating hours and prep times
- ✅ Multiple cuisine tags

## 🎯 View Results

After seeding, open your browser:
```
http://localhost:4200/customer/dashboard
```

You should see all restaurants displayed with images, ratings, and details!

## ⚠️ Important Notes

- **Partner Approval:** If your backend requires admin approval for Partner accounts, you'll need to approve the partner@test.com account before running the seed script.
- **Services Must Be Running:** All three services (Gateway, Auth, Catalog) must be running on ports 5000, 5001, and 5002.
- **Run Once:** The seed script should only be run once. Running it multiple times may create duplicate data.

## 🔧 Troubleshooting

**Services not running?**
- Start your backend services
- Run `.\check-services.ps1` to verify

**Login failed?**
- Check if Partner account needs admin approval
- Verify credentials in seed-restaurants.ps1

**Need more details?**
- See `SEED_DATA_README.md` for comprehensive documentation

---

Happy coding! 🎉
