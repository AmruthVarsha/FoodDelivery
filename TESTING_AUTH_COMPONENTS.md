# Testing Authentication Components - Debug Guide

## 🔧 **FIXES APPLIED**

### **Critical Fixes:**
1. ✅ Added `provideHttpClient()` to `app.config.ts` - **This was the main issue!**
2. ✅ Added comprehensive logging to all auth components
3. ✅ Added debug reset button to login form
4. ✅ Fixed button state management with proper `isLoading` reset
5. ✅ Added extensive error message extraction
6. ✅ Added logging to AuthService and ApiService

---

## 📋 **STEP-BY-STEP TESTING GUIDE**

### **Step 1: Start Backend**
```bash
# Make sure your backend is running on http://localhost:5000
# Check that the Gateway is accessible
```

### **Step 2: Start Frontend**
```bash
cd FrontEndNew
npm start
```

### **Step 3: Open Browser Console**
1. Open your browser (Chrome/Edge recommended)
2. Navigate to `http://localhost:4200`
3. Press `F12` to open Developer Tools
4. Go to the **Console** tab
5. Clear the console (click the 🚫 icon)

### **Step 4: Test Login**
1. Enter email: `test@example.com`
2. Enter password: `Test@123`
3. Click "Sign In"

---

## 🔍 **WHAT TO LOOK FOR IN CONSOLE**

### **Expected Console Output (Successful Login):**

```
Login component initialized
Form valid: false

=== LOGIN FORM SUBMITTED ===
isLoading before: false
Form valid: true
Form value: {email: "test@example.com", password: "Test@123"}
Form errors: null
Form is VALID, proceeding with login...
isLoading set to: true
Calling authService.login with: {email: "test@example.com", password: "***"}
Subscribe called, waiting for response...

[AuthService] login() called with: {email: "test@example.com", password: "***"}
[AuthService] API endpoint: /gateway/auth/Auth/Login

[ApiService] POST request to: http://localhost:5000/gateway/auth/Auth/Login
[ApiService] Request body: {email: "test@example.com", password: "Test@123"}

[ApiService] POST response from http://localhost:5000/gateway/auth/Auth/Login : {token: "eyJ...", refreshToken: {...}}

[AuthService] Login response received: {token: "eyJ...", refreshToken: {...}}
[AuthService] Token found, handling auth success

=== LOGIN SUCCESS ===
Response: {token: "eyJ...", refreshToken: {...}}
Token received, loading profile...
Current user update: {userId: "...", fullName: "...", ...}
User profile loaded: {userId: "...", fullName: "...", ...}
isLoading set to false
Redirecting based on role: Customer
Role number: 0
Navigating to customer dashboard

=== LOGIN OBSERVABLE COMPLETED ===
```

### **Expected Console Output (Failed Login):**

```
=== LOGIN FORM SUBMITTED ===
isLoading before: false
Form valid: true
...
[ApiService] POST error from http://localhost:5000/gateway/auth/Auth/Login : HttpErrorResponse {...}

=== LOGIN ERROR ===
Error object: HttpErrorResponse {status: 401, ...}
Error status: 401
Error message: Unauthorized
isLoading set to false after error
Error message set to: Invalid credentials
```

### **Expected Console Output (Form Invalid):**

```
=== LOGIN FORM SUBMITTED ===
isLoading before: false
Form valid: false
Form is INVALID, marking fields as touched
Field email errors: {required: true}
Field password errors: {minlength: {...}}
```

---

## 🚨 **TROUBLESHOOTING**

### **Problem 1: No console logs at all**
**Symptoms:** Nothing appears in console when clicking "Sign In"

**Possible Causes:**
1. Form submission is not triggering
2. JavaScript error preventing execution
3. Angular not compiled properly

**Solutions:**
```bash
# 1. Check for compilation errors
ng build

# 2. Restart the dev server
# Press Ctrl+C to stop
npm start

# 3. Hard refresh browser
# Press Ctrl+Shift+R (Windows) or Cmd+Shift+R (Mac)

# 4. Check browser console for ANY errors (red text)
```

### **Problem 2: "Form is INVALID" message**
**Symptoms:** Console shows "Form is INVALID, marking fields as touched"

