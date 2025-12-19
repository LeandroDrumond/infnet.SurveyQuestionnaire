
namespace infnet.SurveyQuestionnaire.Domain.Questionnaires.Exceptions;

public abstract class QuestionnaireDomainException(string message) : Exception(message)
{
}

public class QuestionnaireAlreadyPublishedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' is already published")
{
}

public class QuestionnaireAlreadyClosedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' is already closed")
{
}

public class QuestionnaireNotPublishedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' is not published yet")
{
}

public class QuestionnaireNotFoundException(Guid questionnaireId)
    : QuestionnaireDomainException($"Questionnaire with ID '{questionnaireId}' not found")
{
}

public class QuestionnaireInvalidCollectionPeriodException()
    : QuestionnaireDomainException("Collection end date must be after collection start date")
{
}

public class QuestionnaireCannotModifyPublishedException(Guid questionnaireId)
    : QuestionnaireDomainException($"Cannot modify questionnaire with ID '{questionnaireId}' because it is already published or closed")
{
}
