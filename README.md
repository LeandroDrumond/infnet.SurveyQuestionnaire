# Survey Questionnaire API

API para gerenciamento de questionários e coleta de respostas, desenvolvida com .NET9, Clean Architecture e Domain-Driven Design.

---

## 📋 Índice

- [O que é?](#o-que-é)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [**🚀 Como Executar o Projeto**](#-como-executar-o-projeto)
- [Setup inicial do banco (clonando pela primeira vez)](#setup-inicial-do-banco-clonando-pela-primeira-vez)
- [Por que é necessário rodar a Azure Function após os fluxos da API](#por-que-é-necessário-rodar-a-azure-function-após-os-fluxos-da-api)
- [Principais Endpoints](#principais-endpoints)
- [Testes com Postman](#testes-com-postman)
- [Padrões Utilizados](#padrões-utilizados)
- [Fluxo de Submissão Assíncrona](#fluxo-de-submissão-assíncrona)

---

## O que é?

Sistema para criar questionários, publicá-los e coletar respostas de usuários.

**Principais funcionalidades:**
- Administradores criam e gerenciam questionários
- Usuários públicos respondem questionários
- Questões abertas e de múltipla escolha
- Processamento assíncrono de respostas (Azure Service Bus)
- Cada usuário responde apenas uma vez por questionário

---

## Arquitetura

Projeto dividido em5 camadas seguindo Clean Architecture:

**Presentation Layer (API)**
- Controllers, DTOs, Validators

**Application Layer**
- Services, Interfaces, DTOs

**Domain Layer (Core)**
- Entities, Rules, Exceptions

**Infrastructure Layer**
- EF Core, Repositories, Service Bus

**Functions Layer**
- Azure Functions para processamento assíncrono

**Por que esta arquitetura?**
- Domain isolado (não depende de nada)
- Fácil testar regras de negócio
- Fácil trocar tecnologias (banco, mensageria)

---

## Tecnologias

- **.NET9** - Framework
- **C#** - Linguagem
- **EF Core** - ORM/Banco de dados
- **SQL Server (LocalDB)** - Persistência (desenvolvimento)
- **Azure Service Bus** - Fila de mensagens
- **Azure Functions** - Processamento assíncrono
- **Swagger** - Documentação da API
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validações

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- ✅ [.NET9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- ✅ [SQL Server LocalDB]
- ✅ [Azure Functions Core Tools] (para rodar as Functions localmente)
- ✅ [Azurite ou Azure Storage Emulator] (para a Function)

### Configurar o banco de dados

```bash
# Criar/Verificar instância LocalDB (Windows)
# Se já existir, pule este passo
sqllocaldb create mssqllocaldb
sqllocaldb start mssqllocaldb

# Verificar instância
sqllocaldb info mssqllocaldb
```

> Se não usar LocalDB, ajuste a connection string no arquivo `infnet.SurveyQuestionnaire.Api/appsettings.Development.json` para apontar para seu SQL Server.

### Instalar dotnet-ef (se necessário)

```bash
# Instalar globalmente (se não tiver)
dotnet tool install --global dotnet-ef
# Ou atualizar
dotnet tool update --global dotnet-ef
```

### Rodar migrations (primeira vez após clonar)

No root do repositório execute:

```bash
# Aplicar migrations no banco usando o projeto de startup da API
dotnet ef database update --project infnet.SurveyQuestionnaire.Infrastructure.Data --startup-project infnet.SurveyQuestionnaire.Api
```

Caso queira criar uma migration nova (não necessário se já vier com migrations):

```bash
# Criar migration (exemplo de nome InitialCreate)
cd infnet.SurveyQuestionnaire.Infrastructure.Data
dotnet ef migrations add InitialCreate --startup-project ../infnet.SurveyQuestionnaire.Api

# Aplicar migration
dotnet ef database update --startup-project ../infnet.SurveyQuestionnaire.Api
```

### Executar a API

```bash
cd infnet.SurveyQuestionnaire.Api
dotnet run
```

Acesse: https://localhost:5001/swagger

---

## Setup inicial do banco (clonando pela primeira vez)

Se você clonou o repositório pela primeira vez, siga estes passos para configurar o ambiente de desenvolvimento e o banco de dados:

1. Clone o repositório e entre na pasta do projeto:
```bash
git clone https://github.com/LeandroDrumond/infnet.SurveyQuestionnaire.git
cd infnet.SurveyQuestionnaire
```

2. Crie e inicie a instância LocalDB (Windows):
```bash
sqllocaldb create mssqllocaldb
sqllocaldb start mssqllocaldb
```

3. Verifique a connection string em `infnet.SurveyQuestionnaire.Api/appsettings.Development.json`. Padrão esperado:
```json
"ConnectionStrings": {
 "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SurveyQuestionnaireDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
Altere conforme necessário para seu ambiente.

4. Instale a ferramenta EF Core CLI (se ainda não tiver):
```bash
dotnet tool install --global dotnet-ef
```

5. Aplicar migrations e criar o banco:
```bash
dotnet ef database update --project infnet.SurveyQuestionnaire.Infrastructure.Data --startup-project infnet.SurveyQuestionnaire.Api
```

6. (Opcional) Se preferir gerar a migration localmente antes de aplicar (quando estiver desenvolvendo):
```bash
cd infnet.SurveyQuestionnaire.Infrastructure.Data
dotnet ef migrations add MyMigrationName --startup-project ../infnet.SurveyQuestionnaire.Api
dotnet ef database update --startup-project ../infnet.SurveyQuestionnaire.Api
```

7. Inicie a API:
```bash
cd infnet.SurveyQuestionnaire.Api
dotnet run
```

---

## Por que é necessário rodar a Azure Function após os fluxos da API

A API cria a `Submission` e publica uma mensagem na fila do Azure Service Bus, mas **não processa os itens da resposta** — esse processamento é feito pela Azure Function. Ou seja:

- Ao criar uma submission via API você receberá `202 Accepted` e a entidade ficará com status `Pending`.
- A Azure Function consome a mensagem da fila e grava os `SubmissionItems` no banco, atualizando o status para `Completed` (ou `Failed` em caso de erro).

Se a Function não estiver rodando localmente:
- As mensagens permanecem na fila do Service Bus e as submissions ficarão em `Pending` até que a Function seja iniciada.
- Em ambiente local com Azurite/Service Bus real, as mensagens são processadas assim que a Function for iniciada.

Portanto, após executar os endpoints da API que geram mensagens (por exemplo, `POST /api/submissions`), você deve iniciar a Azure Function para processá-las.

### Como iniciar a Azure Function localmente

1. Verifique `infnet.SurveyQuestionnaire.Functions/local.settings.json` com `ServiceBusConnection` e `AzureWebJobsStorage` configurados.
2. Em um terminal separado rode:

```bash
cd infnet.SurveyQuestionnaire.Functions
func start
```

Enquanto a Function estiver rodando, ela irá consumir mensagens da fila `submission-queue` e processar as submissions.

---

## Principais Endpoints

**Usuários**
- POST /api/users - Criar usuário
- GET /api/users - Listar usuários
- GET /api/users/{id} - Buscar por ID

**Questionários**
- POST /api/questionnaires - Criar (Admin)
- GET /api/questionnaires - Listar todos
- POST /api/questionnaires/{id}/publish - Publicar
- POST /api/questionnaires/{id}/questions - Adicionar questão

**Submissões**
- POST /api/submissions - Responder questionário (Public)
- GET /api/submissions/{id} - Buscar resposta
- GET /api/submissions/questionnaire/{id}` - Listar respostas (Admin)

---

## Padrões Utilizados

- **DDD (Domain-Driven Design)** - Lógica de negócio no Domain
- **Repository Pattern** - Abstração de acesso a dados
- **Unit of Work** - Gerenciamento de transações
- **CQRS Simplificado** - Separação de leitura e escrita
- **Dependency Injection** - Inversão de controle

---

## Fluxo de Submissão Assíncrona

Como funciona o processamento de respostas:

1. Cliente envia POST /api/submissions
2. API valida e cria Submission (status: Pending)
3. API publica mensagem no Azure Service Bus
4. API retorna202 Accepted (não trava)
5. Azure Function processa a mensagem
6. Function adiciona SubmissionItems no banco
7. Function atualiza status para Completed ou Failed

Vantagens:
- API não trava esperando processamento
- Escalabilidade automática (Azure Functions)
- Retry automático em caso de falha

---

## Decisões Técnicas

**Clean Architecture** - Independência e testabilidade

**DDD** - Complexidade de negócio exige regras bem definidas

**EF Core** - Produtividade + Migrations

**Azure Service Bus** - Processamento assíncrono confiável

---

## Contribuindo

1. Fork o projeto
2. Crie uma branch (git checkout -b feature/nova-feature)
3. Commit (git commit -m 'Add nova feature')
4. Push (git push origin feature/nova-feature)
5. Abra um Pull Request

---

## Autor

**Leandro Drumond** - [GitHub](https://github.com/LeandroDrumond)

---

## Referências

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)
- [EF Core Docs](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Azure Functions](https://docs.microsoft.com/azure/azure-functions/)
- [Azure Service Bus](https://docs.microsoft.com/azure/service-bus-messaging/)
