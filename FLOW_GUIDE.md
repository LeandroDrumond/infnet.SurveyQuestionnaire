# ?? Fluxo Completo da API - Guia Passo a Passo

Este guia explica o fluxo completo de uso da API, desde a criação de usuários até a consulta de respostas.

---

## ?? Visão Geral do Fluxo

```
1. Criar Usuários (Admin + Public)
   ?
2. Criar Questionário (Admin)
   ?
3. Adicionar Questões (Admin)
   ?
4. Publicar Questionário (Admin)
   ?
5. Responder Questionário (Public User)
   ?
6. Consultar Respostas (Admin)
```

---

## 1?? Criar Usuário Administrador

**Objetivo:** Criar o usuário que poderá criar e gerenciar questionários.

**Endpoint:**
```http
POST /api/users
```

**Body:**
```json
{
  "name": "Admin User",
  "email": "admin@surveyquest.com",
  "userType": "Administrator"
}
```

**Response (201 Created):**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "name": "Admin User",
  "email": "admin@surveyquest.com",
  "userType": "Administrator",
  "createdAt": "2024-12-19T10:00:00Z"
}
```

**? AÇÃO:** Copie o `id` retornado ? Este é o **`adminUserId`**

---

## 2?? Criar Usuário Público

**Objetivo:** Criar um usuário que poderá responder questionários.

**Endpoint:**
```http
POST /api/users
```

**Body:**
```json
{
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "userType": "Public"
}
```

**Response (201 Created):**
```json
{
  "id": "b2c3d4e5-f6g7-8901-bcde-f12345678901",
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "userType": "Public",
  "createdAt": "2024-12-19T10:01:00Z"
}
```

**? AÇÃO:** Copie o `id` retornado ? Este é o **`publicUserId`**

---

## 3?? Criar Questionário

**Objetivo:** Administrador cria um novo questionário (inicialmente em status `Draft`).

**Endpoint:**
```http
POST /api/questionnaires
```

**Headers:**
```
X-User-Id: {adminUserId}  ? Use o ID do usuário Admin
```

**Body:**
```json
{
  "title": "Pesquisa de Satisfação 2024",
  "description": "Pesquisa para avaliar a satisfação dos clientes"
}
```

**Response (201 Created):**
```json
{
  "id": "c3d4e5f6-g7h8-9012-cdef-123456789012",
  "title": "Pesquisa de Satisfação 2024",
  "description": "Pesquisa para avaliar a satisfação dos clientes",
  "status": "Draft",
  "createdByUserId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "createdAt": "2024-12-19T10:02:00Z",
  "questions": null
}
```

**? AÇÃO:** Copie o `id` retornado ? Este é o **`questionnaireId`**

---

## 4?? Adicionar Questão (Múltipla Escolha)

**Objetivo:** Adicionar uma questão de múltipla escolha ao questionário.

**Endpoint:**
```http
POST /api/questionnaires/{questionnaireId}/questions
  ?
             ?? Cole aqui o questionnaireId
```

**Exemplo de URL:**
```
POST /api/questionnaires/c3d4e5f6-g7h8-9012-cdef-123456789012/questions
```

**Headers:**
```
X-User-Id: {adminUserId}  ? Use o ID do Admin
```

**Body:**
```json
{
  "text": "Como você avalia nosso atendimento?",
  "isRequired": true,
  "isMultipleChoice": true,
  "options": [
    {
      "text": "Excelente",
      "order": 1
    },
    {
      "text": "Bom",
      "order": 2
    },
    {
      "text": "Regular",
      "order": 3
    },
    {
      "text": "Ruim",
      "order": 4
    }
  ]
}
```

**Response (200 OK):**
```json
{
  "id": "d4e5f6g7-h8i9-0123-defg-234567890123",
  "text": "Como você avalia nosso atendimento?",
  "isRequired": true,
  "isMultipleChoice": true,
  "createdAt": "2024-12-19T10:03:00Z",
  "options": [
    {
   "id": "e5f6g7h8-i9j0-1234-efgh-345678901234",
      "text": "Excelente",
      "order": 1
    },
    {
      "id": "f6g7h8i9-j0k1-2345-fghi-456789012345",
      "text": "Bom",
      "order": 2
    },
    // ...
]
}
```

**? AÇÃO:** 
- Copie o `id` da questão ? Este é o **`question1Id`**
- Copie o `id` da primeira opção ("Excelente") ? Este é o **`option1Id`**

---

## 5?? Adicionar Questão (Texto Aberto)

**Objetivo:** Adicionar uma questão de texto livre.

**Endpoint:**
```http
POST /api/questionnaires/{questionnaireId}/questions
      ?
            ?? Use o mesmo questionnaireId
```

**Headers:**
```
X-User-Id: {adminUserId}
```

**Body:**
```json
{
  "text": "O que podemos melhorar?",
  "isRequired": false,
  "isMultipleChoice": false
}
```

**Response (200 OK):**
```json
{
  "id": "g7h8i9j0-k1l2-3456-ghij-567890123456",
  "text": "O que podemos melhorar?",
  "isRequired": false,
  "isMultipleChoice": false,
  "createdAt": "2024-12-19T10:04:00Z",
  "options": null
}
```

**? AÇÃO:** Copie o `id` ? Este é o **`question2Id`**

---

## 6?? Publicar Questionário

**Objetivo:** Publicar o questionário para que usuários públicos possam respondê-lo.

**Endpoint:**
```http
POST /api/questionnaires/{questionnaireId}/publish
           ?
     ?? Use o questionnaireId
