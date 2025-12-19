@echo off
chcp 65001 >nul
REM 🔐 Script para Criar Repositório Limpo (Sem Histórico Comprometido)

echo.
echo ============================================
echo 🔐 Criando Repositório Limpo
echo ============================================
echo.

REM Verificar se estamos no diretório correto
if not exist ".git" (
    echo ❌ ERRO: Não estamos em um repositório Git
    echo Execute este script na pasta raiz do projeto
    pause
    exit /b 1
)

echo ⚠️  ATENÇÃO: Este script vai recriar TODO o histórico do Git
echo.
echo O que será feito:
echo  1. Criar branch nova sem histórico
echo  2. Adicionar todos os arquivos atuais
echo  3. Criar 1 commit limpo
echo  4. Substituir branch master
echo  5. Preparar para force push
echo.
echo ✅ Seu código NÃO será perdido
echo ❌ Histórico de commits será perdido
echo.

set /p confirm="Deseja continuar? (S/N): "
if /i not "%confirm%"=="S" (
    echo.
    echo ❌ Operação cancelada
    echo.
    pause
    exit /b 0
)

echo.
echo ============================================
echo 📝 Executando Limpeza
echo ============================================
echo.

REM Passo 1: Salvar estado atual
echo 📝 Passo 1/7: Salvando estado atual...
git stash push -m "Backup antes de limpar histórico"
if %errorlevel% neq 0 (
    echo ⚠️  Nada para salvar no stash
)

REM Passo 2: Criar branch órfã (sem histórico)
echo 📝 Passo 2/7: Criando branch limpa...
git checkout --orphan temp-clean-branch
if %errorlevel% neq 0 goto error

REM Passo 3: Adicionar todos os arquivos
echo 📝 Passo 3/7: Adicionando arquivos...
git add -A
if %errorlevel% neq 0 goto error

REM Passo 4: Criar commit inicial
echo 📝 Passo 4/7: Criando commit limpo...
git commit -m "chore: initial commit - clean repository without secrets in history"
if %errorlevel% neq 0 goto error

REM Passo 5: Deletar branch master antiga
echo 📝 Passo 5/7: Removendo branch antiga...
git branch -D master
if %errorlevel% neq 0 (
    echo ⚠️  Branch master não existe ou já foi removida
)

REM Passo 6: Renomear nova branch para master
echo 📝 Passo 6/7: Renomeando branch...
git branch -m master
if %errorlevel% neq 0 goto error

REM Passo 7: Restaurar stash se houver
echo 📝 Passo 7/7: Verificando stash...
git stash list | find "Backup antes de limpar histórico" >nul
if %errorlevel% equ 0 (
    echo ✅ Restaurando estado salvo...
    git stash pop
)

echo.
echo ============================================
echo ✅ Repositório Limpo Criado!
echo ============================================
echo.
echo 🎉 Sucesso! Agora você tem:
echo   - 1 commit limpo (sem secrets no histórico)
echo   - Todo seu código preservado
echo   - Pronto para fazer push
echo.
echo ============================================
echo 🚀 Próximo Passo: Force Push
echo ============================================
echo.
echo Execute os comandos abaixo para enviar ao GitHub:
echo.
echo   git remote add origin https://github.com/LeandroDrumond/infnet.SurveyQuestionnaire.git
echo   git push origin master --force
echo.
echo ⚠️  IMPORTANTE:
echo   - O force push vai substituir TODO o histórico no GitHub
echo   - Outros devs precisarão clonar o repo novamente
echo   - Depois do push, os secrets não estarão mais no histórico
echo.
echo ============================================
echo 📊 Status Atual
echo ============================================
echo.
git log --oneline --graph --all -10
echo.
echo ✅ Tudo pronto!
echo.
goto end

:error
echo.
echo ============================================
echo ❌ Erro Durante Execução
echo ============================================
echo.
echo Algo deu errado. Possíveis soluções:
echo.
echo 1. Verificar se há alterações não commitadas:
echo    git status
echo.
echo 2. Tentar manualmente:
echo    git checkout --orphan temp-clean-branch
echo git add -A
echo    git commit -m "chore: initial commit"
echo    git branch -D master
echo    git branch -m master
echo.
goto end

:end
echo.
pause
