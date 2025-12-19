using infnet.SurveyQuestionnaire.Domain.Common;

namespace infnet.SurveyQuestionnaire.Domain.Entities;

public sealed class Option : Entity
{
    private const int _textMinLength = 1;
    private const int _textMaxLength = 200;
    private const int _orderMin = 1;
    private const int _orderMax = 100;

    public string Text { get; private init; } = string.Empty;
    public int Order { get; private init; }
    public Guid QuestionId { get; internal set; }

    private Option()
    {
    }

    private Option(string text, int order) : base()
    {
        Text = ValidateAndTrimText(text);
        Order = ValidateOrder(order);
    }

    internal static Option Create(string text, int order)
    {
        return new Option(text, order);
    }

    internal void SetQuestionId(Guid questionId)
    {
        QuestionId = questionId;
    }

    private static string ValidateAndTrimText(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Option text cannot be empty", nameof(text));

        var trimmedText = text.Trim();

        if (trimmedText.Length < _textMinLength)
           throw new ArgumentException($"Option text must be at least {_textMinLength} character",nameof(text));

        if (trimmedText.Length > _textMaxLength)
            throw new ArgumentException($"Option text cannot exceed {_textMaxLength} characters",nameof(text));

        return trimmedText;
    }

    private static int ValidateOrder(int order)
    {
        if (order < _orderMin)
            throw new ArgumentException($"Order must be at least {_orderMin}",nameof(order));

        if (order > _orderMax)
          throw new ArgumentException($"Order cannot exceed {_orderMax}",nameof(order));

        return order;
    }
}
