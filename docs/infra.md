# Infraestrutura - Health&Med

## Docker
- Backend e frontend containerizados
- Compose para ambiente local de desenvolvimento

## Kubernetes
- Uso de Kind ou Minikube para orquestra��o local
- Manifests versionados em `k8s/`
- ConfigMaps e Secrets utilizados para vari�veis sens�veis

## CI/CD
- GitHub Actions
  - Build autom�tico
  - Testes
  - Deploy com docker-compose ou `kubectl apply`

## Banco de Dados
- PostgreSQL com persist�ncia via volumes