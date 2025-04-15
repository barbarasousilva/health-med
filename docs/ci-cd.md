# 🚀 Pipeline CI/CD – Projeto HealthMed

Documentação completa da esteira de CI/CD, deploy e rollback do projeto HealthMed.

---

## 📦 CI – Integração Contínua

### GitHub Actions
- Arquivo: `.github/workflows/main.yml`
- Roda em:
  - Push na branch `main`
  - Pull Requests abertos contra `main`

### Etapas:
1. **Restore do NuGet**
2. **Build do projeto (.NET 8)**
3. **Testes (xUnit)**
4. **Build de imagens Docker**
5. **Push para Docker Hub**
6. **Deploy no AKS (Azure Kubernetes Service)**

### Variáveis sensíveis via Secrets:
- `DOCKER_USERNAME`
- `DOCKER_PASSWORD`
- `KUBE_CONFIG_DATA` (base64 do kubeconfig do AKS)

---

## 🐳 Imagens Docker

### Backend
- Repositório: `barbarasousilva/healthmed-backend`
- Build: `./backend/Dockerfile`

### Frontend
- Repositório: `barbarasousilva/healthmed-frontend`
- Build: `./frontend/Dockerfile`

### Compose (local)
- Arquivo: `compose.yml`
- Usado para build local e publicação de ambas as imagens

---

## ☸️ Deploy no AKS (Kubernetes)

### Requisitos
- Cluster criado via Azure CLI:
  ```bash
  az aks create \
    --resource-group health-med-rg \
    --name healthmed-aks \
    --node-count 1 \
    --node-vm-size Standard_B2s \
    --generate-ssh-keys
  ```

- Conectar ao cluster localmente:
  ```bash
  az aks get-credentials --resource-group health-med-rg --name healthmed-aks
  ```

- Converter o kubeconfig para secret:
  ```bash
  cat ~/.kube/config | base64 -w 0
  ```
  > Colar no GitHub Secrets como `KUBE_CONFIG_DATA`

### Aplicar manifests:
- Diretório: `k8s/`
- Comando usado pela pipeline:
  ```bash
  kubectl apply -f k8s/
  ```

---

## 🔁 Rollback

### Manual via terminal:

```bash
kubectl rollout undo deployment/backend
kubectl rollout undo deployment/frontend
```

### Ver histórico de versões:

```bash
kubectl rollout history deployment/backend
```

### Simulação de rollback (teste):
```bash
kubectl set image deployment/backend backend=nginx
kubectl rollout undo deployment/backend
```

---

## 🔒 Proteção da branch `main`

### Configurado no GitHub:
- Status checks obrigatórios:
  - `build-test` (CI)
- Bloqueio de merge se os testes falharem
- Merge permitido apenas após CI + aprovação (opcional)

---

## 📄 Estrutura de pastas relacionada ao deploy

```bash
/backend
/frontend
/k8s
  ├── configmap.yaml
  ├── secrets.yaml
  ├── deployments.yaml
  ├── services.yaml
  ├── init-job.yaml
  └── postgres.yaml
/compose.yml
/.github/workflows/main.yml
```

---

## 🧪 Como subir o projeto localmente

1. Ter Docker instalado e configurado
2. Clonar o repositório:
   ```bash
   git clone https://github.com/SEU_USUARIO/health-med.git
   cd health-med
   ```
3. Rodar:
   ```bash
   docker compose -f compose.yml up --build
   ```

> Após isso:
> - Backend: http://localhost:5001
> - Frontend: http://localhost:3000

---

## ☁️ Como subir no AKS manualmente

1. Estar com `az login` feito
2. Conectar ao cluster:
   ```bash
   az aks get-credentials --resource-group health-med-rg --name healthmed-aks
   ```
3. Aplicar manifests:
   ```bash
   kubectl apply -f k8s/
   ```

---


