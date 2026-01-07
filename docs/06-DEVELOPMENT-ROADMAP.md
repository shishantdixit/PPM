# Development Roadmap - Petrol Pump Management System

## 🎯 Project Overview

**Goal**: Build a production-ready multi-tenant SaaS Petrol Pump Management System

**Tech Stack**:
- Frontend: Next.js + Tailwind CSS
- Backend: ASP.NET Core Web API
- Database: PostgreSQL
- Deployment: Docker containers

---

## 📅 Development Phases

```
Phase 1: Foundation & Core Setup
Phase 2: Authentication & Multi-tenancy
Phase 3: Machine, Nozzle & Fuel Management
Phase 4: Shift Management (CRITICAL)
Phase 5: Billing & Sales
Phase 6: Credit/Udhaar Management
Phase 7: Stock & Tank Management
Phase 8: Expenses & Reports
Phase 9: Testing & Bug Fixes
Phase 10: Deployment & Production Launch
```

---

## PHASE 1: FOUNDATION & CORE SETUP

### Backend Setup

#### 1.1 Project Structure
```
Tasks:
- Create ASP.NET Core Web API project (.NET 8)
- Set up Clean Architecture layers:
  - PPM.API (Controllers, Middleware)
  - PPM.Core (Entities, Interfaces)
  - PPM.Application (Services, DTOs)
  - PPM.Infrastructure (Data, External Services)
- Configure Swagger/OpenAPI
- Set up logging (Serilog)
- Configure CORS
```

#### 1.2 Database Setup
```
Tasks:
- Install Entity Framework Core
- Install Npgsql (PostgreSQL provider)
- Create DbContext
- Configure connection strings
- Set up migration system
- Create initial database tables:
  - Tenants
  - SystemUsers
```

#### 1.3 Base Infrastructure
```
Tasks:
- Create base entity classes
- Implement generic repository pattern
- Set up unit of work pattern
- Create result/response wrapper
- Set up global exception handling
- Configure FluentValidation
```

### Frontend Setup

#### 1.4 Next.js Project
```
Tasks:
- Create Next.js 14+ app (App Router)
- Set up Tailwind CSS
- Configure TypeScript
- Install UI library (shadcn/ui or Material UI)
- Set up folder structure:
  - /app (pages)
  - /components
  - /lib (utilities)
  - /services (API clients)
- Configure environment variables
```

#### 1.5 API Client Setup
```
Tasks:
- Create Axios instance
- Set up API base configuration
- Create request/response interceptors
- Set up error handling
- Create TypeScript interfaces for DTOs
```

### Deliverables
- ✅ Backend API running on localhost:5001
- ✅ Frontend running on localhost:3000
- ✅ Database created with initial tables
- ✅ Swagger documentation accessible
- ✅ Basic health check endpoint

---

## PHASE 2: AUTHENTICATION & MULTI-TENANCY

### 2.1 Super Admin Module

#### Database
```
Tasks:
- Create Tenants table (complete schema)
- Create SystemUsers table
- Seed initial super admin user
```

#### Backend API
```
Tasks:
- Implement super admin authentication
- Create tenant CRUD endpoints
- Create tenant dashboard stats endpoint
- Implement tenant activation/deactivation
```

#### Frontend
```
Tasks:
- Create super admin login page
- Create tenant management dashboard
- Create tenant list/grid view
- Create add/edit tenant form
- Create tenant details page
```

### 2.2 JWT Authentication

#### Backend
```
Tasks:
- Install Microsoft.AspNetCore.Authentication.JwtBearer
- Configure JWT settings (secret, issuer, audience)
- Implement token generation service
- Implement refresh token mechanism
- Create authentication endpoints:
  - POST /api/auth/login
  - POST /api/auth/refresh
  - POST /api/auth/logout
  - POST /api/auth/change-password
```

#### Frontend
```
Tasks:
- Create authentication context/store
- Implement token storage (localStorage/cookies)
- Create login page
- Implement auto token refresh
- Create route guards/protected routes
- Handle 401 unauthorized responses
```

