# Script to check if all required services are running

Write-Host "=== Checking Backend Services ===" -ForegroundColor Cyan
Write-Host ""

$services = @(
    @{ Name = "Gateway"; Url = "http://localhost:5000"; Port = 5000 },
    @{ Name = "AuthService"; Url = "http://localhost:5001"; Port = 5001 },
    @{ Name = "CatalogService"; Url = "http://localhost:5002"; Port = 5002 }
)

$allRunning = $true

foreach ($service in $services) {
    Write-Host "Checking $($service.Name) on port $($service.Port)..." -NoNewline
    
    try {
        $response = Invoke-WebRequest -Uri $service.Url -Method Get -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
        Write-Host " Running" -ForegroundColor Green
    } catch {
        if ($_.Exception.Message -like "*404*") {
            Write-Host " Running (404 is OK - service is up)" -ForegroundColor Green
        } else {
            Write-Host " Not responding" -ForegroundColor Red
            $allRunning = $false
        }
    }
}

Write-Host ""

if ($allRunning) {
    Write-Host "All services are running!" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run:" -ForegroundColor Cyan
    Write-Host "  1. .\create-partner-user.ps1" -ForegroundColor White
    Write-Host "  2. .\seed-restaurants.ps1" -ForegroundColor White
} else {
    Write-Host "Some services are not running!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please start all backend services before running the seed scripts." -ForegroundColor Yellow
}

Write-Host ""
