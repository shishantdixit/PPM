#!/bin/bash
# ===========================================
# PPM - VPS Initial Setup Script
# For Contabo VPS (Ubuntu 22.04)
# Server: root@75.119.129.104
# ===========================================

set -e

echo "=========================================="
echo "PPM - VPS Initial Setup"
echo "=========================================="

# Update system
echo "[1/8] Updating system packages..."
apt update && apt upgrade -y

# Install essential packages
echo "[2/8] Installing essential packages..."
apt install -y \
    curl \
    wget \
    git \
    htop \
    nano \
    ufw \
    fail2ban \
    unzip

# Install Docker
echo "[3/8] Installing Docker..."
if ! command -v docker &> /dev/null; then
    curl -fsSL https://get.docker.com -o get-docker.sh
    sh get-docker.sh
    rm get-docker.sh
    systemctl enable docker
    systemctl start docker
else
    echo "Docker already installed"
fi

# Install Docker Compose
echo "[4/8] Installing Docker Compose..."
apt install -y docker-compose-plugin

# Configure Firewall
echo "[5/8] Configuring firewall..."
ufw default deny incoming
ufw default allow outgoing
ufw allow ssh
ufw allow 80/tcp
ufw allow 443/tcp
# Allow internal Docker communication
ufw allow from 172.16.0.0/12
echo "y" | ufw enable

# Configure fail2ban
echo "[6/8] Configuring fail2ban..."
systemctl enable fail2ban
systemctl start fail2ban

# Create application directory
echo "[7/8] Creating application directory..."
mkdir -p /opt/ppm
mkdir -p /opt/ppm/backups
mkdir -p /opt/ppm/logs

# Set up swap (if not exists) - useful for small VPS
echo "[8/8] Setting up swap space..."
if [ ! -f /swapfile ]; then
    fallocate -l 2G /swapfile
    chmod 600 /swapfile
    mkswap /swapfile
    swapon /swapfile
    echo '/swapfile none swap sw 0 0' >> /etc/fstab
    echo "Swap created"
else
    echo "Swap already exists"
fi

echo ""
echo "=========================================="
echo "VPS Setup Complete!"
echo "=========================================="
echo ""
echo "Next steps:"
echo "1. Clone your repository:"
echo "   cd /opt/ppm"
echo "   git clone https://github.com/your-repo/PPM.git ."
echo ""
echo "2. Configure environment:"
echo "   cp .env.example .env"
echo "   nano .env"
echo ""
echo "3. Deploy the application:"
echo "   ./scripts/deploy.sh"
echo ""
echo "Server IP: 75.119.129.104"
echo "=========================================="
