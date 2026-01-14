#!/bin/bash
set -e

# PPM Deployment Script
# Usage: ./deploy.sh [environment]
# Environments: production (default), staging

ENVIRONMENT=${1:-production}
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"

# Configuration
if [ "$ENVIRONMENT" = "production" ]; then
    COMPOSE_FILE="docker-compose.external-db.yml"
    HEALTH_URL="http://localhost:5003/health"
elif [ "$ENVIRONMENT" = "staging" ]; then
    COMPOSE_FILE="docker-compose.staging.yml"
    HEALTH_URL="http://localhost:5004/health"
else
    echo "Unknown environment: $ENVIRONMENT"
    exit 1
fi

cd "$PROJECT_DIR"

# Read version from VERSION file
APP_VERSION=$(cat VERSION 2>/dev/null || echo "1.0.0")
GIT_COMMIT=$(git rev-parse --short HEAD 2>/dev/null || echo "unknown")

echo "=========================================="
echo "PPM Deployment - $ENVIRONMENT"
echo "=========================================="
echo "Project directory: $PROJECT_DIR"
echo "Compose file: $COMPOSE_FILE"
echo "Version: $APP_VERSION (commit: $GIT_COMMIT)"
echo ""

# Step 1: Pull latest code (if in git repo)
if [ -d ".git" ]; then
    echo "Pulling latest code..."
    git pull origin main || true
    # Update version info after pull
    APP_VERSION=$(cat VERSION 2>/dev/null || echo "1.0.0")
    GIT_COMMIT=$(git rev-parse --short HEAD 2>/dev/null || echo "unknown")
fi

# Export version for docker-compose
export APP_VERSION
export GIT_COMMIT

# Step 2: Stop existing containers
echo "Stopping existing containers..."
docker compose -f "$COMPOSE_FILE" down || true

# Step 3: Build images
echo "Building Docker images with version $APP_VERSION..."
docker compose -f "$COMPOSE_FILE" build --no-cache

# Step 4: Start containers
echo "Starting containers..."
docker compose -f "$COMPOSE_FILE" up -d

# Step 5: Wait for services to be ready
echo "Waiting for services to start..."
sleep 30

# Step 6: Health check
echo "Running health check..."
MAX_RETRIES=10
RETRY_COUNT=0

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if curl -sf "$HEALTH_URL" > /dev/null 2>&1; then
        echo "Health check passed!"
        break
    fi
    RETRY_COUNT=$((RETRY_COUNT + 1))
    echo "Health check failed, retrying... ($RETRY_COUNT/$MAX_RETRIES)"
    sleep 5
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "ERROR: Health check failed after $MAX_RETRIES attempts"
    echo "Container logs:"
    docker logs ppm-backend --tail 50
    exit 1
fi

# Step 7: Clean up old images
echo "Cleaning up old images..."
docker image prune -f

# Step 8: Show status
echo ""
echo "=========================================="
echo "Deployment completed successfully!"
echo "=========================================="
echo "Version: $APP_VERSION (commit: $GIT_COMMIT)"
docker ps --filter "name=ppm-"
echo ""
echo "Backend health: $(curl -sf $HEALTH_URL || echo 'FAILED')"
echo ""
echo "Version info:"
curl -sf http://localhost:5003/api/version || echo "Version endpoint not available"