```

**Exemplo de URL:**
```
POST /api/questionnaires/c3d4e5f6-g7h8-9012-cdef-123456789012/publish
```

**Headers:**
```
X-User-Id: {adminUserId}
```

**Body:**
```json
{
  "collectionStart": "2024-01-01T00:00:00Z",
  "collectionEnd": "2024-12-31T23:59:59Z"
}
```

**Response (200 OK):**
```json
{
  "id": "c3d4e5f6-g7h8-9012-cdef-123456789012",
  "title": "Pesquisa de Satisfação 2024",
  "status": "Published",  ? Status mudou!
  "collectionStart": "2024-01-01T00:00:00Z",
  "collectionEnd": "2024-12-31T23:59:59Z"
}
```

**?? Agora o questionário está disponível para resposta!**

---

## 7?? Listar Questionários Publicados

**Objetivo:** Ver quais questionários estão disponíveis para resposta.

**Endpoint:**
```http
GET /api/questionnaires/published
```

**Headers:**
```
Nenhum header necessário (endpoint público)
```

**Response (200 OK):**
```json
[
  {
    "id": "c3d4e5f6-g7h8-9012-cdef-123456789012",
    "title": "Pesquisa de Satisfação 2024",
    "description": "Pesquisa para avaliar a satisfação dos clientes",
    "status": "Published",
    "collectionStart": "2024-01-01T00:00:00Z",
    "collectionEnd": "2024-12-31T23:59:59Z",
    "questionCount": 2,
    "createdAt": "2024-12-19T10:02:00Z"
  }
]
```

---

## 8?? Criar Submission (Responder Questionário)

**Objetivo:** Usuário público responde o questionário.

**Endpoint:**
```http
POST /api/submissions
```

**Headers:**
```
X-User-Id: {publicUserId}  ? Use o ID do usuário Public
```

**Body:**
```json
{
  "questionnaireId": "{questionnaireId}",
 ?
          ?? Cole o ID do questionário
  
  "answers": [
    {
      "questionId": "{question1Id}",
        ?
         ?? Cole o ID da primeira questão
      
      "answer": "Excelente",
      "selectedOptionId": "{option1Id}"
            ?
          ?? Cole o ID da opção "Excelente"
    },
    {
      "questionId": "{question2Id}",
      ?
       ?? Cole o ID da segunda questão
      
 "answer": "Poderiam melhorar o tempo de resposta no suporte técnico."
    }
  ]
}
```

**Exemplo com IDs Preenchidos:**
```json
{
  "questionnaireId": "c3d4e5f6-g7h8-9012-cdef-123456789012",
  "answers": [
    {
      "questionId": "d4e5f6g7-h8i9-0123-defg-234567890123",
      "answer": "Excelente",
      "selectedOptionId": "e5f6g7h8-i9j0-1234-efgh-345678901234"
    },
    {
      "questionId": "g7h8i9j0-k1l2-3456-ghij-567890123456",
      "answer": "Poderiam melhorar o tempo de resposta no suporte técnico."
    }
  ]
}
```

**Response (202 Accepted):**
```json
{
"id": "h8i9j0k1-l2m3-4567-hijk-678901234567",
  "questionnaireId": "c3d4e5f6-g7h8-9012-cdef-123456789012",
  "respondentUserId": "b2c3d4e5-f6g7-8901-bcde-f12345678901",
  "status": "Pending",  ? Aguardando processamento assíncrono
  "submittedAt": "2024-12-19T10:05:00Z",
  "failureReason": null,
  "items": null
}
```

**? AÇÃO:** Copie o `id` ? Este é o **`submissionId`**

**? IMPORTANTE:** 
- Status inicial: `Pending`
- A Azure Function irá processar a submission em segundo plano
- Após processamento, status muda para `Completed`
- Aguarde alguns segundos antes de consultar os detalhes

---

## 9?? Consultar Submission (Com Detalhes)

**Objetivo:** Ver a submission com todas as respostas processadas.

**Endpoint:**
```http
GET /api/submissions/{submissionId}/items
   ?
         ?? Use o submissionId
