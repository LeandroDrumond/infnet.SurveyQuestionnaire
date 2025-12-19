using System;

namespace infnet.SurveyQuestionnaire.Domain.Questionnaires.Exceptions;

/// <summary>
/// Base exception for all questionnaire domain exceptions
/// </summary>
public abstract class QuestionnaireDomainException(string message) : Exception(message)
{
}

/// <summary>
/// Exception thrown when attempting to publish an already published questionnaire
/// </summary>
public class QuestionnaireAlreadyPublishedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' is already published")
{
}

/// <summary>
/// Exception thrown when attempting to close an already closed questionnaire
/// </summary>
public class QuestionnaireAlreadyClosedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' is already closed")
{
}

/// <summary>
/// Exception thrown when attempting an operation that requires a published questionnaire
/// </summary>
public class QuestionnaireNotPublishedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' is not published yet")
{
}

/// <summary>
/// Exception thrown when attempting to find a questionnaire that doesn't exist
/// </summary>
public class QuestionnaireNotFoundException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' not found")
{
}

/// <summary>
/// Exception thrown when collection period dates are invalid
/// </summary>
public class QuestionnaireInvalidCollectionPeriodException()
    : QuestionnaireDomainException("Collection end date must be after collection start date")
{
}

/// <summary>
/// Exception thrown when attempting to modify a published or closed questionnaire
/// </summary>
public class QuestionnaireCannotModifyPublishedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Cannot modify questionnaire with ID '{questionnaireId}' because it is already published or closed")
{
}
