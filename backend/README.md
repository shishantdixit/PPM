# PPM Backend API

ASP.NET Core Web API for the Petrol Pump Management System.

## 🛠️ Tech Stack

- **.NET 9.0** - Latest .NET framework
- **Entity Framework Core 9.0** - ORM for database operations
- **PostgreSQL** / **SQL Server** - Database (configurable)
- **Serilog** - Structured logging
- **Swagger** - API documentation
- **BCrypt.Net** - Password hashing

## 📁 Project Structure

```
backend/
├── PPM.API/              # Web API project
│   ├── Controllers/      # API endpoints
│   ├── Program.cs        # Application entry point
│   └── appsettings.json  # Configuration
├── PPM.Core/             # Domain entities and interfaces
│   ├── Entities/         # Domain models
│   └── Common/           # Base classes
├── PPM.Application/      # Business logic and services
│   └── Common/           # DTOs and response models
├── PPM.Infrastructure/   # Data access and external services
│   └── Data/             # DbContext and migrations
└── PPM.sln               # Solution file
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/) OR [SQL Server 2022+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker](https://www.docker.com/products/docker-desktop/) (optional)

### Option 1: Local Development (PostgreSQL)

#### 1. Install PostgreSQL

Download and install PostgreSQL from the official website.

#### 2. Create Database

```bash
psql -U postgres
CREATE DATABASE PPM;
\q
```

#### 3. Update Connection String

Edit `PPM.API/appsettings.Development.json`:

```json
{
  "DatabaseProvider": "PostgreSQL",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PPM;Username=postgres;Password=your_password"
  }
}
```

#### 4. Restore Dependencies

```bash
cd backend
dotnet restore
```

#### 5. Run Migrations

```bash
cd PPM.API
dotnet ef database update
```

Or use the migration script:

```bash
# Add migration (if needed)
dotnet ef migrations add InitialCreate --project ../PPM.Infrastructure --startup-project .

# Update database
dotnet ef database update --project ../PPM.Infrastructure --startup-project .
```

#### 6. Run the API

```bash
dotnet run
```

The API will be available at:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: https://localhost:5001 (root URL)

### Option 2: Local Development (SQL Server)

#### 1. Update Configuration

Edit `PPM.API/appsettings.Development.json`:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PPM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### 2. Run Migrations

```bash
cd PPM.API
dotnet ef database update --project ../PPM.Infrastructure --startup-project .
```

#### 3. Run the API

```bash
dotnet run
```

### Option 3: Docker Development

#### Start PostgreSQL only:

```bash
# From the root PPM directory
docker-compose up postgres -d
```

Then run the API locally:

```bash
cd backend/PPM.API
dotnet run
```

#### Start PostgreSQL + API:

```bash
docker-compose --profile backend up -d
```

#### Start SQL Server instead:

```bash
docker-compose --profile sqlserver up sqlserver -d
```

Then update `appsettings.json` to use SQL Server connection string:
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PPM;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
  }
}
```

## 📊 Database Migrations

### Create a new migration

```bash
cd PPM.API
dotnet ef migrations add MigrationName --project ../PPM.Infrastructure --startup-project .
```

### Apply migrations

```bash
dotnet ef database update --project ../PPM.Infrastructure --startup-project .
```

### Remove last migration

```bash
dotnet ef migrations remove --project ../PPM.Infrastructure --startup-project .
```

### View migration history

```bash
dotnet ef migrations list --project ../PPM.Infrastructure --startup-project .
```

## 🧪 Seed Data

The database is automatically seeded with demo data on first run:

### Super Admin
- **Username**: `superadmin`
- **Password**: `Admin@123`
- **Email**: `admin@ppmapp.com`

### Demo Tenant (DEMO001)
- **Company**: Demo Petrol Pump
- **Owner**: Rajesh Kumar

### Demo Users (Tenant: DEMO001)

#### Owner
- **Username**: `owner`
- **Password**: `Owner@123`
- **Email**: `owner@petroldemo.com`

#### Manager
- **Username**: `manager`
- **Password**: `Manager@123`
- **Email**: `manager@petroldemo.com`

#### Workers
1. **Ramesh Kumar**
   - Username: `ramesh`
   - Password: `Worker@123`

2. **Dinesh Sharma**
   - Username: `dinesh`
   - Password: `Worker@123`

### Demo Fuel Types & Rates
- **Petrol**: ₹102.50/L
- **Diesel**: ₹89.75/L
- **CNG**: ₹75.00/Kg

## 🔌 API Endpoints

### Health Check
- `GET /api/health` - Check API status
- `GET /api/health/db` - Check database connection

### Tenants (Super Admin)
- `GET /api/tenants` - Get all tenants (paginated)
- `GET /api/tenants/{id}` - Get tenant by ID
- `POST /api/tenants` - Create new tenant
- `PUT /api/tenants/{id}` - Update tenant
- `PATCH /api/tenants/{id}/status` - Activate/deactivate tenant

See [API Documentation](../docs/03-API-ENDPOINTS.md) for complete endpoint list.

## 📚 Swagger Documentation

Once the API is running, visit the root URL to access Swagger UI:

```
https://localhost:5001
```

Here you can:
- View all available endpoints
- Test API calls
- See request/response schemas
- Download OpenAPI specification

## 🔐 Authentication (Coming in Phase 2)

JWT authentication will be implemented in Phase 2. For now, the API is open for development.

## 🗄️ Switching Between Databases

### PostgreSQL to SQL Server

1. Update `appsettings.json`:
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PPM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

2. Delete existing migrations (if any):
```bash
rm -rf PPM.Infrastructure/Migrations
```

3. Create new migration:
```bash
cd PPM.API
dotnet ef migrations add InitialCreate --project ../PPM.Infrastructure --startup-project .
```

4. Update database:
```bash
dotnet ef database update --project ../PPM.Infrastructure --startup-project .
```

### SQL Server to PostgreSQL

Follow the same steps but update the connection string to PostgreSQL format.

## 🧹 Clean and Rebuild

```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run
dotnet run --project PPM.API
```

## 📝 Logging

Logs are written to:
- **Console**: Real-time logs during development
- **File**: `logs/ppm-{Date}.txt`

Log levels can be configured in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## 🐛 Troubleshooting

### Migration Error: "Build failed"

Make sure you're in the correct directory:
```bash
cd PPM.API
dotnet ef migrations add InitialCreate --project ../PPM.Infrastructure --startup-project .
```

### Database Connection Failed

**PostgreSQL**:
- Verify PostgreSQL is running: `pg_isready -U postgres`
- Check connection string in `appsettings.json`
- Verify username and password

**SQL Server**:
- Verify SQL Server is running
- Check SQL Server authentication mode
- Try using SQL Server Authentication instead of Windows Authentication

### Port Already in Use

Change the port in `Program.cs` or `launchSettings.json`:

```json
{
  "profiles": {
    "PPM.API": {
      "applicationUrl": "http://localhost:5002;https://localhost:5003"
    }
  }
}
```

### Entity Framework Tools Not Found

Install EF Core tools globally:

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

## 🚀 Next Steps

1. ✅ **Phase 1 Complete**: Foundation & Core Setup
2. 🔄 **Phase 2**: Authentication & Multi-tenancy
   - JWT authentication
   - Role-based authorization
   - Tenant resolution middleware

See [Development Roadmap](../docs/06-DEVELOPMENT-ROADMAP.md) for complete development plan.

## 📧 Support

For issues or questions, please refer to the main project README.

---

**Happy Coding! ⛽🚀**
