# Deployment Guide - PPM (Petrol Pump Management System)

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Local Development](#local-development)
3. [Docker Deployment](#docker-deployment)
4. [Contabo VPS Deployment](#contabo-vps-deployment) ⭐
5. [Production Deployment](#production-deployment)
6. [SSL Configuration](#ssl-configuration)
7. [Database Backup](#database-backup)
8. [Monitoring](#monitoring)
9. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software
- Docker 24.0+ and Docker Compose 2.0+
- Git
- Node.js 20+ (for local development)
- .NET SDK 9.0 (for local development)

### System Requirements
| Resource | Minimum | Recommended |
|----------|---------|-------------|
| CPU | 2 cores | 4 cores |
| RAM | 4 GB | 8 GB |
| Storage | 20 GB | 50 GB |
| OS | Ubuntu 22.04 / Windows Server 2022 | Ubuntu 22.04 LTS |

---

## Local Development

### Backend Only
```bash
cd backend
dotnet restore
dotnet run --project PPM.API
```
Backend will be available at `http://localhost:5002`

### Frontend Only
```bash
cd frontend
npm install
npm run dev
```
Frontend will be available at `http://localhost:3001`

### Database (Docker)
```bash
# Start PostgreSQL only
docker-compose up -d postgres

# Connect to database
psql -h localhost -U postgres -d PPM
```

---

## Docker Deployment

### Development Environment

1. **Clone the repository**
```bash
git clone https://github.com/your-org/PPM.git
cd PPM
```

2. **Configure environment**
```bash
cp .env.example .env
# Edit .env with your values
```

3. **Start all services**
```bash
docker-compose up -d
```

4. **Access the application**
- Frontend: http://localhost:3001
- Backend API: http://localhost:5002
- Swagger: http://localhost:5002/swagger

### Build Individual Images
```bash
# Build backend
docker build -t ppm-backend:latest ./backend

# Build frontend
docker build -t ppm-frontend:latest ./frontend \
  --build-arg NEXT_PUBLIC_API_URL=http://localhost:5002/api
```

---

## Contabo VPS Deployment

### Server Information
- **Server IP:** `75.119.129.104`
- **SSH Access:** `ssh root@75.119.129.104`
- **OS:** Ubuntu 22.04 LTS (recommended)

### Step 1: Initial Server Setup

SSH into your VPS and run the setup script:

```bash
# Connect to VPS
ssh root@75.119.129.104

# Download and run setup script (or copy manually)
cd /opt
git clone https://github.com/your-repo/PPM.git ppm
cd ppm
chmod +x scripts/*.sh
./scripts/vps-setup.sh
```

The setup script will:
- Update system packages
- Install Docker and Docker Compose
- Configure firewall (UFW)
- Set up fail2ban for security
- Create application directories
- Set up swap space

### Step 2: Configure Environment

```bash
cd /opt/ppm

# Create environment file
cp .env.example .env

# Edit with your values
nano .env
```

**Required values in `.env`:**
```env
# Database - Use strong passwords!
POSTGRES_DB=PPM
POSTGRES_USER=postgres
POSTGRES_PASSWORD=YourSecurePassword123!

# JWT - Generate a random 32+ character string
JWT_SECRET=your-super-secret-jwt-key-at-least-32-chars

# API URL - Use your server IP or domain
NEXT_PUBLIC_API_URL=http://75.119.129.104:5002/api
```

### Step 3: Deploy Application

```bash
# Run deployment script
./scripts/deploy.sh
```

Or manually:
```bash
# Build and start all services
docker compose -f docker-compose.prod.yml up -d --build

# Check status
docker compose -f docker-compose.prod.yml ps

# View logs
docker compose -f docker-compose.prod.yml logs -f
```

### Step 4: Verify Deployment

```bash
# Check backend health
curl http://75.119.129.104:5002/health

# Check frontend
curl http://75.119.129.104:3001
```

**Access URLs:**
| Service | URL |
|---------|-----|
| Frontend | http://75.119.129.104:3001 |
| Backend API | http://75.119.129.104:5002 |
| Swagger Docs | http://75.119.129.104:5002/swagger |

### Step 5: Set Up Automated Backups

```bash
# Make backup script executable
chmod +x /opt/ppm/scripts/backup.sh

# Add to crontab (daily at 2 AM)
crontab -e

# Add this line:
0 2 * * * /opt/ppm/scripts/backup.sh >> /var/log/ppm-backup.log 2>&1
```

### Quick Commands Reference

```bash
# SSH to server
ssh root@75.119.129.104

# Navigate to app
cd /opt/ppm

# View running containers
docker compose -f docker-compose.prod.yml ps

# View logs
docker compose -f docker-compose.prod.yml logs -f
docker compose -f docker-compose.prod.yml logs -f backend  # Backend only
docker compose -f docker-compose.prod.yml logs -f frontend # Frontend only

# Restart services
docker compose -f docker-compose.prod.yml restart

# Stop all services
docker compose -f docker-compose.prod.yml down

# Rebuild and restart
docker compose -f docker-compose.prod.yml up -d --build

# Database shell
docker exec -it ppm-postgres psql -U postgres PPM

# Manual backup
./scripts/backup.sh

# Restore from backup
./scripts/restore.sh backups/ppm_20240101_120000.sql.gz

# Check disk space
df -h

# Check memory
free -h

# Check Docker disk usage
docker system df
```

---

## Production Deployment

### Step 1: Server Preparation

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo apt install docker-compose-plugin -y

# Create application directory
sudo mkdir -p /opt/ppm
cd /opt/ppm
```

### Step 2: Clone and Configure

```bash
# Clone repository
git clone https://github.com/your-org/PPM.git .

# Create environment file
cp .env.example .env
nano .env
```

**Required Environment Variables:**
```env
POSTGRES_PASSWORD=<strong-password>
JWT_SECRET=<min-32-character-secret>
NEXT_PUBLIC_API_URL=https://yourdomain.com/api
```

### Step 3: Deploy with Production Compose

```bash
# Build and start
docker-compose -f docker-compose.prod.yml up -d --build

# Check logs
docker-compose -f docker-compose.prod.yml logs -f

# Check status
docker-compose -f docker-compose.prod.yml ps
```

### Step 4: Verify Deployment

```bash
# Check backend health
curl http://localhost:5002/health

# Check database connection
docker exec ppm-postgres pg_isready -U postgres
```

---

## SSL Configuration

### Option 1: Let's Encrypt (Recommended)

1. **Install Certbot**
```bash
sudo apt install certbot -y
```

2. **Obtain Certificate**
```bash
sudo certbot certonly --standalone -d yourdomain.com
```

3. **Copy Certificates**
```bash
sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem ./nginx/ssl/
sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem ./nginx/ssl/
```

4. **Enable HTTPS in nginx.conf**
- Uncomment the HTTPS server block in `nginx/nginx.conf`
- Update `server_name` with your domain

5. **Restart Nginx**
```bash
docker-compose -f docker-compose.prod.yml restart nginx
```

### Option 2: Cloudflare Proxy
1. Add your domain to Cloudflare
2. Enable "Full (strict)" SSL mode
3. Use Cloudflare Origin Certificates

---

## Database Backup

### Manual Backup
```bash
# Create backup
docker exec ppm-postgres pg_dump -U postgres PPM > backup_$(date +%Y%m%d_%H%M%S).sql

# Compress
gzip backup_*.sql
```

### Restore from Backup
```bash
# Stop application
docker-compose -f docker-compose.prod.yml stop backend frontend

# Restore
gunzip -c backup_20240101_120000.sql.gz | docker exec -i ppm-postgres psql -U postgres PPM

# Start application
docker-compose -f docker-compose.prod.yml start backend frontend
```

### Automated Backup Script
```bash
#!/bin/bash
# /opt/ppm/scripts/backup.sh

BACKUP_DIR="/opt/ppm/backups"
DATE=$(date +%Y%m%d_%H%M%S)
RETENTION_DAYS=7

mkdir -p $BACKUP_DIR

# Create backup
docker exec ppm-postgres pg_dump -U postgres PPM | gzip > $BACKUP_DIR/ppm_$DATE.sql.gz

# Delete old backups
find $BACKUP_DIR -name "*.sql.gz" -mtime +$RETENTION_DAYS -delete

echo "Backup completed: ppm_$DATE.sql.gz"
```

Add to crontab:
```bash
# Daily backup at 2 AM
0 2 * * * /opt/ppm/scripts/backup.sh >> /var/log/ppm-backup.log 2>&1
```

---

## Monitoring

### Health Check Endpoints
| Endpoint | Description |
|----------|-------------|
| `GET /health` | Backend health check |
| `GET /api/health` | API health with database status |

### Docker Logs
```bash
# All services
docker-compose -f docker-compose.prod.yml logs -f

# Specific service
docker-compose -f docker-compose.prod.yml logs -f backend
```

### Resource Usage
```bash
# Container stats
docker stats

# Disk usage
docker system df
```

### Recommended Monitoring Tools
- **Uptime Kuma** - Self-hosted uptime monitoring
- **Prometheus + Grafana** - Metrics and dashboards
- **Sentry** - Error tracking

---

## Troubleshooting

### Common Issues

#### 1. Database Connection Failed
```bash
# Check if postgres is running
docker ps | grep postgres

# Check postgres logs
docker logs ppm-postgres

# Verify connection string in .env
```

#### 2. Frontend Build Failed
```bash
# Check for TypeScript errors
cd frontend && npx tsc --noEmit

# Clear cache and rebuild
docker-compose -f docker-compose.prod.yml build --no-cache frontend
```

#### 3. Port Already in Use
```bash
# Find process using port
sudo lsof -i :5002
sudo lsof -i :3001

# Kill process
sudo kill -9 <PID>
```

#### 4. Out of Memory
```bash
# Clear Docker cache
docker system prune -a

# Limit container memory in docker-compose
deploy:
  resources:
    limits:
      memory: 512M
```

#### 5. SSL Certificate Issues
```bash
# Test certificate
openssl s_client -connect yourdomain.com:443

# Renew Let's Encrypt
sudo certbot renew
```

### Useful Commands

```bash
# Rebuild and restart
docker-compose -f docker-compose.prod.yml up -d --build

# View all logs
docker-compose -f docker-compose.prod.yml logs -f --tail=100

# Enter container shell
docker exec -it ppm-backend /bin/sh

# Database shell
docker exec -it ppm-postgres psql -U postgres PPM

# Restart single service
docker-compose -f docker-compose.prod.yml restart backend
```

---

## CI/CD Pipeline

The project includes GitHub Actions workflow (`.github/workflows/ci-cd.yml`) that:

1. **On Pull Request:**
   - Builds backend and frontend
   - Runs type checking
   - Runs tests

2. **On Push to Main:**
   - Builds Docker images
   - (Optional) Pushes to registry
   - (Optional) Deploys to production

### Manual Deployment
```bash
# Pull latest changes
cd /opt/ppm
git pull origin main

# Rebuild and restart
docker-compose -f docker-compose.prod.yml up -d --build
```

---

## Security Checklist

- [ ] Change default database password
- [ ] Use strong JWT secret (min 32 characters)
- [ ] Enable HTTPS with valid SSL certificate
- [ ] Configure firewall (allow only 80, 443)
- [ ] Set up automated backups
- [ ] Enable fail2ban for SSH protection
- [ ] Regular security updates
- [ ] Monitor logs for suspicious activity

---

## Support

For issues and questions:
- Create GitHub issue
- Email: support@yourcompany.com

---

**Last Updated:** January 2026