```

**Exemplo de URL:**
```
GET /api/submissions/h8i9j0k1-l2m3-4567-hijk-678901234567/items
```

**Headers:**
```
X-User-Id: {publicUserId}  ? Use o ID do usuário que respondeu
```

**Response (200 OK):**
```json
{
  "id": "h8i9j0k1-l2m3-4567-hijk-678901234567",
  "questionnaireId": "c3d4e5f6-g7h8-9012-cdef-123456789012",
  "respondentUserId": "b2c3d4e5-f6g7-8901-bcde-f12345678901",
  "status": "Completed",  ? Processamento concluído!
  "submittedAt": "2024-12-19T10:05:00Z",
  "failureReason": null,
  "items": [
    {
   "id": "i9j0k1l2-m3n4-5678-ijkl-789012345678",
      "questionId": "d4e5f6g7-h8i9-0123-defg-234567890123",
      "questionText": "",
      "answer": "Excelente",
      "selectedOptionId": "e5f6g7h8-i9j0-1234-efgh-345678901234",
      "selectedOptionText": null
    },
    {
      "id": "j0k1l2m3-n4o5-6789-jklm-890123456789",
    "questionId": "g7h8i9j0-k1l2-3456-ghij-567890123456",
      "questionText": "",
      "answer": "Poderiam melhorar o tempo de resposta no suporte técnico.",
      "selectedOptionId": null,
      "selectedOptionText": null
    }
  ]
}
```

---

## ?? Listar Todas as Respostas de um Questionário (Admin)

**Objetivo:** Administrador consulta todas as respostas de um questionário.

**Endpoint:**
```http
GET /api/submissions/questionnaire/{questionnaireId}
       ?
          ?? Use o questionnaireId
```

**Exemplo de URL:**
```
GET /api/submissions/questionnaire/c3d4e5f6-g7h8-9012-cdef-123456789012
```

**Headers:**
```
X-User-Id: {adminUserId}  ? Use o ID do Admin
```

**Response (200 OK):**
```json
[
  {
 "id": "h8i9j0k1-l2m3-4567-hijk-678901234567",
 "questionnaireId": "c3d4e5f6-g7h8-9012-cdef-123456789012",
    "questionnaireTitle": "Pesquisa de Satisfação 2024",
    "status": "Completed",
    "submittedAt": "2024-12-19T10:05:00Z"
  },
  {
    "id": "k1l2m3n4-o5p6-7890-klmn-901234567890",
    "questionnaireId": "c3d4e5f6-g7h8-9012-cdef-123456789012",
    "questionnaireTitle": "Pesquisa de Satisfação 2024",
    "status": "Completed",
    "submittedAt": "2024-12-19T10:10:00Z"
  }
]
```

---

## 1??1?? Contar Respostas de um Questionário

**Objetivo:** Saber quantas pessoas responderam o questionário.

**Endpoint:**
```http
GET /api/submissions/questionnaire/{questionnaireId}/count
          ?
     ?? Use o questionnaireId
```

**Example de URL:**
```
GET /api/submissions/questionnaire/c3d4e5f6-g7h8-9012-cdef-123456789012/count
```

**Headers:**
```
X-User-Id: {adminUserId}
```

**Response (200 OK):**
```json
{
  "count": 42
}
```

---

## ?? Resumo dos IDs Importantes

Durante o fluxo, você vai coletar estes IDs:

| Nome da Variável | Obtido em | Usado em |
|------------------|-----------|----------|
| `adminUserId` | Passo 1 (Criar Admin) | Headers de todas as operações de Admin |
| `publicUserId` | Passo 2 (Criar Public User) | Header ao criar Submission |
| `questionnaireId` | Passo 3 (Criar Questionário) | URLs de adicionar questões, publicar, listar respostas |
| `question1Id` | Passo 4 (Adicionar Questão 1) | Body ao criar Submission |
| `question2Id` | Passo 5 (Adicionar Questão 2) | Body ao criar Submission |
| `option1Id` | Passo 4 (Opção da Questão 1) | Body ao criar Submission |
| `submissionId` | Passo 8 (Criar Submission) | URL para consultar detalhes |

---

## ?? Dicas Importantes

### ?? Sempre use Headers:
- `X-User-Id: {adminUserId}` ? Para operações de administrador
- `X-User-Id: {publicUserId}` ? Para criar submissions

### ?? Processamento Assíncrono:
Quando você cria uma Submission:
1. API retorna `202 Accepted` com status `Pending`
2. Mensagem é enviada para Azure Service Bus
3. Azure Function processa em background
4. Status muda para `Completed`
5. **Aguarde 2-5 segundos** antes de consultar os items

### ? Status de Submission:
- `Pending` ? Aguardando processamento
- `Processing` ? Azure Function está processando
- `Completed` ? ? Processado com sucesso
- `Failed` ? ? Erro no processamento

### ?? Como verificar se funcionou:
```
1. Criar Submission ? Status: Pending
2. Aguardar 5 segundos
3. GET /api/submissions/{id}/items ? Status deve ser: Completed
4. Se ainda Processing ? aguardar mais alguns segundos
5. Se Failed ? verificar logs da Function
```

---

## ?? Fluxo Completo em Resumo

```
Admin User (Step 1)
    ? adminUserId
Public User (Step 2)
    ? publicUserId
Questionnaire (Step 3)
    ? questionnaireId
Add Questions (Steps 4-5)
    ? question1Id, question2Id, option1Id
Publish (Step 6)
    ? Status: Published
Create Submission (Step 8)
    ? submissionId, Status: Pending
Azure Function Processing
    ? Status: Processing ? Completed
View Results (Steps 9-11)
    ? Success!
```

**Pronto! Agora você sabe exatamente qual ID usar em cada endpoint!** ??
