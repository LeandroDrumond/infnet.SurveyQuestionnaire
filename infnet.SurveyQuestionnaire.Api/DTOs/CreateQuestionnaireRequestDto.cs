using System.ComponentModel.DataAnnotations;

namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO para criação de questionário (API Layer)
/// </summary>
public class CreateQuestionnaireRequestDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// DTO para atualização de questionário (API Layer)
/// </summary>
public class UpdateQuestionnaireRequestDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// DTO para publicação de questionário (API Layer)
/// </summary>
public class PublishQuestionnaireRequestDto
{
    public DateTime CollectionStart { get; init; }
    public DateTime CollectionEnd { get; init; }
}

/// <summary>
/// DTO para adicionar questão ao questionário (API Layer)
/// </summary>
public class AddQuestionRequestDto
{
    public string Text { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public bool IsMultipleChoice { get; init; }
    public List<AddOptionRequestDto>? Options { get; init; }
}

/// <summary>
/// DTO para atualizar questão (API Layer)
/// </summary>
public class UpdateQuestionRequestDto
{
    public string Text { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public bool IsMultipleChoice { get; init; }
}