### 2.3 Multi-tenancy Implementation

#### Backend
```
Tasks:
- Create tenant resolution middleware
- Extract TenantId from JWT token
- Set TenantId in HttpContext
- Configure EF Core global query filters
- Test tenant data isolation
```

#### Frontend
```
Tasks:
- Store TenantId in auth state
- Include TenantId in API requests
- Create tenant switcher (if needed)
```

### Deliverables
- ✅ Super admin can login
- ✅ Super admin can create/manage tenants
- ✅ JWT authentication working
- ✅ Multi-tenant data isolation verified
- ✅ Protected routes implemented

---

## PHASE 3: MACHINE, NOZZLE & FUEL MANAGEMENT

### 3.1 Database Tables
```
Tasks:
- Create FuelTypes table
- Create FuelRates table
- Create Machines table
- Create Nozzles table
- Run migrations
- Seed default fuel types (Petrol, Diesel, CNG)
```

### 3.2 Backend APIs
```
Tasks:
- Implement Fuel Types CRUD
- Implement Fuel Rates CRUD
  - Current rate endpoint
  - Rate history endpoint
- Implement Machines CRUD
- Implement Nozzles CRUD
- Add validation (rate > 0, unique codes, etc.)
```

### 3.3 Frontend UI
```
Tasks:
- Create fuel management page
  - List fuel types
  - Add/edit fuel type
  - Update fuel rates (Owner only)
  - Rate history view
- Create machine management page
  - List machines
  - Add/edit machine
  - View machine details
- Create nozzle management
  - List nozzles per machine
  - Add/edit nozzle
  - Assign fuel type to nozzle
```

### Deliverables
- ✅ Owner can manage fuel types and rates
- ✅ Owner can add machines and nozzles
- ✅ System tracks current and historical fuel rates

---

## PHASE 4: SHIFT MANAGEMENT (CRITICAL MODULE)

### 4.1 User Management

#### Database
```
Tasks:
- Create Users table (Owner, Manager, Worker)
- Add role-based fields
```

#### Backend
```
Tasks:
- Implement Users CRUD
- Add role-based authorization
- Create worker profile endpoints
- Implement worker performance tracking
```

#### Frontend
```
Tasks:
- Create user management page
- Add/edit user form
- Worker list with filters
- Worker profile page
```

### 4.2 Shift Module

#### Database
```
Tasks:
- Create Shifts table
- Create ShiftReadings table
- Add indexes for performance
```

#### Backend
```
Tasks:
- Implement shift start endpoint
  - Validate worker and nozzle availability
  - Record opening meter reading
  - Set shift status to "Open"
- Implement get active shifts endpoint
- Implement get shift details endpoint
- Implement record intermediate reading endpoint
- Implement shift close endpoint (CRITICAL)
  - Calculate fuel sold
  - Calculate expected vs actual collection
  - Calculate variance
  - Generate shift report
- Implement shift history endpoint
```

#### Frontend
```
Tasks:
- Create shift assignment page (Manager)
  - Select worker
  - Select nozzle
  - Select shift type
  - Enter opening meter reading
- Create active shifts dashboard
  - Real-time shift data
  - Live sales updates
- Create shift detail view
  - All shift metrics
  - Bills created in shift
  - Payment breakdown
- Create shift closure screen (CRITICAL)
  - Enter closing meter reading
  - Show auto-calculations
  - Manager review and approval
  - Display reconciliation data
- Create shift report view
  - Detailed shift summary
  - Export to PDF
```

### 4.3 Worker Mobile Interface
```
Tasks:
- Create worker login page
- Create worker dashboard
  - Show active shift
  - Display current metrics
- Create shift status card
  - Running duration
  - Sales so far
  - Bills count
```

### Deliverables
- ✅ Manager can assign workers to shifts
- ✅ Workers can view their active shift
- ✅ Shift start/close workflow complete
- ✅ Reconciliation and variance calculation working
- ✅ Shift reports generated

---

## PHASE 5: BILLING & SALES

