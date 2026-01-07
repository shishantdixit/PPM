#!/bin/bash
# ===========================================
# PPM - Deployment Script
# For Contabo VPS: 75.119.129.104
# ===========================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
APP_DIR="/opt/ppm"
BACKUP_DIR="/opt/ppm/backups"
COMPOSE_FILE="docker-compose.prod.yml"

echo -e "${GREEN}=========================================="
echo "PPM - Deployment Script"
echo -e "==========================================${NC}"

cd $APP_DIR

# Check if .env exists
if [ ! -f .env ]; then
    echo -e "${RED}Error: .env file not found!${NC}"
    echo "Run: cp .env.example .env && nano .env"
    exit 1
fi

# Backup database before deployment (if running)
if docker ps | grep -q ppm-postgres; then
    echo -e "${YELLOW}[1/5] Backing up database...${NC}"
    BACKUP_FILE="$BACKUP_DIR/pre-deploy_$(date +%Y%m%d_%H%M%S).sql.gz"
    docker exec ppm-postgres pg_dump -U postgres PPM | gzip > $BACKUP_FILE
    echo "Backup saved: $BACKUP_FILE"
else
    echo -e "${YELLOW}[1/5] No running database found, skipping backup${NC}"
fi

# Pull latest code (if git repo)
if [ -d .git ]; then
    echo -e "${YELLOW}[2/5] Pulling latest code...${NC}"
    git pull origin main
else
    echo -e "${YELLOW}[2/5] Not a git repo, skipping pull${NC}"
fi

# Build and deploy
echo -e "${YELLOW}[3/5] Building Docker images...${NC}"
docker compose -f $COMPOSE_FILE build --no-cache

echo -e "${YELLOW}[4/5] Starting services...${NC}"
docker compose -f $COMPOSE_FILE up -d

# Wait for services to be healthy
echo -e "${YELLOW}[5/5] Waiting for services to be ready...${NC}"
sleep 10

# Health check
echo ""
echo -e "${GREEN}Checking service status...${NC}"
docker compose -f $COMPOSE_FILE ps

echo ""
if curl -s -o /dev/null -w "%{http_code}" http://localhost:5002/health | grep -q "200"; then
    echo -e "${GREEN}✓ Backend is healthy${NC}"
else
    echo -e "${RED}✗ Backend health check failed${NC}"
fi

if curl -s -o /dev/null -w "%{http_code}" http://localhost:3001 | grep -q "200"; then
    echo -e "${GREEN}✓ Frontend is healthy${NC}"
else
    echo -e "${YELLOW}⚠ Frontend may still be starting...${NC}"
fi

echo ""
echo -e "${GREEN}=========================================="
echo "Deployment Complete!"
echo "=========================================="
echo ""
echo "Access your application:"
echo "  Frontend: http://75.119.129.104:3001"
echo "  Backend API: http://75.119.129.104:5002"
echo "  Swagger: http://75.119.129.104:5002/swagger"
echo ""
echo "View logs:"
echo "  docker compose -f $COMPOSE_FILE logs -f"
echo -e "==========================================${NC}"
