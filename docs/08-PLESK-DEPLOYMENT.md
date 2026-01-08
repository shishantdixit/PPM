# Plesk Deployment Guide - PPM (Petrol Pump Management System)

## Overview

This guide covers deploying PPM on a Contabo VPS with Plesk Panel.

**Server:** `75.119.129.104`
**Frontend Port:** `3001`
**Backend Port:** `5002`

---

## Step 1: Add Main Website (Frontend)

### 1.1 Create Website in Plesk

1. Login to Plesk: `https://75.119.129.104:8443`
2. Go to **Websites & Domains**
3. Click **Add Domain**
4. Choose **Temporary domain** or enter your domain name
   - Temporary domain example: `ppm.75.119.129.104.nip.io`
   - Or use your actual domain: `ppm.yourdomain.com`
5. Select document root (default is fine)
6. Click **OK**

### 1.2 Configure Node.js for Frontend

1. In your domain's panel, click **Node.js**
2. Enable Node.js and configure:
   - **Node.js Version:** 20.x (or latest LTS)
   - **Document Root:** `/httpdocs`
   - **Application Mode:** Production
   - **Application Startup File:** `server.js`
3. Click **Enable Node.js**

---

## Step 2: Add API Subdomain (Backend)

### 2.1 Create Subdomain

1. Go to **Websites & Domains**
2. Click **Add Subdomain**
3. Enter subdomain name: `api`
   - Full subdomain: `api.ppm.yourdomain.com`
   - Or with temp domain: `api.ppm.75.119.129.104.nip.io`
4. Click **OK**

### 2.2 Note About .NET Backend

Plesk doesn't natively support .NET Core apps via GUI for Linux. We'll run the backend using Docker or systemd service.

---

## Step 3: Clone Repository via SSH

### 3.1 SSH into Server

```bash
ssh root@75.119.129.104
```

### 3.2 Navigate to Web Directory

```bash
# For Plesk, websites are typically in:
cd /var/www/vhosts/ppm.yourdomain.com/

# Or find your domain directory:
ls /var/www/vhosts/
```

### 3.3 Clone Repository

```bash
# Clone to a separate directory (not httpdocs)
cd /var/www/vhosts/ppm.yourdomain.com/
git clone https://github.com/your-repo/PPM.git app

# Or if you have private repo, use token:
git clone https://<token>@github.com/your-repo/PPM.git app
```

---

## Step 4: Install Prerequisites

### 4.1 Install Docker (if not installed)

```bash
# Check if Docker is installed
docker --version

# If not, install:
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh
systemctl enable docker
systemctl start docker
```

### 4.2 Install Docker Compose

```bash
# Check version
docker compose version

# If not installed:
apt install docker-compose-plugin -y
```

### 4.3 Install .NET SDK (Optional - if running without Docker)

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET 9.0 SDK
apt update
apt install -y dotnet-sdk-9.0
```

---

## Step 5: Setup PostgreSQL Database

Since PostgreSQL is already running on the server, we just need to create the database.

### 5.1 Connect to Existing PostgreSQL

```bash
# Connect to PostgreSQL as postgres user
sudo -u postgres psql
```

### 5.2 Create Database and User

```sql
-- Create database
CREATE DATABASE "PPM";

-- Create user (if needed)
CREATE USER ppm_user WITH ENCRYPTED PASSWORD 'YourSecurePassword123!';

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE "PPM" TO ppm_user;

-- Connect to the database and grant schema permissions
\c PPM
GRANT ALL ON SCHEMA public TO ppm_user;

-- Exit
\q
```

### 5.3 Verify Connection

```bash
# Test connection
psql -h localhost -U ppm_user -d PPM -c "SELECT 1"
```

### 5.4 Note Your Connection String

```
Host=localhost;Port=5432;Database=PPM;Username=ppm_user;Password=YourSecurePassword123!
```

---

## Step 6: Configure Environment

### 6.1 Create Environment File

```bash
cd /var/www/vhosts/ppm.yourdomain.com/app

cp .env.example .env
nano .env
```

### 6.2 Update Environment Variables

```env
# Database (using existing PostgreSQL on server)
POSTGRES_DB=PPM
POSTGRES_USER=ppm_user
POSTGRES_PASSWORD=YourSecurePassword123!

