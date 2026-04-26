# Script to create a Partner user account

$baseUrl = "http://localhost:5000"
$gatewayAuthUrl = "$baseUrl/gateway/auth"

Write-Host "=== Creating Partner User ===" -ForegroundColor Cyan
Write-Host ""

# Partner user details
$partnerData = @{
    fullName = "Test Partner"
    email = "partner@test.com"
    password = "Partner@123"
    confirmPassword = "Partner@123"
    phoneNo = "9876543210"
    role = 1  # Partner role
} | ConvertTo-Json

Write-Host "Registering partner account..." -ForegroundColor Yellow
Write-Host "Email: partner@test.com" -ForegroundColor Gray
Write-Host "Password: Partner@123" -ForegroundColor Gray
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri "$gatewayAuthUrl/Auth/Register" -Method Post -Body $partnerData -ContentType "application/json"
    Write-Host "Partner account created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Account Details:" -ForegroundColor Cyan
    Write-Host "  Email: partner@test.com" -ForegroundColor White
    Write-Host "  Password: Partner@123" -ForegroundColor White
    Write-Host "  Role: Partner (1)" -ForegroundColor White
    Write-Host ""
    Write-Host "Note: If the account requires approval, please approve it using an Admin account." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Next step: Run .\seed-restaurants.ps1 to add restaurants and menu items" -ForegroundColor Cyan
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    $errorBody = $_.ErrorDetails.Message
    
    if ($statusCode -eq 400 -and $errorBody -like "*already exists*") {
        Write-Host "Partner account already exists!" -ForegroundColor Green
        Write-Host ""
        Write-Host "You can proceed to run: .\seed-restaurants.ps1" -ForegroundColor Cyan
    } else {
        Write-Host "Failed to create partner account" -ForegroundColor Red
        Write-Host "Status Code: $statusCode" -ForegroundColor Red
        Write-Host "Error: $errorBody" -ForegroundColor Red
    }
}
