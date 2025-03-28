# Arquitetura do Projeto - Health&Med

## Visão Geral
A aplicação segue o modelo client-server com comunicação via REST API.

## Componentes
- **Frontend**: React
- **Backend**: .NET 8 Web API
- **Banco de Dados**: PostgreSQL
- **Autenticação**: JWT
- **Orquestração**: Docker + Kubernetes

## Fluxo de Dados
1. Usuário acessa o frontend (React)
2. Frontend consome APIs protegidas com JWT
3. Backend realiza operações e comunica com PostgreSQL
4. Consultas e horários são gerenciados via APIs RESTful

## Diagrama da Arquitetura com Requisitos Funcionais Atendidos
                                                             
             ┌───────────────┐            ┌───────────────┐
             │    Médico     │            │   Paciente    │
             └──────┬────────┘            └──────┬────────┘
                    │                            │
     Login, gerenciar horários       Login, buscar, agendar, cancelar
                    │                            │
                    └──────────-──┬──────────────┘
                                  ▼
                    ┌────────────────────────────┐
                    │       Navegador Web        │
                    │  (React + Vite + Axios)    │
                    └────────────┬───────────────┘
                                 │
                                 ▼
                    ┌────────────────────────────┐
                    │       .NET 8 Web API       │
                    │  - AuthController          │
                    │  - MedicoController        │
                    │  - PacienteController      │
                    │  - Dapper (PostgreSQL)     │
                    └────────────┬───────────────┘
                                 │
                                 ▼
                    ┌────────────────────────────┐
                    │        PostgreSQL          │
                    │  - Médicos                 │
                    │  - Pacientes               │
                    │  - Horários                │
                    │  - Consultas               │
                    └────────────────────────────┘

## Justificativas Técnicas das Escolhas

.NET 8 Web API

    - Framework moderno, performático e ideal para APIs RESTful.
    - Suporte robusto para autenticação JWT e Entity Framework.
    - Alta integração com ferramentas DevOps e Docker/K8s.

React (com Vite e Tailwind)

    - Performance superior no build com Vite.
    - Componentização moderna e uso otimizado de hooks.
    - Tailwind para UI rápida e responsiva.

PostgreSQL

    - Banco de dados relacional robusto, gratuito e com excelente suporte a transações e concorrência.
    - Suporte ideal para agendamentos, disponibilidade e relacionamento entre entidades.

JWT para autenticação

    - Permite autenticação stateless.
    - Integração simples com .NET e segurança escalável.

Docker + Kubernetes

    - Containers garantem isolamento de ambientes e consistência.
    - Kubernetes garante escalabilidade, alta disponibilidade e orquestração automatizada.

GitHub Actions

    - CI/CD integrado ao repositório.
    - Automação de build, testes e deploy.


## Requisitos Não Funcionais Atendidos

1. Alta Disponibilidade

- Backend e frontend serão executados em containers independentes, permitindo resiliência. 
- Kubernetes será usado para orquestração com réplicas e auto-healing. 
- Banco com persistência via volumes.

2. Escalabilidade

- API e frontend são stateless → escaláveis horizontalmente. 
- K8s gerencia réplicas automáticas com base em carga.
- Banco PostgreSQL com capacidade para lidar com até 20.000 conexões simultâneas em pico, dependendo da configuração.

3. Segurança

- Autenticação JWT com tokens seguros e expirados.
- Senhas serão armazenadas com hash. 
- Utilização de HTTPS nas chamadas (mesmo local via reverse proxy). 
- Docker + Kubernetes garante isolamento de rede e containers.
- Segredos e variáveis sensíveis isoladas em arquivos .env e Kubernetes Secrets.