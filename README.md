# Multi-Client Petrol Pump Management System (SaaS)

> A comprehensive, multi-tenant SaaS solution for petrol pump owners to manage billing, shifts, inventory, credit accounts, and analytics.

## 🎯 Overview

This is a production-ready **Petrol Pump Management & Billing Software** designed as a SaaS (Software as a Service) platform that serves multiple petrol pump clients with complete data isolation, role-based access control, and comprehensive fuel station operations management.

## 🏗️ Tech Stack

### Frontend
- **Framework**: Next.js 14+ (React)
- **Styling**: Tailwind CSS
- **UI Components**: Material UI / shadcn/ui
- **Charts**: Chart.js / Recharts
- **State Management**: Zustand / React Context
- **Language**: TypeScript

### Backend
- **Framework**: ASP.NET Core Web API (.NET 8+)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Architecture**: Clean Architecture / Onion Architecture
- **Background Jobs**: Hangfire
- **Validation**: FluentValidation
- **Logging**: Serilog

### Database
- **Primary**: PostgreSQL 15+
- **Multi-tenancy**: Single database with TenantId isolation
- **Migrations**: EF Core Migrations

### DevOps
- **Containerization**: Docker
- **CI/CD**: GitHub Actions
- **Hosting**: VPS / Cloud (Azure/AWS/DigitalOcean)

## ✨ Key Features

### 🔐 Multi-Tenant SaaS Architecture
- Single software instance serving multiple petrol pump clients
- Complete data isolation using TenantId
- Super Admin portal for software owner
- Per-tenant subscription management

### 💰 Billing & Sales
- Real-time fuel billing (Petrol, Diesel, CNG)
- Multiple payment modes: Cash, UPI, Card, Credit
- Auto-calculation: Quantity × Current Rate
- Thermal printer support
- Bill voiding with audit trail

### 👷 Worker & Shift Management (Core Feature)
- Shift assignment: Worker → Nozzle → Time slot
- Opening/closing meter readings
- Real-time shift tracking
- Auto reconciliation (meter vs billing)
- Shortage/excess detection
- Shift performance reports
- Worker accountability tracking

### 💳 Credit/Udhaar Management
- Customer credit profiles with limits
- Credit bill creation
- Outstanding balance tracking
- Payment recording
- Ageing analysis (0-30, 31-60, 60+ days)
- Overdue alerts

### 🛢️ Stock & Tank Management
- Tank-wise fuel inventory
- Auto stock deduction on sales
- Stock refill entry with invoice tracking
- Stock variance reports
- Low stock alerts

### 💸 Expense Management
- Daily expense tracking by category
- Cash flow monitoring
- Expense reports

### 📊 Reports & Analytics
- Daily/Monthly sales reports
- Worker performance analysis
- Machine/Nozzle-wise reports
- Payment mode breakdown
- Profit & margin analysis
- Export to PDF/Excel

### 🔒 Security & Access Control
- Role-based access: Super Admin, Owner, Manager, Worker
- JWT authentication with refresh tokens
- Row-level security (tenant isolation)
- Audit logs for critical operations
- Password hashing (BCrypt)

## 📚 Documentation

All detailed documentation is available in the [`docs/`](docs/) folder:

| Document | Description |
|----------|-------------|
| [01-SYSTEM-ARCHITECTURE.md](docs/01-SYSTEM-ARCHITECTURE.md) | High-level system architecture, multi-tenancy design, security, deployment architecture |
| [02-DATABASE-SCHEMA.md](docs/02-DATABASE-SCHEMA.md) | Complete database schema with all tables, relationships, indexes, and sample data |
| [03-API-ENDPOINTS.md](docs/03-API-ENDPOINTS.md) | RESTful API endpoints for all modules with request/response examples |
| [04-SHIFT-FLOW-DIAGRAM.md](docs/04-SHIFT-FLOW-DIAGRAM.md) | Detailed worker shift lifecycle from start to closure with reconciliation |
| [05-UI-WIREFRAMES.md](docs/05-UI-WIREFRAMES.md) | Dashboard and UI wireframes for all user roles (Super Admin, Owner, Manager, Worker) |
| [06-DEVELOPMENT-ROADMAP.md](docs/06-DEVELOPMENT-ROADMAP.md) | Phase-wise development plan from setup to production launch |

