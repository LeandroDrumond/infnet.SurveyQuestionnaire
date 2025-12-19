using System.Net;
using System.Text.Json;
using FluentValidation;
using infnet.SurveyQuestionnaire.Api.Models;
using infnet.SurveyQuestionnaire.Domain.Questionnaires.Exceptions;
using infnet.SurveyQuestionnaire.Domain.Exceptions;

namespace infnet.SurveyQuestionnaire.Api.Middleware;

/// <summary>
/// Middleware para tratamento centralizado de exceções
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            // ==================== Validation Exceptions ====================
            ValidationException validationException =>
                CreateValidationErrorResponse(context, validationException),

            // ==================== User Exceptions ====================
            UserNotFoundException notFoundEx =>
                CreateErrorResponse(context, HttpStatusCode.NotFound, "User Not Found", notFoundEx.Message),

            UserAlreadyExistsException conflictEx =>
                CreateErrorResponse(context, HttpStatusCode.Conflict, "User Already Exists", conflictEx.Message),

            UserAlreadyActiveException or UserAlreadyInactiveException =>
                CreateErrorResponse(context, HttpStatusCode.BadRequest, "Invalid User Operation", exception.Message),

            // ==================== Questionnaire Exceptions ====================
            QuestionnaireNotFoundException notFoundEx =>
                CreateErrorResponse(context, HttpStatusCode.NotFound, "Questionnaire Not Found", notFoundEx.Message),

            QuestionnaireAlreadyPublishedException or
            QuestionnaireAlreadyClosedException or
            QuestionnaireNotPublishedException or
            QuestionnaireCannotModifyPublishedException =>
                CreateErrorResponse(context, HttpStatusCode.BadRequest, "Invalid Questionnaire Operation", exception.Message),

            QuestionnaireInvalidCollectionPeriodException =>
                CreateErrorResponse(context, HttpStatusCode.BadRequest, "Invalid Collection Period", exception.Message),

            // ==================== Question Exceptions ====================
            QuestionNotFoundException or OptionNotFoundException =>
                CreateErrorResponse(context, HttpStatusCode.NotFound, "Resource Not Found", exception.Message),

            QuestionNotReadyForPublicationException or
            QuestionCannotModifyException =>
                CreateErrorResponse(context, HttpStatusCode.BadRequest, "Invalid Question Operation", exception.Message),

            OptionAlreadyExistsException =>
                CreateErrorResponse(context, HttpStatusCode.Conflict, "Option Already Exists", exception.Message),

            // ==================== Submission Exceptions ====================
            SubmissionNotFoundException =>
                CreateErrorResponse(context, HttpStatusCode.NotFound, "Submission Not Found", exception.Message),

            QuestionnaireNotAvailableException =>
                CreateErrorResponse(context, HttpStatusCode.BadRequest, "Questionnaire Not Available", exception.Message),

            DuplicateSubmissionException =>
                CreateErrorResponse(context, HttpStatusCode.Conflict, "Duplicate Submission", exception.Message),

            InvalidSubmissionException or RequiredQuestionNotAnsweredException =>
                CreateErrorResponse(context, HttpStatusCode.BadRequest, "Invalid Submission", exception.Message),

            OnlyPublicUsersCanSubmitException =>
                CreateErrorResponse(context, HttpStatusCode.Forbidden, "Only Public Users Can Submit", exception.Message),

            // ==================== KeyNotFoundException (404) ====================
            KeyNotFoundException notFoundEx =>
                CreateErrorResponse(context, HttpStatusCode.NotFound, "Resource Not Found", notFoundEx.Message),

            // ==================== UnauthorizedAccessException (401/403) ====================
            UnauthorizedAccessException unauthorizedEx =>
                CreateErrorResponse(context, HttpStatusCode.Forbidden, "Access Forbidden", unauthorizedEx.Message),

            // ==================== Generic Exceptions ====================
            ArgumentException or ArgumentNullException =>
                CreateErrorResponse(context, HttpStatusCode.BadRequest, "Bad Request", exception.Message),

            NotImplementedException notImplementedEx =>
                CreateErrorResponse(context, HttpStatusCode.NotImplemented, "Not Implemented", notImplementedEx.Message),

            // ==================== Fallback ====================
            _ => CreateErrorResponse(context, HttpStatusCode.InternalServerError, "Internal Server Error",
                "An unexpected error occurred. Please try again later.")
        };

        // Log exception
        LogException(exception, context);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        var result = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(result);
    }

    private static ErrorResponse CreateValidationErrorResponse(HttpContext context, ValidationException validationException)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new ErrorResponse
        {
            Title = "One or more validation errors occurred.",
            Status = (int)HttpStatusCode.BadRequest,
            Errors = errors,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            TraceId = context.TraceIdentifier
        };
    }

    private static ErrorResponse CreateErrorResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string? detail = null)
    {
        context.Response.StatusCode = (int)statusCode;

        return new ErrorResponse
        {
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Type = GetRfcLink(statusCode),
            TraceId = context.TraceIdentifier
        };
    }

    private void LogException(Exception exception, HttpContext context)
    {
        var logLevel = exception switch
        {
            // Warning: Expected business/validation errors
            ValidationException => LogLevel.Warning,

            // User exceptions
            UserNotFoundException => LogLevel.Warning,
            UserAlreadyExistsException => LogLevel.Warning,
            UserAlreadyActiveException => LogLevel.Warning,
            UserAlreadyInactiveException => LogLevel.Warning,

            // Questionnaire exceptions
            QuestionnaireNotFoundException => LogLevel.Warning,
            QuestionnaireAlreadyPublishedException => LogLevel.Warning,
            QuestionnaireAlreadyClosedException => LogLevel.Warning,
            QuestionnaireNotPublishedException => LogLevel.Warning,
            QuestionnaireInvalidCollectionPeriodException => LogLevel.Warning,
            QuestionnaireCannotModifyPublishedException => LogLevel.Warning,

            // Question exceptions
            QuestionNotFoundException => LogLevel.Warning,
            OptionNotFoundException => LogLevel.Warning,
            QuestionNotReadyForPublicationException => LogLevel.Warning,
            QuestionCannotModifyException => LogLevel.Warning,
            OptionAlreadyExistsException => LogLevel.Warning,

            // Submission exceptions
            SubmissionNotFoundException => LogLevel.Warning,
            QuestionnaireNotAvailableException => LogLevel.Warning,
            DuplicateSubmissionException => LogLevel.Warning,
            InvalidSubmissionException => LogLevel.Warning,
            RequiredQuestionNotAnsweredException => LogLevel.Warning,
            OnlyPublicUsersCanSubmitException => LogLevel.Warning,

            // Generic expected errors
            KeyNotFoundException => LogLevel.Warning,
            UnauthorizedAccessException => LogLevel.Warning,
            ArgumentException => LogLevel.Warning,
            NotImplementedException => LogLevel.Information,

            // Error: Unexpected errors
            _ => LogLevel.Error
        };

        // ✅ MELHORADO: Logging estruturado
        _logger.Log(
            logLevel,
            exception,
            "Request {Method} {Path} failed with {ExceptionType}. " +
            "User: {UserId}, IP: {IpAddress}, StatusCode: {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            exception.GetType().Name,
            context.User?.Identity?.Name ?? "Anonymous",
            context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            context.Response.StatusCode
        );
    }

    private static string GetRfcLink(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            HttpStatusCode.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            HttpStatusCode.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            HttpStatusCode.NotImplemented => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
