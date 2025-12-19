namespace infnet.SurveyQuestionnaire.Domain.Exceptions;

public abstract class SubmissionDomainException(string message) : Exception(message)
{
}

public class QuestionnaireNotAvailableException(Guid questionnaireId, string reason) 
    : SubmissionDomainException($"Questionnaire with ID '{questionnaireId}' is not available for submission. Reason: {reason}")
{
}


public class DuplicateSubmissionException(Guid questionnaireId, Guid userId) 
    : SubmissionDomainException($"User '{userId}' has already submitted a response for questionnaire '{questionnaireId}'")
{
}

public class InvalidSubmissionException(string reason) 
    : SubmissionDomainException($"Submission is invalid. Reason: {reason}")
{
}

public class RequiredQuestionNotAnsweredException(Guid questionId) 
    : SubmissionDomainException($"Required question with ID '{questionId}' was not answered")
{
}

public class SubmissionNotFoundException(Guid submissionId) 
    : SubmissionDomainException($"Submission with ID '{submissionId}' not found")
{
}

public class OnlyPublicUsersCanSubmitException(Guid userId) 
    : SubmissionDomainException($"Only public users can submit responses. User '{userId}' is not a public user")
{
}
