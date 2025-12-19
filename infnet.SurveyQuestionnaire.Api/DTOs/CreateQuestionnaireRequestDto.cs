using System.ComponentModel.DataAnnotations;

namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO para criação de questionário (API Layer)
/// </summary>
public class CreateQuestionnaireRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// DTO para atualização de questionário (API Layer)
/// </summary>
public class UpdateQuestionnaireRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// DTO para publicação de questionário (API Layer)
/// </summary>
public class PublishQuestionnaireRequestDto
{
    [Required(ErrorMessage = "Collection start date is required")]
    public DateTime CollectionStart { get; init; }

    [Required(ErrorMessage = "Collection end date is required")]
    public DateTime CollectionEnd { get; init; }
}

/// <summary>
/// DTO para adicionar questão ao questionário (API Layer)
/// </summary>
public class AddQuestionRequestDto
{
    [Required(ErrorMessage = "Question text is required")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "Question text must be between 3 and 500 characters")]
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
    [Required(ErrorMessage = "Question text is required")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "Question text must be between 3 and 500 characters")]
    public string Text { get; init; } = string.Empty;

    public bool IsRequired { get; init; }

    public bool IsMultipleChoice { get; init; }
}
