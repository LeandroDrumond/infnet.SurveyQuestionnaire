# ?? Fluxo Completo da API - Guia Passo a Passo

Este guia explica o fluxo mínimo de uso da API, desde a criação de usuários até a consulta de respostas, com destaque: em perguntas de múltipla escolha o campo `answer` é opcional — basta `selectedOptionId`.

---

## Visão Geral (resumida)
1. Criar Usuários (Admin + Public)
2. Criar Questionário (Admin)
3. Adicionar Questões (Admin)
4. Publicar Questionário (Admin)
5. Responder Questionário (Public User)
6. Consultar Respostas (Admin)

---

## Pontos importantes sobre `answer` e múltipla escolha
- Para questões de **múltipla escolha** o que importa é `selectedOptionId`. O campo `answer` é opcional e pode ser enviado vazio.
- Para questões de **texto aberto** (`isMultipleChoice = false`) o campo `answer` é obrigatório.
- A validação no DTO aceita `answer` vazio quando `selectedOptionId` está presente; a entidade também permite `Answer` vazio nesses casos.

---

## Exemplo de Submission (múltipla escolha + texto aberto)

```json
{
 "questionnaireId": "c3d4e5f6-g7h8-9012-cdef-123456789012",
 "answers": [
 {
 "questionId": "d4e5f6g7-h8i9-0123-defg-234567890123",
 "answer": "", // opcional para múltipla escolha
 "selectedOptionId": "e5f6g7h8-i9j0-1234-efgh-345678901234"
 },
 {
 "questionId": "g7h8i9j0-k1l2-3456-ghij-567890123456",
 "answer": "Poderiam melhorar o tempo de resposta no suporte técnico." // obrigatório para texto aberto
 }
 ]
}
```

---

## Recomendações rápidas
- Ao criar a collection/postman, salve `adminUserId`, `publicUserId`, `questionnaireId`, `question1Id`, `option1Id`, `submissionId` como variáveis de ambiente.
- Sempre rode a Azure Function localmente (ou tenha um consumer) para que as submissions em `Pending` sejam processadas.
- Não use imagens em docs se não for subir os arquivos; prefira texto e exemplos JSON.

---

Pronto — `answer` deixou de ser obrigatório para questões de múltipla escolha. Verifique os exemplos e teste criando uma submission com `selectedOptionId` e `answer` vazio.