### 5.1 Database Tables
```
Tasks:
- Create Bills table
- Create BillItems table
- Create Payments table
- Add foreign keys and indexes
```

### 5.2 Backend APIs
```
Tasks:
- Implement create bill endpoint
  - Validate shift is active
  - Calculate amount (quantity × rate)
  - Create bill and bill items
  - Update shift totals
  - Generate bill number
- Implement get bill endpoint
- Implement get bills with filters
- Implement void bill endpoint (Manager only)
- Implement print bill endpoint
- Implement bill summary/stats endpoint
```

### 5.3 Frontend UI (Worker Mobile)
```
Tasks:
- Create bill creation flow
  - Enter meter reading before
  - Dispense fuel
  - Enter meter reading after
  - Auto-calculate quantity and amount
  - Enter customer details (optional)
  - Select payment mode
  - Submit and print
- Create bill list view
- Create bill details view
- Create print preview
```

### 5.4 Manager/Owner UI
```
Tasks:
- Create sales dashboard
  - Today's sales
  - Sales by nozzle
  - Sales by worker
  - Payment breakdown
- Create bill management
  - View all bills
  - Filter by date, worker, payment mode
  - Void bills (with reason)
- Create thermal printer integration
```

### Deliverables
- ✅ Workers can create bills during shift
- ✅ Bills auto-calculate fuel quantity and amount
- ✅ Bills update shift totals in real-time
- ✅ Bill printing works
- ✅ Manager can view and void bills

---

## PHASE 6: CREDIT/UDHAAR MANAGEMENT

### 6.1 Database Tables
```
Tasks:
- Create CreditCustomers table
- Create CreditTransactions table
```

### 6.2 Backend APIs
```
Tasks:
- Implement credit customer CRUD
- Implement create credit bill
  - Check credit limit
  - Update customer balance
  - Create bill with IsCreditBill = true
  - Create debit transaction
- Implement record payment
  - Create credit transaction
  - Update customer balance
- Implement credit transactions history
- Implement outstanding report
- Implement ageing report
```

### 6.3 Frontend UI
```
Tasks:
- Create credit customer management
  - Add/edit customer
  - Set credit limit
  - View customer details
  - Outstanding balance
- Create credit bill flow (Worker)
  - Search customer
  - Check available credit
  - Create credit bill
- Create payment recording
  - Select customer
  - Enter payment amount
  - Select payment mode
- Create credit dashboard
  - Total outstanding
  - Overdue customers
  - Ageing analysis
- Create credit reports
  - Outstanding by customer
  - Payment history
  - Ageing report
```

### Deliverables
- ✅ System can manage credit customers
- ✅ Workers can create credit bills
- ✅ Credit limits enforced
- ✅ Payment recording updates balances
- ✅ Outstanding and ageing reports available

---

## PHASE 7: STOCK & TANK MANAGEMENT

### 7.1 Database Tables
```
Tasks:
- Create Tanks table
- Create StockEntries table
```

### 7.2 Backend APIs
```
Tasks:
- Implement tank CRUD
- Implement create stock entry
  - Stock In (refill)
  - Stock Out (auto on bill)
  - Adjustment
- Implement stock history
- Implement stock variance report
- Implement low stock alerts
- Auto stock deduction on bill creation
```

### 7.3 Frontend UI
```
Tasks:
- Create tank management
  - Add/edit tanks
  - View current stock levels
  - Visual indicators (low stock alerts)
- Create stock entry form
  - Record refill (with invoice details)
  - Record adjustments
- Create stock history view
- Create stock variance report
  - Opening stock
  - Stock received
  - Stock sold
  - Expected vs actual closing
```

### Deliverables
- ✅ Tanks configured with fuel types
- ✅ Stock auto-deducts on sales
- ✅ Stock refill recorded with invoice
- ✅ Low stock alerts working
- ✅ Stock variance reports available

---

## PHASE 8: EXPENSES & REPORTS

### 8.1 Expense Management

#### Database
```
Tasks:
- Create Expenses table
```

