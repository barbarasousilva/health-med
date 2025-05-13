# 🩺 Health&Med – Sistema de Agendamento Médico

**Health&Med** é uma aplicação web desenvolvida em **.NET 8 + React**, com foco em agendamento de consultas médicas. O sistema permite que **pacientes busquem e agendem horários disponíveis de médicos**, e que **médicos cadastrem e gerenciem sua agenda**. Tudo é feito via uma interface responsiva, com segurança JWT, infraestrutura conteinerizada e deploy automatizado no Kubernetes (AKS).

---

## 🎥 Demonstração

Você pode assistir a uma demonstração da aplicação no link abaixo:

🔗 [Assista à demonstração do Health&Med](https://drive.google.com/file/d/1MMQND_aoaRipQDGoxK6axFgRJeK15zST/view?usp=sharing)

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
| Testes       | xUnit (.NET), Testes de Integração com banco real, launchSettings    |
| CI/CD        | Build, test, push e deploy automático via GitHub Actions             |

---

## 📁 Estrutura do Projeto (Resumo)

```
health-med/
├── backend/         # Solução .NET 8 com Domain, Application, API e Tests
│   ├── HealthMed.Tests.Unit
│   └── HealthMed.Tests.Integration  # Com suporte a launchSettings.json
├── frontend/        # SPA React com Vite e Tailwind
├── k8s/             # Manifests do Kubernetes (Deployments, Services, ConfigMaps...)
├── docs/            # Documentação do projeto (arquitetura, pipeline, etc)
├── scripts/         # Shell scripts para publicação de imagens e testes locais
├── compose.yml      # Docker Compose para ambiente local
├── .github/workflows/  # CI com GitHub Actions
```

---

## 🧪 Testes

- Testes de unidade com xUnit e Moq
- Testes de integração com banco PostgreSQL real (via Docker)
- Uso de `CustomWebApplicationFactory` com injeção de variáveis via ambiente
- `launchSettings.json` no projeto de integração para centralizar configuração local

### ✅ Execução local dos testes
```bash
# Subir banco de dados com init.sql
docker run -d --name healthmed-db-local \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=123456 \
  -e POSTGRES_DB=healthmeddb \
  -p 5432:5432 \
  -v $(pwd)/database/init.sql:/docker-entrypoint-initdb.d/init.sql \
  postgres:15

# Exportar variáveis de ambiente
export JWT_SECRET=...
export DB_CONNECTION_STRING="Host=localhost;Port=5432;..."
export ASPNETCORE_ENVIRONMENT=Development

# Executar testes
cd backend
dotnet test HealthMed.sln
```

---

## ☁️ CI/CD e Deploy

- CI executado em todo push ou PR para a branch `main`
- Build e push de imagens Docker no Docker Hub
- Deploy automático no AKS com `kubectl apply -f k8s/`
- Secrets lidos do GitHub Actions (JWT, conexão com o banco)
- Testes de unidade e integração rodando na pipeline
- Merge bloqueado em caso de falha nos testes

---

## 🔐 Autenticação e Segurança

- JWT com expiração e roles (`médico`, `paciente`)
- Middleware de autorização por perfil
- Senhas com hash via BCrypt
- Variáveis sensíveis com `Environment.GetEnvironmentVariable()`
- Secrets gerenciados via `.env`, GitHub Actions e Kubernetes Secrets

---

## 🧭 Como Rodar o Projeto Localmente

### Requisitos
- Docker e Docker Compose
- .NET 8 SDK (para rodar testes)

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

> O deploy está automatizado via pipeline, mas também pode ser feito manualmente:

```bash
az aks get-credentials --resource-group health-med-rg --name healthmed-aks
kubectl apply -f k8s/
```

Rollback:
```bash
kubectl rollout undo deployment/backend
kubectl rollout undo deployment/frontend
```

---

## 🔐 Variáveis de Ambiente e Secrets

O projeto utiliza `.env` para ambientes locais e `GitHub Secrets` + `Kubernetes Secrets` em produção.

### Exemplo de `.env`:
```env
JWT_SECRET=seu_token_jwt
DB_CONNECTION_STRING=Host=localhost;Port=5432;...
ASPNETCORE_ENVIRONMENT=Development
```

### Criar manualmente os secrets no cluster:
```bash
kubectl create secret generic secrets \
  --from-literal=JWT_SECRET=... \
  --from-literal=DB_CONNECTION_STRING=...
```

---

## 📄 Documentação
- [`docs/arquitetura.md`](./docs/arquitetura.md): Arquitetura do sistema
- [`docs/pipeline.md`](./docs/pipeline.md): CI/CD, deploy, rollback, testes

---

## 🙋‍♀️ Autoria

**Bárbara da Silva**  
[LinkedIn](https://www.linkedin.com/in/barbarasousilva) • Full Stack Engineer • Especialista em Arquitetura .NET

---

## ✅ Status do Projeto

**Concluído** – Rodando com CI/CD completo, testes automatizados, arquitetura limpa e pronto para escalar em produção com AKS.











Bus