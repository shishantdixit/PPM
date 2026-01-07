#!/bin/bash
# ===========================================
# PPM - Database Backup Script
# Run via cron: 0 2 * * * /opt/ppm/scripts/backup.sh
# ===========================================

set -e

# Configuration
BACKUP_DIR="/opt/ppm/backups"
RETENTION_DAYS=7
DATE=$(date +%Y%m%d_%H%M%S)

# Create backup directory if not exists
mkdir -p $BACKUP_DIR

# Create backup
echo "Creating backup: ppm_$DATE.sql.gz"
docker exec ppm-postgres pg_dump -U postgres PPM | gzip > $BACKUP_DIR/ppm_$DATE.sql.gz

# Delete old backups
echo "Cleaning up backups older than $RETENTION_DAYS days..."
find $BACKUP_DIR -name "ppm_*.sql.gz" -mtime +$RETENTION_DAYS -delete

# List current backups
echo ""
echo "Current backups:"
ls -lh $BACKUP_DIR/ppm_*.sql.gz 2>/dev/null || echo "No backups found"

echo ""
echo "Backup completed: ppm_$DATE.sql.gz"