# JWT Secret - Generate a random 32+ character string
JWT_SECRET=your-super-secret-jwt-key-at-least-32-characters-long

# API URL - Use your domain/subdomain
NEXT_PUBLIC_API_URL=https://api.ppm.yourdomain.com/api

# Or with IP and port:
# NEXT_PUBLIC_API_URL=http://75.119.129.104:5002/api
```

---

## Step 7: Deploy Backend (API)

### Option A: Docker Deployment (Recommended)

```bash
cd /var/www/vhosts/ppm.yourdomain.com/app

# Build backend container
docker build -t ppm-backend:latest ./backend

# Run with connection to host's PostgreSQL
docker run -d \
  --name ppm-backend \
  --add-host=host.docker.internal:host-gateway \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:5002 \
  -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5432;Database=PPM;Username=ppm_user;Password=YourSecurePassword123!" \
  -e "Jwt__Secret=your-super-secret-jwt-key-at-least-32-characters-long" \
  -p 5002:5002 \
  --restart unless-stopped \
  ppm-backend:latest
```

**Note:** `host.docker.internal` allows the Docker container to connect to PostgreSQL running on the host machine.

### Option B: Docker Compose (Recommended for Production)

```bash
cd /var/www/vhosts/ppm.yourdomain.com/app

# Use the external database compose file
docker compose -f docker-compose.external-db.yml up -d --build
```

### Option C: Systemd Service (Without Docker)

```bash
# Build the backend
cd /var/www/vhosts/ppm.yourdomain.com/app/backend
dotnet publish -c Release -o /opt/ppm-backend

# Create systemd service
cat > /etc/systemd/system/ppm-backend.service << 'EOF'
[Unit]
Description=PPM Backend API
After=network.target postgresql.service

[Service]
WorkingDirectory=/opt/ppm-backend
ExecStart=/usr/bin/dotnet /opt/ppm-backend/PPM.API.dll
Restart=always
RestartSec=10
SyslogIdentifier=ppm-backend
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://+:5002
Environment=ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=PPM;Username=ppm_user;Password=YourSecurePassword123!
Environment=Jwt__Secret=your-super-secret-jwt-key-at-least-32-characters-long

[Install]
WantedBy=multi-user.target
EOF

# Enable and start service
systemctl daemon-reload
systemctl enable ppm-backend
systemctl start ppm-backend

# Check status
systemctl status ppm-backend
```

---

## Step 8: Deploy Frontend

### 8.1 Build Frontend

```bash
cd /var/www/vhosts/ppm.yourdomain.com/app/frontend

# Install dependencies
npm ci

# Set API URL and build
export NEXT_PUBLIC_API_URL=https://api.ppm.yourdomain.com/api
npm run build
```

### 8.2 Option A: Run with PM2 (Recommended)

```bash
# Install PM2 globally
npm install -g pm2

# Start frontend
cd /var/www/vhosts/ppm.yourdomain.com/app/frontend
pm2 start npm --name "ppm-frontend" -- start -- -p 3001

# Save PM2 configuration
pm2 save

# Setup PM2 to start on boot
pm2 startup systemd
```

### 8.2 Option B: Run with Docker

```bash
cd /var/www/vhosts/ppm.yourdomain.com/app

# Build and run frontend container
docker build -t ppm-frontend:latest \
  --build-arg NEXT_PUBLIC_API_URL=https://api.ppm.yourdomain.com/api \
  ./frontend

docker run -d \
  --name ppm-frontend \
  -p 3001:3000 \
  --restart unless-stopped \
  ppm-frontend:latest
