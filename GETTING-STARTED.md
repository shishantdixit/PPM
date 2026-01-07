# 🚀 Getting Started with PPM Development

This guide will help you set up and run the Petrol Pump Management System for the first time.

## ✅ What's Been Created

### Phase 1: Foundation & Core Setup ✅ COMPLETE

The following has been implemented:

#### Backend (ASP.NET Core .NET 9)
- ✅ Clean Architecture structure (Core, Application, Infrastructure, API)
- ✅ Entity Framework Core with support for **both PostgreSQL and SQL Server**
- ✅ Domain entities (Tenant, SystemUser, User, FuelType, FuelRate)
- ✅ DbContext with comprehensive seed data
- ✅ API Controllers (Health Check, Tenants Management)
- ✅ Swagger/OpenAPI documentation
- ✅ Structured logging with Serilog
- ✅ Docker support

#### Database Features
- ✅ Multi-tenant architecture (TenantId isolation)
- ✅ Seed data with demo tenant and users
- ✅ Support for both PostgreSQL and SQL Server
- ✅ Auto-migration on startup

#### Documentation
- ✅ Complete system architecture
- ✅ Database schema design
- ✅ API endpoints documentation
- ✅ Shift management flow
- ✅ UI wireframes
- ✅ Development roadmap

---

## 🏃 Quick Start (Recommended)

### For Windows Users

1. **Open Command Prompt** in the `backend` folder
2. Run the quick start script:
   ```cmd
   quick-start.cmd
   ```
3. Follow the on-screen instructions
4. Once complete, start the API:
   ```cmd
   cd PPM.API
   dotnet run
   ```

### For Mac/Linux Users

1. **Open Terminal** in the `backend` folder
2. Make the script executable:
   ```bash
   chmod +x quick-start.sh
   ```
3. Run the script:
   ```bash
   ./quick-start.sh
   ```
4. Once complete, start the API:
   ```bash
   cd PPM.API
   dotnet run
   ```

---

## 📋 Manual Setup

If you prefer to set up manually or the quick start script fails:

### Prerequisites

1. **Install .NET 9 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verify installation: `dotnet --version`

2. **Choose Your Database**

   **Option A: PostgreSQL (Recommended)**
   - Download: https://www.postgresql.org/download/
   - Default credentials: username=`postgres`, password=`postgres`
   - Create database: `CREATE DATABASE PPM;`

   **Option B: SQL Server**
   - Download: https://www.microsoft.com/sql-server/sql-server-downloads
   - Use Windows Authentication or create SA password

3. **Install Entity Framework Tools**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

### Step-by-Step Setup

#### 1. Configure Database Connection

Navigate to `backend/PPM.API/appsettings.Development.json`

**For PostgreSQL:**
```json
{
  "DatabaseProvider": "PostgreSQL",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PPM;Username=postgres;Password=your_password"
  }
}
```

**For SQL Server:**
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PPM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### 2. Restore NuGet Packages

```bash
cd backend
dotnet restore
```

#### 3. Build the Solution

```bash
dotnet build
```

#### 4. Create Database Migration

```bash
cd PPM.API
dotnet ef migrations add InitialCreate --project ../PPM.Infrastructure --startup-project .
```

#### 5. Apply Migration (Create Database)

```bash
dotnet ef database update --project ../PPM.Infrastructure --startup-project .
```

This will:
- Create the database
- Create all tables
- Seed demo data

#### 6. Run the API

```bash
dotnet run
```

The API will start at:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: https://localhost:5001

---

## 🐳 Docker Setup (Alternative)

If you prefer using Docker:

### Start PostgreSQL Only

```bash
# From the root PPM directory
docker-compose up postgres -d
```

Then run the API locally:
```bash
cd backend/PPM.API
dotnet run
```

### Start PostgreSQL + API

```bash
docker-compose --profile backend up -d
```

### Start SQL Server

```bash
docker-compose --profile sqlserver up sqlserver -d
```

