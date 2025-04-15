# 🩺 Health&Med – Sistema de Agendamento Médico

**Health&Med** é uma aplicação web desenvolvida em **.NET 8 + React**, com foco em agendamento de consultas médicas. O sistema permite que **pacientes busquem e agendem horários disponíveis de médicos**, e que **médicos cadastrem e gerenciem sua agenda**. Tudo é feito via uma interface responsiva, com segurança JWT, infraestrutura conteinerizada e deploy automatizado no Kubernetes (AKS).

---

## 🔍 Funcionalidades

### Pacientes
- Cadastro e login (via e-mail ou CPF)
- Listagem de médicos com filtros
- Agendamento de consultas
- Cancelamento com justificativa

### Médicos
- Cadastro e login (via CRM)
- Cadastro de horários disponíveis
- Aceitar ou recusar consultas agendadas

### Admin (futuro)
- Visão centralizada do sistema
- Monitoramento e métricas

---

## 🚀 Tecnologias Utilizadas

| Camada       | Stack                                                                 |
|--------------|-----------------------------------------------------------------------|
| Frontend     | React, Vite, TailwindCSS, Axios                                       |
| Backend      | .NET 8 Web API, Clean Architecture, Dapper, JWT                       |
| Banco        | PostgreSQL                                                            |
| Infra        | Docker, Docker Compose, Kubernetes (AKS), GitHub Actions             |
| Testes       | xUnit (.NET), Testes de Integração com banco real                    |
| CI/CD        | Build, test, push e deploy automático via GitHub Actions             |

---

## 📁 Estrutura do Projeto (Resumo)

```
health-med/
├── backend/         # Solução .NET 8 com Domain, Application, API e Tests
├── frontend/        # SPA React com Vite e Tailwind
├── k8s/             # Manifests do Kubernetes (Deployments, Services, ConfigMaps...)
├── docs/            # Documentação do projeto (arquitetura, pipeline, etc)
├── scripts/         # Shell scripts para publicação de imagens e restauração local
├── compose.yml      # Docker Compose para ambiente local
├── .github/workflows/  # CI com GitHub Actions
```

---

## 🧪 Testes

- Testes de unidade com xUnit e Moq
- Testes de integração com banco PostgreSQL real (via docker-compose)
- Pipeline executa os testes antes de qualquer deploy

### 🧪 Testes Locais

Os testes (unitários e de integração) estão disponíveis no repositório e são executados automaticamente na pipeline de CI/CD.  
Eles também podem ser rodados manualmente localmente:

```bash
dotnet test ./backend/HealthMed.Tests.Unit
dotnet test ./backend/HealthMed.Tests.Integration
```

---

## ☁️ CI/CD e Deploy

- CI executado em todo push ou PR para a branch `main`
- Build e push das imagens Docker no Docker Hub
- Deploy automático no AKS com `kubectl apply -f k8s/`
- Proteção contra merge caso os testes falhem

---

## 🔐 Autenticação e Segurança

- JWT com expiração e roles (`médico`, `paciente`)
- Middleware de autorização por perfil
- Senhas com hash SHA256
- Variáveis sensíveis em `.env` + Secrets do K8s

---

## 🧭 Como Rodar o Projeto Localmente

### Requisitos
- Docker e Docker Compose

### Passos
```bash
git clone https://github.com/seu-usuario/health-med.git
cd health-med
docker compose -f compose.yml up --build
```

Acesso local:
- Frontend: http://localhost:3000  
- Backend: http://localhost:5001  

---

## ☸️ Deploy no Kubernetes (AKS)

> O deploy está automatizado via pipeline, mas pode ser feito manualmente:

```bash
az aks get-credentials --resource-group health-med-rg --name healthmed-aks
kubectl apply -f k8s/
```

Para rollback:
```bash
kubectl rollout undo deployment/backend
kubectl rollout undo deployment/frontend
```

---

## 🔐 Variáveis de Ambiente e Arquivos Sensíveis

Este projeto utiliza arquivos `.env` e `secrets.yaml` para armazenar configurações sensíveis (como JWT, conexão com banco e chaves). Por segurança:

- **`.env`** e **`k8s/secrets.yaml`** NÃO devem ser versionados. Ambos já estão listados no `.gitignore`
- Um exemplo de variáveis está disponível em [`.env.example`](./.env.example)

### Como criar o seu `.env`
```bash
cp .env.example .env
```

### Como gerar os secrets no Kubernetes manualmente:
```bash
kubectl create secret generic secrets \
  --from-literal=DB_USER=postgres \
  --from-literal=DB_PASS=your_password \
  --from-literal=DB_CONNECTION_STRING="Host=db;Port=5432;Database=healthmeddb;Username=postgres;Password=your_password" \
  --from-literal=JWT_SECRET=your_jwt_secret_here
```

> O arquivo `secrets.yaml` pode ser recriado com base nas variáveis do `.env`. Documentado para garantir consistência entre os ambientes.

---

## 📄 Documentação
- [`docs/arquitetura.md`](./docs/arquitetura.md): Arquitetura do sistema
- [`docs/pipeline.md`](./docs/pipeline.md): CI/CD, deploy e rollback

---

## 🙋‍♀️ Autoria
**Bárbara da Silva**  
[LinkedIn](https://www.linkedin.com/in/barbarasousilva) • Full Stack Engineer • Especialista em Arquitetura .NET

---

## ✅ Status do Projeto
**Concluído** – Em ambiente real (AKS) com CI/CD, testes e infraestrutura documentada.  
Pronto para evolução, extensão de funcionalidades e escalabilidade em produção.

