using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Exceptions;

namespace infnet.SurveyQuestionnaire.Domain.Entities;

public sealed class Question : Entity
{
    private const int _textMinLength = 3;
    private const int _textMaxLength = 500;

    private readonly List<Option> _options = new();

    public string Text { get; private set; } = string.Empty;
    public bool IsRequired { get; private set; }
    public bool IsMultipleChoice { get; private set; }
    public Guid QuestionnaireId { get; internal set; }
    public IReadOnlyCollection<Option> Options => _options.AsReadOnly();

    private Question()
    {
    }

    private Question(string text, bool isRequired, bool isMultipleChoice) : base()
    {
        SetText(text);
        IsRequired = isRequired;
        IsMultipleChoice = isMultipleChoice;
    }

    internal static Question Create(string text, bool isRequired, bool isMultipleChoice)
    {
        return new Question(text, isRequired, isMultipleChoice);
    }

    internal void SetQuestionnaireId(Guid questionnaireId)
    {
        QuestionnaireId = questionnaireId;
    }

    internal void AddOption(string text, int order)
    {
        if (!IsMultipleChoice)
            throw new InvalidOperationException("Cannot add options to non-multiple choice question");

        if (_options.Any(o => o.Order == order))
            throw new OptionAlreadyExistsException(Id, $"Option with order {order}");

        if (_options.Any(o => o.Text.Equals(text, StringComparison.OrdinalIgnoreCase)))
            throw new OptionAlreadyExistsException(Id, text);

        var option = Option.Create(text, order);
    
        option.SetQuestionId(Id);
   
        _options.Add(option);
    }

    internal void RemoveOption(Guid optionId)
    {
        var option = _options.FirstOrDefault(o => o.Id == optionId)
                ?? throw new InvalidOperationException($"Option with ID '{optionId}' not found");

        _options.Remove(option);
    }

    internal void UpdateText(string text)
    {
        SetText(text);
    }

    internal void ValidateForPublication()
    {
        if (IsMultipleChoice && _options.Count < 2)
            throw new QuestionNotReadyForPublicationException(Id,"Multiple choice questions must have at least 2 options");

        if (IsMultipleChoice && _options.Count == 0)
            throw new QuestionNotReadyForPublicationException(Id, "Multiple choice questions must have options");
    }

    private void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));

        var trimmedText = text.Trim();

        if (trimmedText.Length < _textMinLength)
            throw new ArgumentException($"Question text must be at least {_textMinLength} characters",nameof(text));

        if (trimmedText.Length > _textMaxLength)
            throw new ArgumentException($"Question text cannot exceed {_textMaxLength} characters",nameof(text));

        Text = trimmedText;
    }
}