```

---

## Step 9: Configure Plesk Proxy Rules

### 9.1 Frontend Proxy (Port 3001)

1. In Plesk, go to your main domain
2. Click **Apache & nginx Settings**
3. Scroll to **Additional nginx directives**
4. Add:

```nginx
location / {
    proxy_pass http://127.0.0.1:3001;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection 'upgrade';
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_cache_bypass $http_upgrade;
}
```

5. Click **OK** / **Apply**

### 9.2 API Subdomain Proxy (Port 5002)

1. In Plesk, go to your API subdomain (`api.ppm.yourdomain.com`)
2. Click **Apache & nginx Settings**
3. Add to **Additional nginx directives**:

```nginx
location / {
    proxy_pass http://127.0.0.1:5002;
    proxy_http_version 1.1;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

5. Click **OK** / **Apply**

---

## Step 10: Setup SSL Certificates

### 10.1 Using Let's Encrypt (Free)

1. In Plesk, go to your domain
2. Click **SSL/TLS Certificates**
3. Click **Install** under Let's Encrypt
4. Check:
   - [x] Secure the domain
   - [x] Include www subdomain (if applicable)
   - [x] Secure webmail (if needed)
5. Click **Get it free**

Repeat for API subdomain.

### 10.2 Redirect HTTP to HTTPS

1. Go to **Hosting Settings** for each domain
2. Check **Permanent SEO-safe 301 redirect from HTTP to HTTPS**
3. Click **OK**

---

## Step 11: Verify Deployment

### 11.1 Check Services

```bash
# Check backend
curl http://localhost:5002/health

# Check frontend
curl http://localhost:3001

# Check Docker containers (if using Docker)
docker ps

# Check PM2 (if using PM2)
pm2 status
```

### 11.2 Test External Access

```bash
# From your local machine:
curl https://ppm.yourdomain.com
curl https://api.ppm.yourdomain.com/health
```

---

## Step 12: Setup Automated Backups

### 12.1 Database Backup Script

```bash
cat > /opt/ppm-backup.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/var/www/vhosts/ppm.yourdomain.com/backups"
DATE=$(date +%Y%m%d_%H%M%S)
mkdir -p $BACKUP_DIR

# Backup using existing PostgreSQL on host
sudo -u postgres pg_dump PPM | gzip > $BACKUP_DIR/ppm_$DATE.sql.gz

# Keep only last 7 days
find $BACKUP_DIR -name "*.sql.gz" -mtime +7 -delete

echo "Backup completed: ppm_$DATE.sql.gz"
EOF

chmod +x /opt/ppm-backup.sh
```

### 12.2 Add to Crontab

```bash
crontab -e

# Add this line (daily backup at 2 AM):
0 2 * * * /opt/ppm-backup.sh >> /var/log/ppm-backup.log 2>&1
```

---

## Quick Reference Commands

```bash
# SSH to server
ssh root@75.119.129.104

# Navigate to app
cd /var/www/vhosts/ppm.yourdomain.com/app

# View backend logs (Docker)
docker logs -f ppm-backend

# View backend logs (Systemd)
journalctl -u ppm-backend -f

# View frontend logs (PM2)
pm2 logs ppm-frontend

# Restart backend (Docker)
docker restart ppm-backend

# Restart backend (Systemd)
systemctl restart ppm-backend

# Restart frontend (PM2)
pm2 restart ppm-frontend

# Update application
git pull origin main
docker restart ppm-backend      # or: systemctl restart ppm-backend
pm2 restart ppm-frontend

# Full rebuild (Docker)
docker build -t ppm-backend:latest ./backend
docker stop ppm-backend && docker rm ppm-backend
docker run -d --name ppm-backend ... (see Step 7)

# Full rebuild (Systemd)
cd backend && dotnet publish -c Release -o /opt/ppm-backend
systemctl restart ppm-backend

# Frontend rebuild
cd frontend && npm ci && npm run build && pm2 restart ppm-frontend

# Database access
sudo -u postgres psql -d PPM
```

---

## Troubleshooting

### Backend Not Starting

```bash
# Check logs
docker logs ppm-backend

# Or for systemd:
journalctl -u ppm-backend -f
```

### Frontend 502 Bad Gateway

```bash
# Check if frontend is running
pm2 status
# Or
docker ps | grep frontend

# Check nginx error logs
tail -f /var/log/nginx/error.log
```

### Database Connection Issues

```bash
# Test PostgreSQL connection
sudo -u postgres psql -d PPM -c "SELECT 1"

# Check if PostgreSQL is running
systemctl status postgresql

# Check PostgreSQL logs
tail -f /var/log/postgresql/postgresql-*-main.log
```

### Plesk nginx Reload

```bash
# After changing nginx directives
plesk repair web -y
# Or
systemctl restart nginx
```

---

## Access URLs

| Service | URL |
|---------|-----|
| Frontend | https://ppm.yourdomain.com |
| Backend API | https://api.ppm.yourdomain.com |
| Swagger Docs | https://api.ppm.yourdomain.com/swagger |
| Plesk Panel | https://75.119.129.104:8443 |

---

**Last Updated:** January 2026
