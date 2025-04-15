#!/bin/bash

CLUSTER_NAME="kind"

echo "🐳 Iniciando Docker..."
sudo service docker start > /dev/null 2>&1 || echo "⚠️  Docker já está rodando."

# Cria o cluster KIND se não existir
if ! kind get clusters | grep -q "$CLUSTER_NAME"; then
  echo "🔧 Criando cluster KIND..."
  kind create cluster --name $CLUSTER_NAME
else
  echo "✅ Cluster KIND já existe."
fi

# Aplica os manifests Kubernetes
echo "🚀 Aplicando manifests Kubernetes..."
kubectl apply -f k8s/

# Aguardando os pods ficarem prontos
echo "⏳ Aguardando pods ficarem prontos..."
kubectl wait --for=condition=ready pod --all --timeout=120s

# Executa o job de inicialização do banco
echo "📦 Executando job de inicialização do banco..."
kubectl delete job init-db-job --ignore-not-found=true
kubectl apply -f k8s/init-job.yaml

# Aguarda o serviço do banco estar disponível e faz port-forward
echo "🔗 Expondo o PostgreSQL na porta local 5433..."
kubectl port-forward svc/postgres 5433:5432 > /dev/null 2>&1 &
sleep 3

# Conecta automaticamente com o psql
echo "🔌 Conectando ao PostgreSQL com psql (localhost:5433)..."

# Variáveis (use as mesmas do .env se quiser)
DB_USER=${DB_USER:-postgres}
DB_NAME=${DB_NAME:-healthmeddb}
DB_PASS=${DB_PASS:-123456}

export PGPASSWORD=$DB_PASS

# Rodar psql dentro da mesma aba
psql -h 127.0.0.1 -p 5433 -U $DB_USER -d $DB_NAME
