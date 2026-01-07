# System Architecture - Multi-Tenant Petrol Pump Management SaaS

## 🏗️ High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│  Next.js Frontend (React)                                        │
│  - Super Admin Portal (Software Owner)                           │
│  - Pump Owner Dashboard                                          │
│  - Manager Portal                                                │
│  - Worker/Operator Interface                                     │
│  - Tailwind CSS / Material UI                                    │
│  - Chart.js / Recharts                                           │
└─────────────────┬───────────────────────────────────────────────┘
                  │ HTTPS / REST API
                  │ JWT Bearer Token
┌─────────────────▼───────────────────────────────────────────────┐
│                    API GATEWAY / LOAD BALANCER                   │
│                    (Nginx / Azure API Management)                │
└─────────────────┬───────────────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────────────┐
│                   APPLICATION LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│  ASP.NET Core Web API (.NET 8+)                                  │
│                                                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  Authentication & Authorization Middleware                  │ │
│  │  - JWT Token Validation                                     │ │
│  │  - Role-based Access Control (RBAC)                         │ │
│  │  - Tenant Isolation Middleware                              │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  API Controllers (RESTful)                                  │ │
│  │  - Tenant Management                                        │ │
│  │  - Billing & Sales                                          │ │
│  │  - Machine & Nozzle                                         │ │
│  │  - Worker & Shift                                           │ │
│  │  - Credit/Udhaar Management                                 │ │
│  │  - Stock & Tank                                             │ │
│  │  - Expenses & Reports                                       │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  Business Logic Layer (Services)                            │ │
│  │  - Billing Service                                          │ │
│  │  - Shift Management Service                                 │ │
│  │  - Credit Management Service                                │ │
│  │  - Stock Management Service                                 │ │
│  │  - Report Generation Service                                │ │
│  │  - Notification Service                                     │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  Data Access Layer                                          │ │
│  │  - Entity Framework Core                                    │ │
│  │  - Repository Pattern                                       │ │
│  │  - Unit of Work Pattern                                     │ │
│  │  - CQRS (optional for complex queries)                      │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  Background Jobs                                            │ │
│  │  - Hangfire / Quartz.NET                                    │ │
│  │  - Daily reconciliation jobs                                │ │
│  │  - Report generation                                        │ │
│  │  - Low stock alerts                                         │ │
│  │  - Subscription expiry notifications                        │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────┬───────────────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────────────┐
│                      DATABASE LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│  PostgreSQL (Primary)                                            │
│  - Multi-tenant Database (Single DB, TenantId isolation)         │
│  - Row-Level Security (RLS)                                      │
│  - Connection Pooling                                            │
│  - Read Replicas (for scaling)                                   │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    EXTERNAL INTEGRATIONS                         │
├─────────────────────────────────────────────────────────────────┤
│  - SMS Gateway (OTP, Notifications)                              │
│  - Email Service (Reports, Alerts)                               │
│  - Payment Gateway (Future)                                      │
│  - WhatsApp Business API (Future)                                │
│  - Thermal Printer API                                           │
└─────────────────────────────────────────────────────────────────┘
```

## 🏢 Multi-Tenancy Architecture

### Tenant Isolation Strategy

**Approach: Discriminator Column (TenantId)**

All tenant-specific tables include a `TenantId` column for data isolation.

```
┌─────────────────────────────────────────────────────────────────┐
│                     TENANT ISOLATION                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐        │
│  │ Tenant 1 │  │ Tenant 2 │  │ Tenant 3 │  │ Tenant N │        │
│  │ (Pump A) │  │ (Pump B) │  │ (Pump C) │  │ (Pump N) │        │
│  └─────┬────┘  └─────┬────┘  └─────┬────┘  └─────┬────┘        │
│        │             │             │             │               │
│        └─────────────┴─────────────┴─────────────┘               │
│                            │                                     │
│                            ▼                                     │
│        ┌─────────────────────────────────────────┐              │
│        │   Single PostgreSQL Database            │              │
│        │   - All tables have TenantId column     │              │
│        │   - Filtered queries at middleware      │              │
│        │   - Global query filters in EF Core     │              │
│        └─────────────────────────────────────────┘              │
│                                                                   │
│  Benefits:                                                        │
│  ✓ Cost-effective                                                │
│  ✓ Easy maintenance                                              │
│  ✓ Simple backup & restore                                       │
│  ✓ Easy to migrate to separate DBs later                         │
└─────────────────────────────────────────────────────────────────┘
```

### Tenant Context Resolution

```
User Request → JWT Token → Extract TenantId → Set in HttpContext
→ Apply to all queries via EF Core Global Query Filters
```

## 🔐 Security Architecture

### Authentication Flow

```
┌──────────┐                                  ┌──────────────┐
│  Client  │                                  │  API Server  │
└─────┬────┘                                  └──────┬───────┘
      │                                              │
      │  1. POST /api/auth/login                    │
      │     { username, password, tenantCode }      │
      ├────────────────────────────────────────────>│
      │                                              │
      │                              2. Validate credentials
      │                                 & tenant     │
      │                                              │
      │  3. Return JWT Token                        │
      │     { token, refreshToken, user, roles }    │
      │<────────────────────────────────────────────┤
      │                                              │
      │  4. Subsequent requests with                │
      │     Authorization: Bearer {token}           │
      ├────────────────────────────────────────────>│
      │                                              │
      │                              5. Validate JWT │
      │                                 Extract TenantId
      │                                 Apply filters│
      │                                              │
      │  6. Response with filtered data             │
      │<────────────────────────────────────────────┤
      │                                              │
