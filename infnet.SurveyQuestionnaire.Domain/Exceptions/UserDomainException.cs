namespace infnet.SurveyQuestionnaire.Domain.Exceptions;

public abstract class UserDomainException(string message) : Exception(message)
{
}

public class UserAlreadyExistsException(string email) : UserDomainException($"User with email '{email}' already exists")
{
}

public class UserNotFoundException(Guid userId) : UserDomainException($"User with ID '{userId}' not found")
{
}

public class UserAlreadyActiveException(Guid userId) : UserDomainException($"User '{userId}' is already active")
{
}

public class UserAlreadyInactiveException(Guid userId) : UserDomainException($"User '{userId}' is already inactive")
{
}