**Solutions:**
1. Check that email is valid format (contains @)
2. Check that password is at least 6 characters
3. Look at the field errors in console to see which field is invalid

### **Problem 3: API call not reaching backend**
**Symptoms:** Console shows `[ApiService] POST request to:` but no response

**Possible Causes:**
1. Backend not running
2. CORS issue
3. Wrong API URL

**Solutions:**
```bash
# 1. Check backend is running
# Open http://localhost:5000 in browser
# You should see something (not connection refused)

# 2. Check environment.ts
# File: FrontEndNew/src/environments/environment.ts
# Should have: apiUrl: 'http://localhost:5000'

# 3. Check browser Network tab
# Press F12 > Network tab
# Try login again
# Look for request to /gateway/auth/Auth/Login
# Check the status code and response
```

### **Problem 4: CORS Error**
**Symptoms:** Console shows error like "Access to XMLHttpRequest blocked by CORS policy"

**Solution:**
Your backend needs to allow CORS from `http://localhost:4200`

Check your backend CORS configuration (usually in Program.cs or Startup.cs):
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ...

app.UseCors("AllowFrontend");
```

### **Problem 5: Button stuck in "Signing in..." state**
**Symptoms:** Button shows "Signing in..." and never changes back

**Immediate Solution:**
Click the red "Reset (Debug)" button that appears below the Sign In button

**Permanent Solution:**
This means the error handler is not being called. Check:
1. Is there a JavaScript error in console?
2. Is the Observable completing?
3. Check the console logs to see where it stops

### **Problem 6: 401 Unauthorized**
**Symptoms:** Console shows `Error status: 401`

**This is EXPECTED if:**
- Email doesn't exist in database
- Password is wrong
- Account is not activated

**Solution:**
1. Make sure you're using correct credentials
2. Check backend logs to see why authentication failed
3. Try registering a new account first

### **Problem 7: Network Error / Connection Refused**
**Symptoms:** Console shows "Unable to connect to the server"

**Solution:**
```bash
# Backend is not running!
# Start your backend:
cd Gateway/FoodDelivery.Gateway
dotnet run

# Or if using Visual Studio, press F5
```

---

## 🎯 **QUICK DEBUG CHECKLIST**

Before asking for help, check these:

- [ ] Backend is running on http://localhost:5000
- [ ] Frontend is running on http://localhost:4200
- [ ] Browser console is open (F12)
- [ ] Console is cleared before testing
- [ ] No red errors in console before clicking login
- [ ] Email is valid format (contains @)
- [ ] Password is at least 6 characters
- [ ] You see "=== LOGIN FORM SUBMITTED ===" in console when clicking button
- [ ] You see "[ApiService] POST request to:" in console
- [ ] You checked Network tab for the actual HTTP request

---

## 📊 **BROWSER NETWORK TAB INSPECTION**

1. Open Developer Tools (F12)
2. Go to **Network** tab
3. Clear network log (🚫 icon)
4. Try to login
5. Look for request to `/gateway/auth/Auth/Login`

**What to check:**
- **Status Code:** Should be 200 (success) or 401 (wrong credentials)
- **Request Headers:** Should include `Content-Type: application/json`
- **Request Payload:** Should show your email and password
- **Response:** Should show token or error message

**If you don't see the request at all:**
- The form submission is not working
- Check console for JavaScript errors

---

## 🔑 **TEST CREDENTIALS**

Make sure you have a test user in your database:

```sql
-- Check if user exists
SELECT * FROM Users WHERE Email = 'test@example.com';

-- If not, register through the app or create manually
```

---

## 📝 **REPORTING ISSUES**

If it still doesn't work, provide:

1. **Full console output** (copy all text from console)
2. **Network tab screenshot** showing the failed request
3. **Backend logs** (if available)
4. **Exact steps** you took
5. **Error message** displayed on screen (if any)

---

## ✅ **SUCCESS INDICATORS**

You'll know it's working when:

1. ✅ Console shows all the expected logs
2. ✅ Button changes from "Signing in..." back to "Sign In" (on error) or redirects (on success)
3. ✅ Network tab shows 200 or 401 status code
4. ✅ Error message appears on screen (if credentials wrong)
5. ✅ You get redirected to dashboard (if credentials correct)

---

**Last Updated:** April 26, 2026
