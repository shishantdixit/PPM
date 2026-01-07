#!/bin/bash
# ===========================================
# PPM - Database Restore Script
# Usage: ./restore.sh backup_file.sql.gz
# ===========================================

set -e

BACKUP_DIR="/opt/ppm/backups"

# Check if backup file is provided
if [ -z "$1" ]; then
    echo "Usage: ./restore.sh <backup_file.sql.gz>"
    echo ""
    echo "Available backups:"
    ls -lh $BACKUP_DIR/ppm_*.sql.gz 2>/dev/null || echo "No backups found"
    exit 1
fi

BACKUP_FILE=$1

# Check if file exists
if [ ! -f "$BACKUP_FILE" ]; then
    # Try with backup directory prefix
    if [ -f "$BACKUP_DIR/$BACKUP_FILE" ]; then
        BACKUP_FILE="$BACKUP_DIR/$BACKUP_FILE"
    else
        echo "Error: Backup file not found: $BACKUP_FILE"
        exit 1
    fi
fi

echo "=========================================="
echo "PPM - Database Restore"
echo "=========================================="
echo "Backup file: $BACKUP_FILE"
echo ""

read -p "This will OVERWRITE the current database. Are you sure? (yes/no): " confirm
if [ "$confirm" != "yes" ]; then
    echo "Restore cancelled."
    exit 0
fi

# Stop application services (keep database running)
echo "Stopping application services..."
docker compose -f /opt/ppm/docker-compose.prod.yml stop backend frontend 2>/dev/null || true

# Restore database
echo "Restoring database..."
gunzip -c "$BACKUP_FILE" | docker exec -i ppm-postgres psql -U postgres PPM

# Start application services
echo "Starting application services..."
docker compose -f /opt/ppm/docker-compose.prod.yml start backend frontend

echo ""
echo "=========================================="
echo "Restore Complete!"
echo "=========================================="
