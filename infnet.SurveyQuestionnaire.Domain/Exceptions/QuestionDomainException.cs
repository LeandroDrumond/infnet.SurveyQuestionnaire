

namespace infnet.SurveyQuestionnaire.Domain.Exceptions;

public abstract class QuestionDomainException(string message) : Exception(message)
{
}

public class QuestionNotReadyForPublicationException(Guid questionId, string reason)
    : QuestionDomainException($"Question with ID '{questionId}' is not ready for publication. Reason: {reason}")
{
}

public class QuestionCannotModifyException(Guid questionId)
    : QuestionDomainException($"Cannot modify question with ID '{questionId}' because the questionnaire is already published or closed")
{
}

public class QuestionNotFoundException(Guid questionId)
    : QuestionDomainException($"Question with ID '{questionId}' not found")
{
}

public class OptionAlreadyExistsException(Guid questionId, string optionText)
    : QuestionDomainException($"Option '{optionText}' already exists in question with ID '{questionId}'")
{
}

public class OptionNotFoundException(Guid optionId)
    : QuestionDomainException($"Option with ID '{optionId}' not found")
{
}
