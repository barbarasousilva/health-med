#!/bin/bash

set -e

echo "🚀 Iniciando build das imagens..."

# Build das imagens
docker build -t barbarasousilva/healthmed-backend:latest ./backend
docker build -t barbarasousilva/healthmed-frontend:latest ./frontend

echo "🔐 Verificando login no Docker Hub..."
if ! docker info | grep -q Username; then
  echo "⚠️  Você não está logado no Docker Hub. Rodando docker login..."
  docker login
else
  echo "✅ Login já realizado."
fi

echo "📤 Publicando imagens no Docker Hub..."
docker push barbarasousilva/healthmed-backend:latest
docker push barbarasousilva/healthmed-frontend:latest

echo "✅ Publicação concluída com sucesso!"