---

## 🧪 Test the API

### Using Browser (Swagger UI)

1. Open https://localhost:5001 in your browser
2. You'll see the Swagger UI with all available endpoints
3. Click on an endpoint to test it

### Test Health Check

**Using Swagger:**
1. Expand `GET /api/health`
2. Click "Try it out"
3. Click "Execute"

**Using cURL:**
```bash
curl -k https://localhost:5001/api/health
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "status": "healthy",
    "timestamp": "2024-01-06T...",
    "version": "1.0.0"
  }
}
```

### Test Database Connection

**Using Swagger:**
1. Expand `GET /api/health/db`
2. Click "Try it out"
3. Click "Execute"

**Expected Response:**
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "database": "connected",
    "provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
    "tenantCount": 1
  }
}
```

### Get All Tenants

**Using Swagger:**
1. Expand `GET /api/tenants`
2. Click "Try it out"
3. Click "Execute"

You should see the demo tenant (DEMO001) in the response.

---

## 🔑 Demo Credentials

The database is seeded with the following demo accounts:

### Super Admin (Platform Owner)
- **Username**: `superadmin`
- **Password**: `Admin@123`
- **Email**: `admin@ppmapp.com`
- **Access**: Manage all tenants

### Demo Tenant: DEMO001

#### Owner
- **Username**: `owner`
- **Password**: `Owner@123`
- **Email**: `owner@petroldemo.com`
- **Access**: Full access to Demo Petrol Pump data

#### Manager
- **Username**: `manager`
- **Password**: `Manager@123`
- **Email**: `manager@petroldemo.com`
- **Access**: Shift management, billing, operations

#### Workers
1. **Ramesh Kumar**
   - Username: `ramesh`
   - Password: `Worker@123`
   - Employee Code: EMP001

2. **Dinesh Sharma**
   - Username: `dinesh`
   - Password: `Worker@123`
   - Employee Code: EMP002

### Demo Fuel Types
- **Petrol**: ₹102.50/L
- **Diesel**: ₹89.75/L
- **CNG**: ₹75.00/Kg

> **Note**: Authentication is not yet implemented (Phase 2). These credentials are for reference when authentication is added.

---

## 📊 Verify Seed Data

You can verify the seed data using a database client:

### PostgreSQL
```bash
psql -U postgres -d PPM

-- View tenants
SELECT * FROM "Tenants";

-- View users
SELECT * FROM "Users";

-- View fuel types
SELECT * FROM "FuelTypes";

-- View fuel rates
SELECT * FROM "FuelRates";
```

### SQL Server
```sql
USE PPM;

-- View tenants
SELECT * FROM Tenants;

-- View users
SELECT * FROM Users;

-- View fuel types
SELECT * FROM FuelTypes;

-- View fuel rates
SELECT * FROM FuelRates;
```

---

## 🔧 Troubleshooting

### "dotnet command not found"
- Install .NET 9 SDK from https://dotnet.microsoft.com/download/dotnet/9.0
- Restart your terminal after installation

### "Build failed" Error
- Make sure you're in the `backend` directory
- Run `dotnet restore` first
- Check that all .csproj files are present

### "Database connection failed"
**For PostgreSQL:**
- Verify PostgreSQL is running: `pg_isready -U postgres`
- Check username and password in connection string
- Try: `psql -U postgres` to test connection

**For SQL Server:**
- Verify SQL Server is running
- Check Windows Services for SQL Server
- Try connecting with SQL Server Management Studio (SSMS)

### Migration Error
```bash
# Delete existing migrations
rm -rf PPM.Infrastructure/Migrations

# Create new migration
cd PPM.API
dotnet ef migrations add InitialCreate --project ../PPM.Infrastructure --startup-project .

