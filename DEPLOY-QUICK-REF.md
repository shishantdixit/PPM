# PPM - Quick Deployment Reference

## Server: Contabo VPS
- **IP:** `75.119.129.104`
- **SSH:** `ssh root@75.119.129.104`

---

## First Time Setup

```bash
# 1. SSH to server
ssh root@75.119.129.104

# 2. Clone repo
cd /opt
git clone https://github.com/your-repo/PPM.git ppm
cd ppm

# 3. Run setup script
chmod +x scripts/*.sh
./scripts/vps-setup.sh

# 4. Configure environment
cp .env.example .env
nano .env   # Edit with your values

# 5. Deploy
./scripts/deploy.sh
```

---

## Daily Operations

### Deploy Updates
```bash
ssh root@75.119.129.104
cd /opt/ppm
git pull origin main
./scripts/deploy.sh
```

### View Logs
```bash
docker compose -f docker-compose.prod.yml logs -f
```

### Restart Services
```bash
docker compose -f docker-compose.prod.yml restart
```

### Database Backup
```bash
./scripts/backup.sh
```

### Database Restore
```bash
./scripts/restore.sh backups/ppm_YYYYMMDD_HHMMSS.sql.gz
```

---

## Access URLs

| Service | URL |
|---------|-----|
| Frontend | http://75.119.129.104:3001 |
| Backend API | http://75.119.129.104:5002 |
| Swagger | http://75.119.129.104:5002/swagger |

---

## Emergency Commands

```bash
# Stop everything
docker compose -f docker-compose.prod.yml down

# Restart everything
docker compose -f docker-compose.prod.yml up -d

# Check what's running
docker ps

# Check disk space
df -h

# Check memory
free -h

# View container logs
docker logs ppm-backend
docker logs ppm-frontend
docker logs ppm-postgres
```

---

## Troubleshooting

### Backend not starting
```bash
docker logs ppm-backend
# Check database connection in .env
```

### Frontend showing error
```bash
docker logs ppm-frontend
# Rebuild: docker compose -f docker-compose.prod.yml build frontend
```

### Database issues
```bash
docker exec -it ppm-postgres psql -U postgres PPM
# Run: \dt to list tables
```

### Out of disk space
```bash
docker system prune -a
```
