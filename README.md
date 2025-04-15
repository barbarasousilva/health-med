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
├── compose.yml      # Docker Compose para ambiente local
├── .github/workflows/  # CI com GitHub Actions
```

---

## 🧪 Testes

- Testes de unidade com xUnit e Moq
- Testes de integração com banco PostgreSQL real (via docker-compose)
- Pipeline executa os testes antes de qualquer deploy

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

## 📄 Documentação
- [`docs/arquitetura.md`](./docs/arquitetura.md): Arquitetura do sistema
- [`docs/ci-cd.md`](./docs/ci-cd.md): CI/CD, deploy e rollback

---

## 🙋‍♀️ Autoria
**Bárbara da Silva**  
[LinkedIn](https://www.linkedin.com/in/barbarasousilva) • Full Stack Engineer • Especialista em Arquitetura .NET

---

## ✅ Status do Projeto
**Concluído** – Em ambiente real (AKS) com CI/CD, testes e infraestrutura documentada.  
Pronto para evolução, extensão de funcionalidades e escalabilidade em produção.