```

### Role-Based Access Control (RBAC)

```
SuperAdmin (Software Owner)
├── Manage all tenants
├── Create/disable pump accounts
├── View all analytics
├── Subscription management
└── System configuration

Tenant Roles:
├── Owner (Pump Owner)
│   ├── Full access to pump data
│   ├── Add/remove managers & workers
│   ├── Configure machines & nozzles
│   ├── View all reports
│   └── Financial reports
│
├── Manager
│   ├── Shift management
│   ├── Worker assignment
│   ├── Billing & sales
│   ├── Credit approval
│   ├── Stock management
│   └── Daily reports
│
└── Worker/Operator
    ├── Start/end shift
    ├── Record meter readings
    ├── Create bills
    ├── View assigned nozzle data
    └── Limited credit issuance
```

## 🔄 Worker Shift Management Flow

```
┌────────────────────────────────────────────────────────────────┐
│                    SHIFT LIFECYCLE                              │
└────────────────────────────────────────────────────────────────┘

1. SHIFT START
   ├── Worker login
   ├── Manager assigns worker to nozzle
   ├── Record opening meter reading
   ├── Record opening cash in hand
   └── Shift status: ACTIVE

2. DURING SHIFT
   ├── Create bills (fuel sales)
   ├── Record payments (cash/digital/credit)
   ├── Handle credit transactions
   └── Track all activities

3. SHIFT END
   ├── Record closing meter reading
   ├── Calculate fuel sold (closing - opening)
   ├── Calculate expected collection
   │   └── (Fuel sold × Rate) - Credit given
   ├── Record actual cash/digital received
   ├── Calculate variance (shortage/excess)
   ├── Manager approval required
   └── Shift status: CLOSED

4. RECONCILIATION
   ├── Compare meter vs billing
   ├── Identify discrepancies
   ├── Generate shift report
   └── Update worker accountability
```

## 📊 Data Flow Architecture

### Billing Transaction Flow

```
Customer Request → Worker Login → Nozzle Selection →
Fuel Type & Quantity → Auto Calculate (Qty × Rate) →
Payment Mode (Cash/UPI/Credit) → Generate Invoice →
Update Stock → Update Shift Data → Print Receipt
```

### Stock Management Flow

```
New Stock Received → Record Invoice → Update Tank →
Auto-deduct on sales → Calculate variance →
Generate stock report → Low stock alert
```

## 🚀 Deployment Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRODUCTION DEPLOYMENT                         │
└─────────────────────────────────────────────────────────────────┘

Docker Containerized:
├── Frontend Container (Next.js)
│   └── Nginx reverse proxy
├── Backend Container (ASP.NET Core API)
│   └── Multiple instances for load balancing
├── PostgreSQL Container / Managed DB
└── Redis (Caching & Session - optional)

Deployment Options:
├── VPS (DigitalOcean, Linode, AWS EC2)
├── Azure App Service + Azure SQL
├── AWS ECS/EKS + RDS
└── Docker Swarm / Kubernetes (for scaling)

CI/CD Pipeline:
Git Push → GitHub Actions → Build → Test →
Docker Build → Deploy to Production
```

