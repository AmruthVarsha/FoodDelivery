# QuickBites API Gateway

## Overview
API Gateway built with Ocelot for the QuickBites Food Delivery Platform. Routes requests to microservices and provides unified API access.

## Technology Stack
- **.NET 10.0**
- **Ocelot 24.1.0** - API Gateway
- **Swashbuckle 8.0.0** - Swagger/OpenAPI documentation

## Gateway Routes

### Base URL
`http://localhost:5000`

### Service Routes

#### 1. Auth Service (Port 5001)
**Gateway Route:** `/gateway/auth/{endpoint}`  
**Downstream:** `http://localhost:5001/api/{endpoint}`

**Examples:**
- `POST /gateway/auth/register` → Auth Service registration
- `POST /gateway/auth/login` → Auth Service login
- `POST /gateway/auth/logout` → Auth Service logout

#### 2. Catalog Service (Port 5002)
**Gateway Route:** `/gateway/catalog/{endpoint}`  
**Downstream:** `http://localhost:5002/api/{endpoint}`

**Examples:**
- `GET /gateway/catalog/restaurants` → Get all restaurants
- `GET /gateway/catalog/menu/{id}` → Get menu by restaurant ID

#### 3. Order Service (Port 5003)
**Gateway Route:** `/gateway/order/{endpoint}`  
**Downstream:** `http://localhost:5003/api/{endpoint}`

**Examples:**
- `POST /gateway/order/place` → Place new order
- `GET /gateway/order/history` → Get order history

#### 4. Admin Service (Port 5004)
**Gateway Route:** `/gateway/admin/{endpoint}`  
**Downstream:** `http://localhost:5004/api/{endpoint}`

**Examples:**
- `GET /gateway/admin/users` → Get all users
- `POST /gateway/admin/approve-request` → Approve role request

## Features

### CORS Configuration
- Allows requests from Angular frontend (`http://localhost:4200`)
- Allows requests from React frontend (`http://localhost:3000`)
- Supports credentials for JWT authentication

### Swagger Documentation
- Available at: `http://localhost:5000/swagger`
- Includes JWT Bearer authentication support
- Documents all gateway routes

### Health Check
- Endpoint: `GET /health`
- Returns gateway status and available routes

## Running the Gateway

### Prerequisites
All microservices must be running:
- Auth Service on port 5001
- Catalog Service on port 5002
- Order Service on port 5003
- Admin Service on port 5004

### Start Gateway
```bash
cd Gateway/FoodDelivery.Gateway
dotnet run
```

Gateway will start on: `http://localhost:5000`

### Access Swagger UI
Open browser: `http://localhost:5000/swagger`

## Configuration

### ocelot.json
Contains route configuration for all microservices:
- Upstream paths (gateway routes)
- Downstream paths (service routes)
- HTTP methods allowed
- Host and port mappings

### appsettings.json
Contains logging and environment configuration.

## Testing the Gateway

### Health Check
```bash
curl http://localhost:5000/health
```

### Test Auth Service via Gateway
```bash
# Register
curl -X POST http://localhost:5000/gateway/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "Password123",
    "phoneNumber": "1234567890",
    "role": 0
  }'

# Login
curl -X POST http://localhost:5000/gateway/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "Password123"
  }'
```

## Architecture

```
Frontend (Angular/React)
         ↓
    API Gateway (Port 5000)
         ↓
    ┌────┴────┬────────┬────────┐
    ↓         ↓        ↓        ↓
  Auth    Catalog   Order    Admin
 (5001)   (5002)   (5003)   (5004)
```

## Security

### JWT Authentication
- Gateway passes JWT tokens to downstream services
- Each service validates tokens independently
- CORS configured for frontend origins

### HTTPS (Production)
For production, configure HTTPS in `appsettings.Production.json`:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5001"
      }
    }
  }
}
```

## Troubleshooting

### Gateway not starting
- Check if port 5000 is available
- Ensure all dependencies are restored: `dotnet restore`

### Routes not working
- Verify downstream services are running
- Check ocelot.json configuration
- Review logs for routing errors

### CORS errors
- Verify frontend origin is in CORS policy
- Check browser console for specific CORS error
- Ensure credentials are included in requests

## Development

### Adding New Routes
1. Open `ocelot.json`
2. Add new route configuration:
```json
{
  "UpstreamPathTemplate": "/gateway/newservice/{everything}",
  "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
  "DownstreamPathTemplate": "/api/{everything}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "localhost",
      "Port": 5005
    }
  ]
}
```
3. Restart gateway

### Modifying CORS
Edit `Program.cs`:
```csharp
options.AddPolicy("AllowFrontend", policy =>
{
    policy.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://neworigin:port")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
});
```

## Monitoring

### Logs
Gateway logs are written to console. Configure Serilog for file logging:
```bash
dotnet add package Serilog.AspNetCore
```

### Metrics
Consider adding:
- Application Insights
- Prometheus metrics
- Health checks for downstream services

## Production Deployment

### Docker
Create `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY bin/Release/net10.0/publish/ .
ENTRYPOINT ["dotnet", "FoodDelivery.Gateway.dll"]
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT` - Set to Production
- `ASPNETCORE_URLS` - Gateway URL
- Service URLs in ocelot.json

## Support
For issues or questions, contact the development team.
