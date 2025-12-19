using System.Text.RegularExpressions;
using infnet.SurveyQuestionnaire.Domain.Common;

namespace infnet.SurveyQuestionnaire.Domain.Users.ValueObjects;

public sealed class Email : ValueObject
{
    private const int _maxLength = 200;
    private static readonly Regex _emailRegex = new(
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > _maxLength)
        {
            throw new ArgumentException($"Email cannot exceed {_maxLength} characters", nameof(email));
        }

        if (!_emailRegex.IsMatch(normalizedEmail))
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }

        return new Email(normalizedEmail);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
