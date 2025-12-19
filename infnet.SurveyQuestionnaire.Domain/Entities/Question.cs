using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Exceptions;

namespace infnet.SurveyQuestionnaire.Domain.Entities;

/// <summary>
/// Representa uma questão de um questionário - INTERNAL ENTITY
/// 
/// ?? Esta é uma ENTITY INTERNA do agregado Questionnaire
/// - Não pode ser acessada diretamente de fora do agregado
/// - Só pode ser modificada através do Questionnaire (Aggregate Root)
/// - Não tem repository próprio
/// </summary>
public sealed class Question : Entity
{
    private const int _textMinLength = 3;
    private const int _textMaxLength = 500;

    private readonly List<Option> _options = new();

    public string Text { get; private set; } = string.Empty;
    public bool IsRequired { get; private set; }
    public bool IsMultipleChoice { get; private set; }
    
    // ? Foreign Key (necessário para EF Core)
    public Guid QuestionnaireId { get; private set; }

    /// <summary>
    /// Opções de resposta (apenas para questões de múltipla escolha)
    /// </summary>
    public IReadOnlyCollection<Option> Options => _options.AsReadOnly();

    // EF Constructor
    private Question()
    {
    }

    private Question(string text, bool isRequired, bool isMultipleChoice) : base()
    {
        SetText(text);
        IsRequired = isRequired;
        IsMultipleChoice = isMultipleChoice;
    }

    /// <summary>
    /// Cria uma nova questão (INTERNAL - só chamado pelo Aggregate Root)
    /// </summary>
    internal static Question Create(string text, bool isRequired, bool isMultipleChoice)
    {
        return new Question(text, isRequired, isMultipleChoice);
    }

    /// <summary>
    /// Adiciona uma opção (INTERNAL - só chamado pelo Aggregate Root)
    /// </summary>
    internal void AddOption(string text, int order)
    {
        if (!IsMultipleChoice)
            throw new InvalidOperationException("Cannot add options to non-multiple choice question");

        // Validar duplicação de order
        if (_options.Any(o => o.Order == order))
            throw new OptionAlreadyExistsException(Id, $"Option with order {order}");

        // Validar duplicação de texto
        if (_options.Any(o => o.Text.Equals(text, StringComparison.OrdinalIgnoreCase)))
            throw new OptionAlreadyExistsException(Id, text);

        var option = Option.Create(text, order);
        _options.Add(option);
    }

    /// <summary>
    /// Remove uma opção (INTERNAL - só chamado pelo Aggregate Root)
    /// </summary>
    internal void RemoveOption(Guid optionId)
    {
        var option = _options.FirstOrDefault(o => o.Id == optionId)
                ?? throw new InvalidOperationException($"Option with ID '{optionId}' not found");

        _options.Remove(option);
    }

    /// <summary>
    /// Atualiza o texto da questão (INTERNAL - só chamado pelo Aggregate Root)
    /// </summary>
    internal void UpdateText(string text)
    {
        SetText(text);
    }

    /// <summary>
    /// Valida se a questão está pronta para publicação (INTERNAL)
    /// </summary>
    internal void ValidateForPublication()
    {
        if (IsMultipleChoice && _options.Count < 2)
            throw new QuestionNotReadyForPublicationException(
        Id,
          "Multiple choice questions must have at least 2 options");

        if (IsMultipleChoice && _options.Count == 0)
            throw new QuestionNotReadyForPublicationException(
        Id,
                "Multiple choice questions must have options");
    }

    private void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));

        var trimmedText = text.Trim();

        if (trimmedText.Length < _textMinLength)
            throw new ArgumentException(
                $"Question text must be at least {_textMinLength} characters",
                 nameof(text));

        if (trimmedText.Length > _textMaxLength)
            throw new ArgumentException(
      $"Question text cannot exceed {_textMaxLength} characters",
         nameof(text));

        Text = trimmedText;
    }
}