#### Backend
```
Tasks:
- Implement expense CRUD
- Implement expense categories
- Implement expense summary by category
```

#### Frontend
```
Tasks:
- Create expense management page
  - Add/edit expense
  - List expenses with filters
  - Delete expense
- Create expense summary
  - By category
  - By date range
```

### 8.2 Reports & Analytics

#### Backend
```
Tasks:
- Implement daily sales report
- Implement monthly sales report
- Implement worker performance report
- Implement nozzle-wise report
- Implement profit/margin analysis
- Implement dashboard stats API
- Implement report export (PDF/Excel)
```

#### Frontend
```
Tasks:
- Create reports page
  - Select report type
  - Choose date range
  - Generate report
  - Export to PDF/Excel
- Create owner dashboard
  - Key metrics cards
  - Sales trends chart
  - Payment breakdown pie chart
  - Fuel-wise sales
  - Active shifts
  - Alerts
- Create manager dashboard
  - Active shifts focus
  - Quick actions
  - Pending items
- Create analytics page
  - Sales trends
  - Worker performance comparison
  - Fuel type analysis
  - Profit margins
```

### Deliverables
- ✅ Expense tracking functional
- ✅ All key reports available
- ✅ Dashboard shows real-time metrics
- ✅ Export to PDF/Excel works
- ✅ Charts and visualizations rendered

---

## PHASE 9: TESTING & BUG FIXES

### 9.1 Backend Testing
```
Tasks:
- Write unit tests for services
- Write integration tests for APIs
- Test multi-tenant data isolation
- Test role-based authorization
- Load testing (simulate multiple shifts)
- Test shift reconciliation calculations
- Test stock deduction accuracy
- Test credit limit enforcement
```

### 9.2 Frontend Testing
```
Tasks:
- Test all user flows
- Test responsive design (desktop, tablet, mobile)
- Test worker mobile interface
- Test bill creation flow
- Test shift closure flow
- Cross-browser testing
- Performance testing
```

### 9.3 Security Testing
```
Tasks:
- Test JWT token expiry and refresh
- Test tenant isolation (try to access other tenant data)
- Test SQL injection prevention
- Test XSS prevention
- Test authorization bypass attempts
- Penetration testing
```

### 9.4 Bug Fixes & Optimization
```
Tasks:
- Fix identified bugs
- Optimize slow queries
- Add database indexes
- Optimize frontend bundle size
- Code cleanup and refactoring
- Documentation updates
```

### Deliverables
- ✅ 90%+ test coverage on critical modules
- ✅ All major bugs fixed
- ✅ Performance optimized
- ✅ Security hardened

---

## PHASE 10: DEPLOYMENT & PRODUCTION LAUNCH

### 10.1 Docker Setup
```
Tasks:
- Create Dockerfile for backend
- Create Dockerfile for frontend
- Create docker-compose.yml
  - API service
  - Frontend service
  - PostgreSQL service
  - Redis (optional, for caching)
- Test local Docker deployment
```

### 10.2 Production Environment
```
Tasks:
- Choose hosting provider (AWS/Azure/DigitalOcean)
- Set up production database (PostgreSQL)
- Configure environment variables
- Set up SSL certificates (HTTPS)
- Configure domain names
- Set up email service (for notifications)
- Set up backup strategy
```

### 10.3 CI/CD Pipeline
```
Tasks:
- Set up GitHub Actions or similar
- Configure build pipeline
- Configure test pipeline
- Configure deployment pipeline
- Set up staging environment
- Set up production environment
```

### 10.4 Monitoring & Logging
```
Tasks:
- Set up application logging
- Set up error tracking (Sentry/AppInsights)
- Set up performance monitoring
- Set up uptime monitoring
- Create admin alerts
```

### 10.5 Documentation
```
Tasks:
- User manual (PDF)
- API documentation (Swagger)
- Deployment guide
- Admin guide
- Video tutorials (optional)
```

### 10.6 Launch Preparation
```
Tasks:
- Create demo tenant
- Prepare marketing materials
- Set up support channel
- Create pricing plans
- Prepare onboarding flow
```

