namespace infnet.SurveyQuestionnaire.Domain.Exceptions;

/// <summary>
/// Base exception for all submission domain exceptions
/// </summary>
public abstract class SubmissionDomainException(string message) : Exception(message)
{
}

/// <summary>
/// Exception thrown when questionnaire is not available for submission
/// </summary>
public class QuestionnaireNotAvailableException(Guid questionnaireId, string reason) 
    : SubmissionDomainException($"Questionnaire with ID '{questionnaireId}' is not available for submission. Reason: {reason}")
{
}

/// <summary>
/// Exception thrown when user has already submitted response
/// </summary>
public class DuplicateSubmissionException(Guid questionnaireId, Guid userId) 
    : SubmissionDomainException($"User '{userId}' has already submitted a response for questionnaire '{questionnaireId}'")
{
}

/// <summary>
/// Exception thrown when submission is invalid
/// </summary>
public class InvalidSubmissionException(string reason) 
    : SubmissionDomainException($"Submission is invalid. Reason: {reason}")
{
}

/// <summary>
/// Exception thrown when required question is not answered
/// </summary>
public class RequiredQuestionNotAnsweredException(Guid questionId) 
    : SubmissionDomainException($"Required question with ID '{questionId}' was not answered")
{
}

/// <summary>
/// Exception thrown when attempting to find a submission that doesn't exist
/// </summary>
public class SubmissionNotFoundException(Guid submissionId) 
    : SubmissionDomainException($"Submission with ID '{submissionId}' not found")
{
}

/// <summary>
/// Exception thrown when only public users can submit
/// </summary>
public class OnlyPublicUsersCanSubmitException(Guid userId) 
    : SubmissionDomainException($"Only public users can submit responses. User '{userId}' is not a public user")
{
}
