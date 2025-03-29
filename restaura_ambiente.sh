#!/bin/bash

# Nome do cluster KIND
CLUSTER_NAME="kind"

# Inicia o serviço do Docker (caso esteja usando WSL)
echo "🐳 Iniciando Docker..."
sudo service docker start > /dev/null 2>&1 || echo "⚠️  Docker já deve estar rodando via Docker Desktop ou systemd no WSL2."

# Verifica se o cluster já existe
if ! kind get clusters | grep -q "$CLUSTER_NAME"; then
  echo "🔧 Cluster KIND não encontrado. Criando..."
  kind create cluster --name $CLUSTER_NAME
else
  echo "✅ Cluster KIND já existe. Pulando criação."
fi

# Aplica Secrets, ConfigMaps, Deployments e Services
echo "🚀 Aplicando manifests Kubernetes..."
kubectl apply -f k8s/

# Aguarda os pods estarem prontos
echo "⏳ Aguardando pods ficarem prontos..."
kubectl wait --for=condition=ready pod --all --timeout=120s

# Executa o Job de inicialização do banco
echo "📦 Executando job de inicialização do banco de dados..."
kubectl delete job init-db-job --ignore-not-found=true
kubectl apply -f k8s/init-job.yaml

# Confirmação final
echo -e "\n✅ Ambiente restaurado com sucesso!"

