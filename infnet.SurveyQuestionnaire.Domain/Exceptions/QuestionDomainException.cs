using System;

namespace infnet.SurveyQuestionnaire.Domain.Exceptions;

/// <summary>
/// Base exception for all question domain exceptions
/// </summary>
public abstract class QuestionDomainException(string message) : Exception(message)
{
}

/// <summary>
/// Exception thrown when a question is not ready for publication
/// </summary>
public class QuestionNotReadyForPublicationException(Guid questionId, string reason)
    : QuestionDomainException($"Question with ID '{questionId}' is not ready for publication. Reason: {reason}")
{
}

/// <summary>
/// Exception thrown when attempting to modify a question in a published or closed questionnaire
/// </summary>
public class QuestionCannotModifyException(Guid questionId)
    : QuestionDomainException($"Cannot modify question with ID '{questionId}' because the questionnaire is already published or closed")
{
}

/// <summary>
/// Exception thrown when attempting to find a question that doesn't exist
/// </summary>
public class QuestionNotFoundException(Guid questionId)
    : QuestionDomainException($"Question with ID '{questionId}' not found")
{
}

/// <summary>
/// Exception thrown when attempting to add a duplicate option to a question
/// </summary>
public class OptionAlreadyExistsException(Guid questionId, string optionText)
    : QuestionDomainException($"Option '{optionText}' already exists in question with ID '{questionId}'")
{
}

/// <summary>
/// Exception thrown when attempting to find an option that doesn't exist
/// </summary>
public class OptionNotFoundException(Guid optionId)
    : QuestionDomainException($"Option with ID '{optionId}' not found")
{
}
