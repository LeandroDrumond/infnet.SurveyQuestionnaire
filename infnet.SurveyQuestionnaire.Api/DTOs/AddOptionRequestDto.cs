using System.ComponentModel.DataAnnotations;

namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO para adicionar opção a uma questão (API Layer)
/// </summary>
public class AddOptionRequestDto
{
    public string Text { get; init; } = string.Empty;
    public int Order { get; init; }
}
