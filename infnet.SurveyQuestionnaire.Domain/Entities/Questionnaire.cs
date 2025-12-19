using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Questionnaires.Exceptions;

namespace infnet.SurveyQuestionnaire.Domain.Entities;

/// <summary>
/// Apenas usuários administradores podem criar questionários
/// </summary>
public sealed class Questionnaire : Entity
{
    private const int _titleMinLength = 3;
    private const int _titleMaxLength = 200;
    private const int _descriptionMaxLength = 1000;

    private readonly List<Question> _questions = [];

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public QuestionnaireStatus Status { get; private set; }
    public DateTime? CollectionStart { get; private set; }
    public DateTime? CollectionEnd { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    /// <summary>
    /// Questões associadas ao questionário
    /// </summary>
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    private Questionnaire()
    {
    }

    private Questionnaire(string title, string description, Guid createdByUserId) : base()
    {
        SetTitle(title);
        SetDescription(description);
        CreatedByUserId = createdByUserId;
        Status = QuestionnaireStatus.Draft;
    }

    /// <summary>
    /// Cria um novo questionário
    /// </summary>
    public static Questionnaire Create(string title, string description, Guid createdByUserId)
    {
        return new Questionnaire(title, description, createdByUserId);
    }

    /// <summary>
    /// Adiciona uma questão ao questionário
    /// </summary>
    /// <returns>ID da questão criada</returns>
    public Guid AddQuestion(string text, bool isRequired, bool isMultipleChoice)
    {
        if (Status != QuestionnaireStatus.Draft)
            throw new QuestionnaireCannotModifyPublishedException(Id);

        var question = Question.Create(text, isRequired, isMultipleChoice);
        
        // ? SOLUÇÃO: Setar FK antes de adicionar à coleção
        question.SetQuestionnaireId(Id);
        
        _questions.Add(question);
        SetUpdatedAt();

        return question.Id;
    }

    /// <summary>
    /// Adiciona uma questão com opções ao questionário (para questões de múltipla escolha)
    /// </summary>
    /// <param name="text">Texto da questão</param>
    /// <param name="isRequired">Se a questão é obrigatória</param>
    /// <param name="options">Lista de opções (texto, ordem)</param>
    /// <returns>ID da questão criada</returns>
    public Guid AddQuestionWithOptions(string text, bool isRequired, IEnumerable<(string Text, int Order)> options)
    {
        if (Status != QuestionnaireStatus.Draft)
            throw new QuestionnaireCannotModifyPublishedException(Id);

        // Cria questão de múltipla escolha
        var question = Question.Create(text, isRequired, isMultipleChoice: true);
      
        // ? SOLUÇÃO: Setar FK antes de adicionar opções
        question.SetQuestionnaireId(Id);
   
        // Adiciona todas as opções
        foreach (var (optionText, order) in options)
        {
            question.AddOption(optionText, order);
        }
        
        _questions.Add(question);
        SetUpdatedAt();

        return question.Id;
    }

    /// <summary>
    /// Adiciona uma ou mais opções a uma questão existente
    /// </summary>
    /// <param name="questionId">ID da questão</param>
    /// <param name="options">Lista de opções (1 ou mais)</param>
    public void AddOptionsToQuestion(Guid questionId, IEnumerable<(string Text, int Order)> options)
    {
        if (Status != QuestionnaireStatus.Draft)
  throw new QuestionnaireCannotModifyPublishedException(Id);

        var question = _questions.FirstOrDefault(q => q.Id == questionId)
      ?? throw new InvalidOperationException($"Question with ID '{questionId}' not found in questionnaire");

foreach (var (text, order) in options)
        {
            question.AddOption(text, order);
        }
 
     SetUpdatedAt();
    }

    /// <summary>
    /// Remove uma questão do questionário
    /// </summary>
    public void RemoveQuestion(Guid questionId)
    {
        if (Status != QuestionnaireStatus.Draft)
            throw new QuestionnaireCannotModifyPublishedException(Id);

        var question = _questions.FirstOrDefault(q => q.Id == questionId)
            ?? throw new InvalidOperationException($"Question with ID '{questionId}' not found in questionnaire");

        _questions.Remove(question);
        SetUpdatedAt();
    }

    /// <summary>
    /// Atualiza uma questão (AGGREGATE ROOT METHOD)
    /// </summary>
    public void UpdateQuestion(Guid questionId, string text)
    {
        if (Status != QuestionnaireStatus.Draft)
            throw new QuestionnaireCannotModifyPublishedException(Id);

        var question = _questions.FirstOrDefault(q => q.Id == questionId)
            ?? throw new InvalidOperationException($"Question with ID '{questionId}' not found in questionnaire");

        question.UpdateText(text);
        SetUpdatedAt();
    }

    /// <summary>
    /// Remove uma opção de uma questão (AGGREGATE ROOT METHOD)
    /// </summary>
    public void RemoveOptionFromQuestion(Guid questionId, Guid optionId)
    {
        if (Status != QuestionnaireStatus.Draft)
            throw new QuestionnaireCannotModifyPublishedException(Id);

        var question = _questions.FirstOrDefault(q => q.Id == questionId) ?? throw new InvalidOperationException($"Question with ID '{questionId}' not found in questionnaire");

        question.RemoveOption(optionId);
        SetUpdatedAt();
    }

    /// <summary>
    /// Publica o questionário tornando-o disponível para coleta de respostas
    /// </summary>
    public void Publish(DateTime collectionStart, DateTime collectionEnd)
    {
        if (Status != QuestionnaireStatus.Draft)
            throw new QuestionnaireAlreadyPublishedException(Id);

        if (collectionEnd <= collectionStart)
            throw new QuestionnaireInvalidCollectionPeriodException();

        ValidateQuestionsForPublication();

        Status = QuestionnaireStatus.Published;
        CollectionStart = collectionStart;
        CollectionEnd = collectionEnd;
        SetUpdatedAt();
    }

    /// <summary>
    /// Fecha o questionário impedindo novas respostas
    /// </summary>
    public void Close()
    {
        if (Status == QuestionnaireStatus.Closed)
            throw new QuestionnaireAlreadyClosedException(Id);

        if (Status != QuestionnaireStatus.Published)
            throw new QuestionnaireNotPublishedException(Id);

        Status = QuestionnaireStatus.Closed;
        SetUpdatedAt();
    }

    /// <summary>
    /// Atualiza os dados básicos do questionário
    /// </summary>
    public void Update(string title, string description)
    {
        if (Status != QuestionnaireStatus.Draft)
            throw new QuestionnaireCannotModifyPublishedException(Id);

        SetTitle(title);
        SetDescription(description);
        SetUpdatedAt();
    }

    /// <summary>
    /// Verifica se o questionário está publicado
    /// </summary>
    public bool IsPublished() => Status == QuestionnaireStatus.Published;

    /// <summary>
    /// Verifica se o questionário está fechado
    /// </summary>
    public bool IsClosed() => Status == QuestionnaireStatus.Closed;

    /// <summary>
    /// Verifica se o questionário está dentro do período de coleta
    /// </summary>
    public bool IsWithinCollectionPeriod()
    {
        if (!IsPublished())
            return false;

        var now = DateTime.UtcNow;
        return CollectionStart.HasValue &&
               CollectionEnd.HasValue &&
               now >= CollectionStart.Value &&
               now <= CollectionEnd.Value;
    }

    /// <summary>
    /// Valida se todas as questões estão prontas para publicação
    /// </summary>
    private void ValidateQuestionsForPublication()
    {
        if (_questions.Count == 0)
            throw new InvalidOperationException("Cannot publish questionnaire without questions.");

        foreach (var question in _questions)
        {
            question.ValidateForPublication();
        }
    }
    
    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var trimmedTitle = title.Trim();

        if (trimmedTitle.Length < _titleMinLength)
             throw new ArgumentException($"Title must be at least {_titleMinLength} characters long",nameof(title));

        if (trimmedTitle.Length > _titleMaxLength)
            throw new ArgumentException( $"Title cannot exceed {_titleMaxLength} characters", nameof(title));

        Title = trimmedTitle;
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > _descriptionMaxLength)
            throw new ArgumentException($"Description cannot exceed {_descriptionMaxLength} characters",nameof(description));

        Description = trimmedDescription;
    }
}