## 🔧 Technology Stack Details

### Frontend (Next.js)
- **Framework**: Next.js 14+ (App Router)
- **UI Library**: Tailwind CSS + shadcn/ui or Material UI
- **State Management**: Zustand / React Context
- **Charts**: Chart.js / Recharts
- **Forms**: React Hook Form + Zod validation
- **API Client**: Axios / Fetch with interceptors
- **Authentication**: NextAuth.js (optional) or custom JWT

### Backend (ASP.NET Core)
- **.NET Version**: .NET 8+
- **Architecture**: Clean Architecture / Onion Architecture
- **ORM**: Entity Framework Core 8+
- **Authentication**: JWT Bearer tokens
- **Authorization**: Policy-based authorization
- **Validation**: FluentValidation
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI
- **Background Jobs**: Hangfire
- **Caching**: IMemoryCache / Redis (optional)

### Database (PostgreSQL)
- **Version**: PostgreSQL 15+
- **Migrations**: EF Core Migrations
- **Indexing**: Strategic indexes on TenantId, foreign keys
- **Backup**: Automated daily backups
- **Monitoring**: pg_stat_statements

## 📈 Scalability Considerations

### Horizontal Scaling
- Stateless API design
- Load balancer (Nginx/HAProxy)
- Multiple API instances
- Session storage in Redis (if needed)

### Database Scaling
- Read replicas for reporting
- Connection pooling
- Query optimization
- Partitioning by TenantId (future)

### Future: Migrate to Database-per-Tenant
When scale demands:
- Separate database per large tenant
- Tenant catalog table for routing
- Same codebase, different connection strings

## 🔒 Security Best Practices

1. **API Security**
   - JWT with short expiry (15-30 mins)
   - Refresh tokens (7-30 days)
   - HTTPS only
   - CORS configuration
   - Rate limiting

2. **Data Security**
   - Tenant isolation at middleware
   - Encrypted sensitive data
   - Audit logs for critical operations
   - Regular security audits

3. **SQL Injection Prevention**
   - Parameterized queries (EF Core)
   - Input validation
   - Output encoding

4. **Authentication Security**
   - Password hashing (BCrypt/PBKDF2)
   - OTP-based login (optional)
   - Failed login attempts tracking
   - Account lockout mechanism

## 📦 Project Structure

```
PPM/
├── frontend/                    # Next.js Frontend
│   ├── src/
│   │   ├── app/                # App router pages
│   │   ├── components/         # Reusable components
│   │   ├── lib/                # Utilities & API client
│   │   ├── hooks/              # Custom React hooks
│   │   └── store/              # State management
│   └── public/
│
├── backend/                     # ASP.NET Core Backend
│   ├── PPM.API/                # Web API project
│   ├── PPM.Core/               # Domain entities & interfaces
│   ├── PPM.Application/        # Business logic & services
│   ├── PPM.Infrastructure/     # Data access & external services
│   └── PPM.Tests/              # Unit & integration tests
│
├── database/
│   ├── migrations/             # SQL migration scripts
│   └── seeds/                  # Seed data
│
├── docs/                        # Documentation
└── docker-compose.yml          # Container orchestration
```

## 🎯 Key Architectural Decisions

1. **Multi-tenancy via TenantId**: Simpler to start, can migrate to separate DBs later
2. **API-first design**: Enables future mobile apps easily
3. **Clean Architecture**: Separation of concerns, testable code
4. **JWT Authentication**: Stateless, scalable
5. **PostgreSQL**: Open-source, cost-effective, powerful
6. **Docker**: Consistent deployment, easy scaling
7. **Background jobs**: Automated tasks don't block API requests

---

**Next Steps**: Proceed to Database Schema Design
