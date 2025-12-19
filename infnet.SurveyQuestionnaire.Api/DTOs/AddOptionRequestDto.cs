using System.ComponentModel.DataAnnotations;

namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO para adicionar opção a uma questão (API Layer)
/// </summary>
public class AddOptionRequestDto
{
    [Required(ErrorMessage = "Option text is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Option text must be between 1 and 200 characters")]
    public string Text { get; init; } = string.Empty;

    [Required(ErrorMessage = "Order is required")]
    [Range(1, 100, ErrorMessage = "Order must be between 1 and 100")]
    public int Order { get; init; }
}
