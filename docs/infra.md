# Infraestrutura - Health&Med

## Docker
- Backend e frontend containerizados
- Compose para ambiente local de desenvolvimento

## Kubernetes
- Uso de Kind ou Minikube para orquestração local
- Manifests versionados em `k8s/`
- ConfigMaps e Secrets utilizados para variáveis sensíveis

## CI/CD
- GitHub Actions
  - Build automático
  - Testes
  - Deploy com docker-compose ou `kubectl apply`

## Banco de Dados
- PostgreSQL com persistência via volumes