### Deliverables
- ✅ Application deployed to production
- ✅ SSL/HTTPS configured
- ✅ Database backups automated
- ✅ Monitoring and alerts set up
- ✅ Documentation complete
- ✅ Ready for customer onboarding

---

## 🚀 OPTIONAL FUTURE ENHANCEMENTS

### Phase 11: Mobile Apps
```
- React Native mobile app (Android/iOS)
- Worker mobile app optimization
- Offline mode support
- Push notifications
```

### Phase 12: Advanced Features
```
- WhatsApp/SMS notifications
- Email reports automation
- IoT integration with fuel dispensers
  (auto meter reading capture)
- QR code for bill payments
- Integration with accounting software
  (Tally, QuickBooks)
- Customer loyalty program
- Fuel price API integration
- Multi-language support
```

### Phase 13: AI/ML Features
```
- Sales forecasting
- Anomaly detection (fraud detection)
- Predictive maintenance alerts
- Customer churn prediction
```

---

## 📊 DEVELOPMENT APPROACH

### Agile Methodology
```
- 2-week sprints
- Daily standups
- Sprint planning
- Sprint retrospectives
- Continuous integration
```

### Team Structure (Recommended)
```
- 1 Backend Developer (.NET)
- 1 Frontend Developer (React/Next.js)
- 1 Full-stack Developer (Flexible)
- 1 UI/UX Designer (Part-time)
- 1 QA Engineer
- 1 DevOps Engineer (Part-time)
```

---

## 🎯 CRITICAL SUCCESS FACTORS

### Must-Have Features (MVP)
1. ✅ Multi-tenant architecture
2. ✅ Shift management with reconciliation
3. ✅ Billing with multiple payment modes
4. ✅ Credit/Udhaar tracking
5. ✅ Stock management
6. ✅ Basic reports

### Performance Targets
- Page load: < 2 seconds
- API response: < 500ms
- Database queries: < 100ms
- Support 50+ concurrent tenants
- 99.9% uptime

### Data Integrity
- Zero tolerance for:
  - Lost bills
  - Incorrect shift calculations
  - Stock mismatches
  - Credit balance errors

---

## 📝 TECHNICAL DEBT MANAGEMENT

### Code Quality
```
- Follow SOLID principles
- Code reviews before merging
- Maintain consistent coding standards
- Regular refactoring sprints
- Keep dependencies updated
```

### Documentation
```
- Inline code comments (where necessary)
- API documentation (Swagger)
- Database schema documentation
- Architecture decision records (ADR)
```

---

## 🔐 SECURITY CHECKLIST

- [x] HTTPS only
- [x] JWT token authentication
- [x] Password hashing (BCrypt)
- [x] SQL injection prevention (EF Core)
- [x] XSS prevention (input sanitization)
- [x] CORS configured properly
- [x] Rate limiting
- [x] Input validation
- [x] Audit logging
- [x] Tenant data isolation

---

## 📈 GO-LIVE CHECKLIST

- [ ] All tests passing
- [ ] Security audit complete
- [ ] Performance testing done
- [ ] Database backups configured
- [ ] SSL certificates installed
- [ ] Monitoring and alerts set up
- [ ] Documentation complete
- [ ] Support process defined
- [ ] Pricing plans finalized
- [ ] Demo tenant prepared
- [ ] Marketing materials ready
- [ ] Customer onboarding flow tested

---

## 🎉 POST-LAUNCH ACTIVITIES

### First Week
```
- Monitor system performance
- Track user feedback
- Fix critical bugs immediately
- Provide onboarding support
```

### First Month
```
- Collect feature requests
- Analyze usage patterns
- Optimize based on real data
- Plan next sprint features
```

### Ongoing
```
- Regular feature releases
- Security updates
- Performance optimization
- Customer success check-ins
```

---

**END OF ROADMAP**

This roadmap provides a clear path from zero to production. Follow each phase sequentially, ensuring deliverables are met before moving to the next phase. Prioritize data integrity and user experience throughout development.