## 🚀 Quick Start

### Prerequisites
- Node.js 18+ (for Next.js frontend)
- .NET 8 SDK (for ASP.NET Core backend)
- PostgreSQL 15+
- Docker (optional, for containerized deployment)

### Backend Setup

```bash
# Navigate to backend folder
cd backend/PPM.API

# Restore packages
dotnet restore

# Update database connection string in appsettings.json
# Run migrations
dotnet ef database update

# Run the API
dotnet run
```

API will be available at: `https://localhost:5001`

### Frontend Setup

```bash
# Navigate to frontend folder
cd frontend

# Install dependencies
npm install

# Configure environment variables
cp .env.example .env.local
# Edit .env.local with your API URL

# Run development server
npm run dev
```

Frontend will be available at: `http://localhost:3000`

### Docker Setup (Recommended for Production)

```bash
# Build and run all services
docker-compose up -d

# Check logs
docker-compose logs -f
```

## 📁 Project Structure

```
PPM/
├── frontend/                 # Next.js Frontend
│   ├── src/
│   │   ├── app/             # Next.js App Router pages
│   │   ├── components/      # Reusable React components
│   │   ├── lib/             # Utilities & API client
│   │   ├── hooks/           # Custom React hooks
│   │   └── store/           # State management
│   └── public/
│
├── backend/                  # ASP.NET Core Backend
│   ├── PPM.API/             # Web API project
│   ├── PPM.Core/            # Domain entities & interfaces
│   ├── PPM.Application/     # Business logic & services
│   ├── PPM.Infrastructure/  # Data access & external services
│   └── PPM.Tests/           # Unit & integration tests
│
├── docs/                     # Documentation
│   ├── 01-SYSTEM-ARCHITECTURE.md
│   ├── 02-DATABASE-SCHEMA.md
│   ├── 03-API-ENDPOINTS.md
│   ├── 04-SHIFT-FLOW-DIAGRAM.md
│   ├── 05-UI-WIREFRAMES.md
│   └── 06-DEVELOPMENT-ROADMAP.md
│
├── docker-compose.yml        # Container orchestration
└── README.md                 # This file
```

## 👥 User Roles

### Super Admin (Software Owner)
- Manage all tenants (petrol pumps)
- Create/disable pump accounts
- View global analytics
- Subscription management

### Pump Owner
- Full access to pump data
- Configure machines, nozzles, workers
- View all reports and analytics
- Financial management

### Manager
- Shift management and assignment
- Worker supervision
- Billing oversight
- Credit approval
- Daily operations

### Worker/Operator
- Start/end shift
- Create bills
- Record meter readings
- Handle customer transactions
- Limited credit issuance

## 🔑 Key Workflows

### 1. Shift Management Flow
```
Manager assigns worker → Worker starts shift → Records opening meter
→ Creates bills during shift → Records closing meter
→ Manager reviews reconciliation → Approves & closes shift
→ System generates shift report
```

### 2. Billing Flow
```
Customer arrives → Worker records meter before → Dispenses fuel
→ Records meter after → System calculates quantity & amount
→ Customer pays (Cash/UPI/Card/Credit) → Bill generated & printed
→ Shift totals updated → Stock deducted
```

### 3. Credit Management Flow
```
Credit customer arrives → Worker selects customer
→ System checks credit limit → Creates credit bill
→ Updates customer balance → Records transaction
→ Links to shift accountability
```

## 📊 Core Modules