# Apply migration
dotnet ef database update --project ../PPM.Infrastructure --startup-project .
```

### Port Already in Use
If port 5000 or 5001 is already in use, you can change it:

Edit `PPM.API/Properties/launchSettings.json`:
```json
{
  "profiles": {
    "PPM.API": {
      "applicationUrl": "http://localhost:5002;https://localhost:5003"
    }
  }
}
```

---

## 📁 Project Structure

```
PPM/
├── backend/
│   ├── PPM.API/              # Web API (controllers, startup)
│   ├── PPM.Core/             # Domain entities
│   ├── PPM.Application/      # Business logic
│   ├── PPM.Infrastructure/   # Data access (EF Core)
│   ├── quick-start.cmd       # Windows setup script
│   ├── quick-start.sh        # Mac/Linux setup script
│   └── README.md             # Backend documentation
├── docs/
│   ├── 01-SYSTEM-ARCHITECTURE.md
│   ├── 02-DATABASE-SCHEMA.md
│   ├── 03-API-ENDPOINTS.md
│   ├── 04-SHIFT-FLOW-DIAGRAM.md
│   ├── 05-UI-WIREFRAMES.md
│   └── 06-DEVELOPMENT-ROADMAP.md
├── docker-compose.yml        # Docker setup
├── README.md                 # Main project README
└── GETTING-STARTED.md        # This file
```

---

## 🎯 What's Next?

### Phase 2: Authentication & Multi-tenancy

The next phase will implement:
- JWT authentication
- Login/logout endpoints
- Role-based authorization middleware
- Tenant resolution from JWT tokens
- Password hashing with BCrypt
- Refresh tokens

See [Development Roadmap](docs/06-DEVELOPMENT-ROADMAP.md#phase-2-authentication--multi-tenancy) for details.

### Current API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/health` | GET | Check API health |
| `/api/health/db` | GET | Check database connection |
| `/api/tenants` | GET | Get all tenants (paginated) |
| `/api/tenants/{id}` | GET | Get tenant by ID |
| `/api/tenants` | POST | Create new tenant |
| `/api/tenants/{id}` | PUT | Update tenant |
| `/api/tenants/{id}/status` | PATCH | Activate/deactivate tenant |

More endpoints will be added in subsequent phases.

---

## 📚 Additional Resources

- **System Architecture**: [docs/01-SYSTEM-ARCHITECTURE.md](docs/01-SYSTEM-ARCHITECTURE.md)
- **Database Schema**: [docs/02-DATABASE-SCHEMA.md](docs/02-DATABASE-SCHEMA.md)
- **API Documentation**: [docs/03-API-ENDPOINTS.md](docs/03-API-ENDPOINTS.md)
- **Development Roadmap**: [docs/06-DEVELOPMENT-ROADMAP.md](docs/06-DEVELOPMENT-ROADMAP.md)
- **Backend README**: [backend/README.md](backend/README.md)

---

## 💡 Tips

1. **Use Swagger UI**: It's the easiest way to test the API during development
2. **Check Logs**: Logs are in `backend/PPM.API/logs/` - helpful for debugging
3. **Database GUI Tools**:
   - PostgreSQL: pgAdmin, DBeaver
   - SQL Server: SQL Server Management Studio (SSMS), Azure Data Studio
4. **API Testing**: Use Postman, Insomnia, or Thunder Client (VS Code extension)

---

## 🎉 Success Checklist

- [ ] .NET 9 SDK installed and working
- [ ] Database (PostgreSQL or SQL Server) running
- [ ] Backend solution builds successfully
- [ ] Database created with migrations
- [ ] Seed data loaded (1 tenant, 4 users, 3 fuel types)
- [ ] API running on https://localhost:5001
- [ ] Swagger UI accessible
- [ ] Health check endpoint returns success
- [ ] Tenants endpoint returns demo tenant

If all checkboxes are ticked, you're ready to start development! 🚀

---

**Need Help?** Check the [Troubleshooting](#-troubleshooting) section or refer to [backend/README.md](backend/README.md) for detailed instructions.
