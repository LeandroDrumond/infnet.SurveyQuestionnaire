# ?? Survey Questionnaire API

API para gerenciamento de questionários e coleta de respostas, desenvolvida com .NET 9, Clean Architecture e Domain-Driven Design.

---

## ?? O que é?

Sistema para criar questionários, publicá-los e coletar respostas de usuários.

**Principais funcionalidades:**
- ? Administradores criam e gerenciam questionários
- ? Usuários públicos respondem questionários
- ? Questões abertas e de múltipla escolha
- ? Processamento assíncrono de respostas (Azure Service Bus)
- ? Cada usuário responde apenas uma vez por questionário

---

## ??? Arquitetura

Projeto dividido em **5 camadas** seguindo Clean Architecture:

```
?? Presentation (API + Azure Functions)
   ?
?? Application (Services, DTOs, Interfaces)
   ?
?? Domain (Entities, Rules, Exceptions)
   ?
?? Infrastructure (EF Core, Repositories, Service Bus)
```

**Por que esta arquitetura?**
- Domain isolado (não depende de nada)
- Fácil testar regras de negócio
- Fácil trocar tecnologias (banco, mensageria)

---

## ??? Tecnologias

| Tecnologia | Uso |
|-----------|-----|
| .NET 9 | Framework |
| C# 13 | Linguagem |
| EF Core | ORM/Banco de dados |
| SQL Server | Persistência |
| Azure Service Bus | Fila de mensagens |
| Azure Functions | Processamento assíncrono |
| Swagger | Documentação da API |
| AutoMapper | Mapeamento de objetos |
| FluentValidation | Validações |

---

## ?? Estrutura do Projeto

```
infnet.SurveyQuestionnaire/
?
??? Api/        # Controllers, DTOs, Validators
??? Application/           # Services, Interfaces
??? Domain/        # Entities, Rules, Exceptions
??? Infrastructure.Data/   # EF Core, Repositories, Service Bus
??? Functions/        # Azure Functions (processamento)
```

---

## ?? Como Executar

### 1. Pré-requisitos

- .NET 9 SDK
- SQL Server (LocalDB)
- Azure Service Bus (opcional - para processamento assíncrono)

### 2. Configurar o banco de dados

```bash
cd infnet.SurveyQuestionnaire.Infrastructure.Data
dotnet ef database update --startup-project ../infnet.SurveyQuestionnaire.Api
```

### 3. Configurar appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SurveyQuestionnaireDb;Trusted_Connection=True"
  },
  "AzureServiceBus": {
    "ConnectionString": "sua-connection-string-aqui",
  "DefaultQueueName": "submission-queue"
  }
}
```

### 4. Executar a API

```bash
cd infnet.SurveyQuestionnaire.Api
dotnet run
```

**Swagger:** https://localhost:5001/swagger

---

## ?? Principais Endpoints

### Usuários
- `POST /api/users` - Criar usuário
- `GET /api/users` - Listar usuários
- `GET /api/users/{id}` - Buscar por ID

### Questionários
- `POST /api/questionnaires` - Criar (Admin)
- `GET /api/questionnaires` - Listar todos
- `POST /api/questionnaires/{id}/publish` - Publicar
- `POST /api/questionnaires/{id}/questions` - Adicionar questão

### Submissões
- `POST /api/submissions` - Responder questionário (Public)
- `GET /api/submissions/{id}` - Buscar resposta
- `GET /api/submissions/questionnaire/{id}` - Listar respostas (Admin)

---

## ?? Padrões Utilizados

- **DDD (Domain-Driven Design)**: Lógica de negócio no Domain
- **Repository Pattern**: Abstração de acesso a dados
- **Unit of Work**: Gerenciamento de transações
- **CQRS Simplificado**: Separação de leitura e escrita
- **Dependency Injection**: Inversão de controle

---

## ?? Fluxo de Submissão

```
1. POST /api/submissions
2. Valida e cria Submission (status: Pending)
3. Publica mensagem no Service Bus
4. Retorna 202 Accepted
5. Azure Function processa em background
6. Adiciona SubmissionItems
7. Atualiza status (Completed/Failed)
```

**Por quê?** API não trava esperando processamento + escalabilidade automática.

---

## ?? Decisões Técnicas

| Decisão | Motivo |
|---------|--------|
| Clean Architecture | Independência e testabilidade |
| DDD | Complexidade de negócio exige regras bem definidas |
| EF Core | Produtividade + Migrations |
| Azure Service Bus | Processamento assíncrono confiável |
| Repository Pattern | Abstração + facilita testes |

---

## ?? Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-feature`)
3. Commit (`git commit -m 'Add nova feature'`)
4. Push (`git push origin feature/nova-feature`)
5. Abra um Pull Request

---

## ????? Autor

**Leandro Drumond** - [GitHub](https://github.com/LeandroDrumond)

---

## ?? Referências

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)
- [EF Core Docs](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)

---

? **Se gostou, deixe uma estrela!**