1. **Authentication & Authorization** - JWT-based auth with role management
2. **Tenant Management** - Multi-client SaaS infrastructure
3. **Machine & Nozzle Setup** - Fuel dispenser configuration
4. **Fuel Rate Management** - Dynamic pricing with history
5. **User Management** - Owner, Manager, Worker profiles
6. **Shift Management** - Complete shift lifecycle with reconciliation
7. **Billing & Sales** - Real-time billing with multiple payment modes
8. **Credit/Udhaar** - Customer credit accounts and tracking
9. **Stock Management** - Tank inventory with auto-deduction
10. **Expense Tracking** - Daily expense management
11. **Reports & Analytics** - Comprehensive reporting suite

## 🛠️ Development Phases

| Phase | Focus Area | Status |
|-------|-----------|--------|
| Phase 1 | Foundation & Core Setup | ✅ Complete |
| Phase 2 | Authentication & Multi-tenancy | ✅ Complete |
| Phase 3 | Machine, Nozzle & Fuel Management | ✅ Complete |
| Phase 4 | Shift Management (Critical) | ✅ Complete |
| Phase 5 | Billing & Sales | ✅ Complete |
| Phase 6 | Credit/Udhaar Management | ✅ Complete |
| Phase 7 | Stock & Tank Management | ✅ Complete |
| Phase 8 | Expenses & Reports | ✅ Complete |
| Phase 9 | Testing & Bug Fixes | ✅ Complete |
| Phase 10 | Deployment & Production Launch | ⏳ Pending |
| Phase 12 | Feature-Based Subscription Management | ✅ Complete |

See [06-DEVELOPMENT-ROADMAP.md](docs/06-DEVELOPMENT-ROADMAP.md) for detailed phase breakdown.

## 🔐 Security Features

- ✅ HTTPS only
- ✅ JWT token authentication with refresh
- ✅ Password hashing (BCrypt/PBKDF2)
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ XSS prevention (input sanitization)
- ✅ CORS configuration
- ✅ Rate limiting
- ✅ Multi-tenant data isolation
- ✅ Audit logging for critical operations
- ✅ Role-based authorization

## 📈 Scalability

### Current Design (Phase 1)
- Single database with TenantId isolation
- Supports 50-100 concurrent tenants
- Connection pooling
- Indexed queries

### Future Scaling Options
- Database-per-tenant for large clients
- Read replicas for reporting
- Redis caching layer
- Horizontal API scaling with load balancer
- Kubernetes orchestration

## 🎯 Success Metrics

### Performance Targets
- Page load: < 2 seconds
- API response: < 500ms
- Database queries: < 100ms
- 99.9% uptime

### Data Integrity (Zero Tolerance)
- No lost bills
- Accurate shift calculations
- Precise stock tracking
- Correct credit balances

## 🚀 Deployment

### Development
```bash
docker-compose up -d
```

### Production
- Docker containers on VPS/Cloud
- PostgreSQL managed database
- SSL/TLS certificates
- Automated backups
- Monitoring and alerts

See deployment guide in [06-DEVELOPMENT-ROADMAP.md](docs/06-DEVELOPMENT-ROADMAP.md#phase-10-deployment--production-launch)

## 🔄 Future Enhancements

- 📱 Native mobile apps (React Native)
- 🔌 IoT integration with fuel dispensers
- 📲 WhatsApp/SMS notifications
- 🤖 AI-powered sales forecasting
- 🔍 Anomaly detection for fraud prevention
- 🌐 Multi-language support
- 💼 Accounting software integration (Tally, QuickBooks)
- 🎁 Customer loyalty program

## 📞 Support & Contact

For questions, issues, or feature requests:
- Create an issue in the repository
- Contact: [Your contact information]
- Documentation: See `docs/` folder

## 📄 License

[Specify your license here]

---

**Built with ❤️ for the petrol pump industry**

*Empowering fuel station owners with technology*
