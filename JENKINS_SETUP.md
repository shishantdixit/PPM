# Jenkins CI/CD Setup for PPM

This guide explains how to set up Jenkins for automated deployment of the PPM application.

## Prerequisites

- Jenkins server installed and running
- SSH access to the Contabo VPS (75.119.129.104)
- GitHub repository configured

## Required Jenkins Plugins

Install these plugins from **Manage Jenkins > Plugins**:

1. **SSH Agent Plugin** - For SSH key authentication
2. **Pipeline** - For Jenkinsfile support
3. **Git** - For Git integration
4. **Credentials Binding** - For secure credential handling

## Setup Steps

### 1. Add SSH Credentials

1. Go to **Manage Jenkins > Credentials > System > Global credentials**
2. Click **Add Credentials**
3. Configure:
   - **Kind**: SSH Username with private key
   - **ID**: `contabo-ssh-key`
   - **Username**: `root`
   - **Private Key**: Enter directly (paste your SSH private key)
   - **Description**: Contabo VPS SSH Key

### 2. Add Server Host Credential (Optional)

1. Go to **Manage Jenkins > Credentials > System > Global credentials**
2. Click **Add Credentials**
3. Configure:
   - **Kind**: Secret text
   - **ID**: `ppm-server-host`
   - **Secret**: `75.119.129.104`
   - **Description**: PPM Server Host

### 3. Create Pipeline Job

1. Click **New Item**
2. Enter name: `PPM-Deploy`
3. Select **Pipeline**
4. Click **OK**

### 4. Configure Pipeline

In the job configuration:

**General:**
- Check "GitHub project"
- Project URL: `https://github.com/shishantdixit/PPM`

**Build Triggers:**
- Check "GitHub hook trigger for GITScm polling" (for automatic builds on push)
- Or check "Poll SCM" with schedule: `H/5 * * * *` (every 5 minutes)

**Pipeline:**
- Definition: **Pipeline script from SCM**
- SCM: **Git**
- Repository URL: `https://github.com/shishantdixit/PPM.git`
- Credentials: Add GitHub credentials if private repo
- Branch Specifier: `*/main`
- Script Path: `Jenkinsfile`

### 5. GitHub Webhook (Optional - for automatic builds)

1. Go to your GitHub repo > Settings > Webhooks
2. Add webhook:
   - Payload URL: `http://your-jenkins-url/github-webhook/`
   - Content type: `application/json`
   - Events: Just the push event
   - Active: Checked

## Pipeline Stages

The Jenkinsfile includes these stages:

1. **Checkout** - Clones the repository
2. **Update Server Git** - Updates the Plesk Git repo from GitHub
3. **Deploy Code** - Syncs code to the deploy directory
4. **Build & Deploy Containers** - Builds Docker images and starts containers
5. **Health Check** - Verifies the deployment is healthy
6. **Cleanup** - Removes old Docker images

## Environment Variables

Set these in the Jenkinsfile or as Jenkins environment variables:

| Variable | Value | Description |
|----------|-------|-------------|
| SERVER_HOST | 75.119.129.104 | VPS IP address |
| SERVER_USER | root | SSH user |
| DEPLOY_PATH | /var/www/vhosts/.../httpdocs | Deployment directory |
| COMPOSE_FILE | docker-compose.external-db.yml | Docker Compose file |

## Manual Trigger

To manually trigger a build:
1. Go to the job page
2. Click **Build Now**

## Troubleshooting

### SSH Connection Issues
```bash
# Test SSH from Jenkins server
ssh -i /path/to/key root@75.119.129.104 "echo 'Connection OK'"
```

### Build Failures
1. Check the Console Output in Jenkins
2. SSH to server and check Docker logs:
   ```bash
   docker logs ppm-backend --tail 100
   ```

### Health Check Failures
The health endpoint should return "Healthy":
```bash
curl http://localhost:5003/health
```

## Security Notes

- Never commit credentials to the repository
- Use Jenkins Credentials Manager for all secrets
- The `.env` file on the server is preserved during deployments
- SSH keys should be rotated periodically
