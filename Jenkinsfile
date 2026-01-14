pipeline {
    agent any

    environment {
        // Server configuration
        SERVER_HOST = '75.119.129.104'
        SERVER_USER = 'root'
        DEPLOY_PATH = '/var/www/vhosts/loving-noether.75-119-129-104.plesk.page/httpdocs'
        GIT_REPO_PATH = '/var/www/vhosts/loving-noether.75-119-129-104.plesk.page/git/PPM.git'
        COMPOSE_FILE = 'docker-compose.external-db.yml'
    }

    stages {
        stage('Checkout') {
            steps {
                script {
                    env.GIT_COMMIT_SHORT = sh(script: 'git rev-parse --short HEAD', returnStdout: true).trim()
                    env.GIT_COMMIT_MSG = sh(script: 'git log -1 --pretty=%B', returnStdout: true).trim()
                }
                echo "Deploying commit: ${env.GIT_COMMIT_SHORT}"
                echo "Message: ${env.GIT_COMMIT_MSG}"
            }
        }

        stage('Update Server Git') {
            when {
                branch 'main'
            }
            steps {
                sshagent(credentials: ['contabo-ssh-key']) {
                    sh """
                        ssh -o StrictHostKeyChecking=no ${SERVER_USER}@${SERVER_HOST} '
                            cd ${GIT_REPO_PATH}
                            git fetch origin main
                            git update-ref refs/heads/main FETCH_HEAD
                            echo "Git repo updated to: \$(git log -1 --oneline)"
                        '
                    """
                }
            }
        }

        stage('Deploy Code') {
            when {
                branch 'main'
            }
            steps {
                sshagent(credentials: ['contabo-ssh-key']) {
                    sh """
                        ssh -o StrictHostKeyChecking=no ${SERVER_USER}@${SERVER_HOST} '
                            cd /tmp
                            rm -rf ppm_deploy
                            git clone ${GIT_REPO_PATH} ppm_deploy

                            # Sync to deploy path (preserve .env file)
                            rsync -av --delete \\
                                --exclude=".env" \\
                                --exclude="logs/" \\
                                ppm_deploy/ ${DEPLOY_PATH}/

                            rm -rf ppm_deploy
                            echo "Code deployed successfully"
                        '
                    """
                }
            }
        }

        stage('Build & Deploy Containers') {
            when {
                branch 'main'
            }
            steps {
                sshagent(credentials: ['contabo-ssh-key']) {
                    sh """
                        ssh -o StrictHostKeyChecking=no ${SERVER_USER}@${SERVER_HOST} '
                            cd ${DEPLOY_PATH}

                            echo "=== Stopping existing containers ==="
                            docker compose -f ${COMPOSE_FILE} down || true

                            echo "=== Building Docker images ==="
                            docker compose -f ${COMPOSE_FILE} build --no-cache

                            echo "=== Starting containers ==="
                            docker compose -f ${COMPOSE_FILE} up -d

                            echo "=== Waiting for services ==="
                            sleep 40
                        '
                    """
                }
            }
        }

        stage('Health Check') {
            when {
                branch 'main'
            }
            steps {
                sshagent(credentials: ['contabo-ssh-key']) {
                    sh """
                        ssh -o StrictHostKeyChecking=no ${SERVER_USER}@${SERVER_HOST} '
                            MAX_RETRIES=10
                            RETRY=0

                            while [ \$RETRY -lt \$MAX_RETRIES ]; do
                                if curl -sf http://localhost:5003/health > /dev/null 2>&1; then
                                    echo "Health check PASSED"
                                    docker ps --filter "name=ppm-"
                                    exit 0
                                fi
                                RETRY=\$((RETRY + 1))
                                echo "Retry \$RETRY/\$MAX_RETRIES..."
                                sleep 5
                            done

                            echo "Health check FAILED"
                            docker logs ppm-backend --tail 30
                            exit 1
                        '
                    """
                }
            }
        }

        stage('Cleanup') {
            when {
                branch 'main'
            }
            steps {
                sshagent(credentials: ['contabo-ssh-key']) {
                    sh """
                        ssh -o StrictHostKeyChecking=no ${SERVER_USER}@${SERVER_HOST} '
                            docker image prune -f
                            echo "Cleanup completed"
                        '
                    """
                }
            }
        }
    }

    post {
        success {
            echo "Deployment SUCCESSFUL - Commit: ${env.GIT_COMMIT_SHORT}"
        }
        failure {
            echo "Deployment FAILED - Commit: ${env.GIT_COMMIT_SHORT}"
        }
        cleanup {
            deleteDir()
        }
    }
}
