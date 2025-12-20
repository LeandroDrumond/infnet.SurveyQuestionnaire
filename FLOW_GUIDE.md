# Fluxo Completo — Guia Passo a Passo

Guia enxuto e completo para executar o fluxo end-to-end: criar usuários, criar e publicar questionário, adicionar questões, responder e consultar respostas.

---

Resumo dos passos:
1. Criar usuário Admin → salvar `adminUserId`
2. Criar usuário Public → salvar `publicUserId`
3. Criar questionário (Admin) → salvar `questionnaireId`
4. Adicionar questões (Admin) → salvar `questionId` e `optionId` quando aplicável
5. Publicar questionário (Admin)
6. Responder questionário (Public) → salvar `submissionId`
7. Aguardar processamento (Azure Function)
8. Consultar submission (detalhada) e listar respostas do questionário

---

1) Criar usuário Admin
- Endpoint: `POST /api/users`
- Body:
 ```json
 { "name": "Admin User", "email": "admin@domain.com", "userType": "Administrator" }
 ```
- Response:201 Created → copie `id` → variável: `adminUserId`

2) Criar usuário Public
- Endpoint: `POST /api/users`
- Body:
 ```json
 { "name": "João Silva", "email": "joao@domain.com", "userType": "Public" }
 ```
- Response:201 Created → copie `id` → variável: `publicUserId`

3) Criar questionário (Admin)
- Endpoint: `POST /api/questionnaires`
- Header: `X-User-Id: {adminUserId}`
- Body:
 ```json
 { "title": "Pesquisa2025", "description": "Avaliação" }
 ```
- Response:201 Created → copie `id` → variável: `questionnaireId`

4) Adicionar questões (Admin)
- Endpoint: `POST /api/questionnaires/{questionnaireId}/questions`
- Header: `X-User-Id: {adminUserId}`
- Exemplos:
 - Múltipla escolha:
 ```json
 {
 "text": "Como avalia o atendimento?",
 "isRequired": true,
 "isMultipleChoice": true,
 "options": [ { "text": "Excelente", "order":1 }, { "text": "Bom", "order":2 } ]
 }
 ```
 Response:200 OK → copie `questionId` e `optionId` das opções (ex.: `option1Id`).

 - Texto aberto (se for necessário):
 ```json
 { "text": "O que podemos melhorar?", "isRequired": false, "isMultipleChoice": false }
 ```
 Response:200 OK → copie `questionId` para uso em submissions.

Nota: salve `question1Id`, `question2Id`, `option1Id` etc. para usar nas submissões.

5) Publicar questionário (Admin)
- Endpoint: `POST /api/questionnaires/{questionnaireId}/publish`
- Header: `X-User-Id: {adminUserId}`
- Body:
 ```json
 { "collectionStart": "2025-01-01T00:00:00Z", "collectionEnd": "2025-12-31T23:59:59Z" }
 ```
- Response:200 OK → `status` passa para `Published`.

6) Criar submission (Public)
- Endpoint: `POST /api/submissions`
- Header: `X-User-Id: {publicUserId}`
- Regras importantes:
 - Para questões de múltipla escolha: `selectedOptionId` é obrigatório; o campo `answer` é opcional (pode ser vazio).
 - Para perguntas abertas: `answer` é obrigatório e `selectedOptionId` deve ser omitido ou nulo.
- Exemplo:
 ```json
 {
 "questionnaireId": "{questionnaireId}",
 "answers": [
 { "questionId": "{question1Id}", "answer": "", "selectedOptionId": "{option1Id}" },
 { "questionId": "{question2Id}", "answer": "Sugestão de melhoria." }
 ]
 }
 ```
- Response:202 Accepted → copie `submissionId` (status inicial `Pending`).

7) Rodar/aguardar Azure Function (processamento)
- A API publica mensagem na fila; a Function consome e grava `SubmissionItems` e atualiza `Submission` para `Completed`.
- Se estiver desenvolvendo localmente, execute a Function:
 ```bash
 cd infnet.SurveyQuestionnaire.Functions
 func start
 ```
- Aguarde alguns segundos e verifique o status da submission.

8) Consultar submission (detalhada)
- Endpoint: `GET /api/submissions/{submissionId}/items`
- Header: `X-User-Id: {publicUserId}` (ou `adminUserId` dependendo da regra de acesso)
- Response:200 OK → `items` com cada resposta processada.

9) Listar todas as submissions de um questionário (Admin)
- Endpoint: `GET /api/submissions/questionnaire/{questionnaireId}`
- Header: `X-User-Id: {adminUserId}`
- Response: lista de submissions (resumos).

10) Contar submissions de um questionário (Admin)
- Endpoint: `GET /api/submissions/questionnaire/{questionnaireId}/count`
- Header: `X-User-Id: {adminUserId}`
- Response: `{ "count": <number> }`

---

Boas práticas e notas rápidas:
- Use um environment no Postman para guardar as variáveis: `baseUrl`, `adminUserId`, `publicUserId`, `questionnaireId`, `question1Id`, `option1Id`, `submissionId`.
- Para testes locais, mantenha a Function rodando para processar mensagens em tempo real.
