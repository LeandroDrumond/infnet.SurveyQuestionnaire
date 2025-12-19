using AutoMapper;
using infnet.SurveyQuestionnaire.Api.DTOs;
using infnet.SurveyQuestionnaire.Api.Models;
using infnet.SurveyQuestionnaire.Application.DTOs.Questionnaires;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace infnet.SurveyQuestionnaire.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de questionários
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class QuestionnairesController : ControllerBase
{
    private readonly IQuestionnaireService _questionnaireService;
    private readonly ILogger<QuestionnairesController> _logger;
    private readonly IMapper _mapper;

    public QuestionnairesController(
 IQuestionnaireService questionnaireService,
    ILogger<QuestionnairesController> logger,
        IMapper mapper)
    {
     _questionnaireService = questionnaireService;
        _logger = logger;
        _mapper = mapper;
    }

    // ==================== Questionnaire Endpoints ====================

    /// <summary>
    /// Cria um novo questionário (apenas administradores)
    /// </summary>
    /// <param name="requestDto">Dados do questionário</param>
    /// <param name="createdByUserId">ID do usuário administrador (via header ou claim)</param>
    /// <returns>Questionário criado</returns>
    /// <response code="201">Questionário criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
 /// <response code="401">Usuário não autorizado (não é administrador)</response>
    [HttpPost]
    [ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateQuestionnaire(
      [FromBody] CreateQuestionnaireRequestDto requestDto,
    [FromHeader(Name = "X-User-Id")] Guid createdByUserId)
 {
        _logger.LogInformation("Creating questionnaire: {Title} by user {UserId}", 
 requestDto.Title, createdByUserId);

        var request = _mapper.Map<CreateQuestionnaireRequest>(requestDto);
     var response = await _questionnaireService.CreateQuestionnaireAsync(request, createdByUserId);
        var responseDto = _mapper.Map<QuestionnaireResponseDto>(response);

   _logger.LogInformation("Questionnaire created with ID: {QuestionnaireId}", responseDto.Id);

        return CreatedAtAction(
nameof(GetQuestionnaireById),
new { id = responseDto.Id },
    responseDto);
    }

 /// <summary>
  /// Obtém um questionário por ID (sem questões)
    /// </summary>
    /// <param name="id">ID do questionário</param>
 /// <returns>Dados do questionário</returns>
/// <response code="200">Questionário encontrado</response>
    /// <response code="404">Questionário não encontrado</response>
[HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestionnaireById(Guid id)
 {
  var response = await _questionnaireService.GetQuestionnaireByIdAsync(id);
        var responseDto = _mapper.Map<QuestionnaireResponseDto>(response);
      return Ok(responseDto);
    }

    /// <summary>
    /// Obtém um questionário por ID com todas as questões e opções
    /// </summary>
    /// <param name="id">ID do questionário</param>
    /// <returns>Dados completos do questionário</returns>
    /// <response code="200">Questionário encontrado</response>
    /// <response code="404">Questionário não encontrado</response>
    [HttpGet("{id:guid}/details")]
    [ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetQuestionnaireWithQuestions(Guid id)
    {
      var response = await _questionnaireService.GetQuestionnaireWithQuestionsAsync(id);
  var responseDto = _mapper.Map<QuestionnaireResponseDto>(response);
 return Ok(responseDto);
  }

    /// <summary>
    /// Lista todos os questionários
    /// </summary>
    /// <returns>Lista de questionários</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestionnaireListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllQuestionnaires()
    {
  var response = await _questionnaireService.GetAllQuestionnairesAsync();
        var responseDto = _mapper.Map<IEnumerable<QuestionnaireListResponseDto>>(response);
 return Ok(responseDto);
 }

    /// <summary>
 /// Lista questionários criados por um usuário específico
    /// </summary>
/// <param name="creatorId">ID do criador</param>
    /// <returns>Lista de questionários</returns>
    /// <response code="200">Lista retornada com sucesso</response>
[HttpGet("by-creator/{creatorId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<QuestionnaireListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuestionnairesByCreator(Guid creatorId)
  {
        var response = await _questionnaireService.GetQuestionnairesByCreatorAsync(creatorId);
        var responseDto = _mapper.Map<IEnumerable<QuestionnaireListResponseDto>>(response);
     return Ok(responseDto);
    }

    /// <summary>
/// Lista todos os questionários publicados
    /// </summary>
/// <returns>Lista de questionários publicados</returns>
    /// <response code="200">Lista retornada com sucesso</response>
 [HttpGet("published")]
    [ProducesResponseType(typeof(IEnumerable<QuestionnaireListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublishedQuestionnaires()
    {
     var response = await _questionnaireService.GetPublishedQuestionnairesAsync();
        var responseDto = _mapper.Map<IEnumerable<QuestionnaireListResponseDto>>(response);
     return Ok(responseDto);
    }

    /// <summary>
    /// Atualiza um questionário (apenas em Draft)
    /// </summary>
/// <param name="id">ID do questionário</param>
    /// <param name="requestDto">Novos dados</param>
 /// <param name="userId">ID do usuário</param>
    /// <returns>Questionário atualizado</returns>
    /// <response code="200">Questionário atualizado</response>
    /// <response code="400">Dados inválidos ou questionário já publicado</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Questionário não encontrado</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status200OK)]
 [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestionnaire(
     Guid id,
 [FromBody] UpdateQuestionnaireRequestDto requestDto,
   [FromHeader(Name = "X-User-Id")] Guid userId)
 {
        _logger.LogInformation("Updating questionnaire {QuestionnaireId} by user {UserId}", id, userId);

   var request = _mapper.Map<UpdateQuestionnaireRequest>(requestDto);
    var response = await _questionnaireService.UpdateQuestionnaireAsync(id, request, userId);
    var responseDto = _mapper.Map<QuestionnaireResponseDto>(response);

        return Ok(responseDto);
    }

    /// <summary>
    /// Deleta um questionário
    /// </summary>
    /// <param name="id">ID do questionário</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>No content</returns>
    /// <response code="204">Questionário deletado</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Questionário não encontrado</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestionnaire(
 Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
     _logger.LogInformation("Deleting questionnaire {QuestionnaireId} by user {UserId}", id, userId);

      await _questionnaireService.DeleteQuestionnaireAsync(id, userId);

        return NoContent();
    }

 // ==================== Publish/Close Endpoints ====================

    /// <summary>
    /// Publica um questionário
    /// </summary>
  /// <param name="id">ID do questionário</param>
    /// <param name="requestDto">Datas de coleta</param>
  /// <param name="userId">ID do usuário</param>
 /// <returns>Questionário publicado</returns>
    /// <response code="200">Questionário publicado</response>
    /// <response code="400">Dados inválidos ou questionário não está pronto</response>
  /// <response code="401">Não autorizado</response>
 /// <response code="404">Questionário não encontrado</response>
 [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status200OK)]
 [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishQuestionnaire(
     Guid id,
     [FromBody] PublishQuestionnaireRequestDto requestDto,
   [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        _logger.LogInformation("Publishing questionnaire {QuestionnaireId} by user {UserId}", id, userId);

     var request = _mapper.Map<PublishQuestionnaireRequest>(requestDto);
  var response = await _questionnaireService.PublishQuestionnaireAsync(id, request, userId);
        var responseDto = _mapper.Map<QuestionnaireResponseDto>(response);

      _logger.LogInformation("Questionnaire {QuestionnaireId} published successfully", id);

    return Ok(responseDto);
    }

    /// <summary>
    /// Fecha um questionário
    /// </summary>
    /// <param name="id">ID do questionário</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Questionário fechado</returns>
    /// <response code="200">Questionário fechado</response>
    /// <response code="400">Questionário não está publicado</response>
 /// <response code="401">Não autorizado</response>
    /// <response code="404">Questionário não encontrado</response>
    [HttpPost("{id:guid}/close")]
[ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
 public async Task<IActionResult> CloseQuestionnaire(
  Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        _logger.LogInformation("Closing questionnaire {QuestionnaireId} by user {UserId}", id, userId);

        var response = await _questionnaireService.CloseQuestionnaireAsync(id, userId);
        var responseDto = _mapper.Map<QuestionnaireResponseDto>(response);

      _logger.LogInformation("Questionnaire {QuestionnaireId} closed successfully", id);

        return Ok(responseDto);
    }

    // ==================== Question Endpoints ====================

    /// <summary>
    /// Adiciona uma questão ao questionário
    /// </summary>
    /// <param name="id">ID do questionário</param>
    /// <param name="requestDto">Dados da questão</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Questionário com a nova questão</returns>
    /// <response code="200">Questão adicionada</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autorizado</response>
/// <response code="404">Questionário não encontrado</response>
[HttpPost("{id:guid}/questions")]
    [ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
 public async Task<IActionResult> AddQuestion(
  Guid id,
        [FromBody] AddQuestionRequestDto requestDto,
        [FromHeader(Name = "X-User-Id")] Guid userId)
{
    _logger.LogInformation("Adding question to questionnaire {QuestionnaireId}", id);

  var request = _mapper.Map<AddQuestionRequest>(requestDto);
   var response = await _questionnaireService.AddQuestionAsync(id, request, userId);
     var responseDto = _mapper.Map<QuestionnaireResponseDto>(response);

  return Ok(responseDto);
 }

    /// <summary>
    /// Atualiza uma questão
    /// </summary>
    /// <param name="questionId">ID da questão</param>
/// <param name="requestDto">Novos dados da questão</param>
  /// <param name="userId">ID do usuário</param>
    /// <returns>Questão atualizada</returns>
    /// <response code="200">Questão atualizada</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Questão não encontrada</response>
    [HttpPut("questions/{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestion(
   Guid questionId,
        [FromBody] UpdateQuestionRequestDto requestDto,
  [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        _logger.LogInformation("Updating question {QuestionId}", questionId);

      var request = _mapper.Map<UpdateQuestionRequest>(requestDto);
      var response = await _questionnaireService.UpdateQuestionAsync(questionId, request, userId);
        var responseDto = _mapper.Map<QuestionResponseDto>(response);

    return Ok(responseDto);
    }

    /// <summary>
  /// Deleta uma questão
 /// </summary>
    /// <param name="questionId">ID da questão</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>No content</returns>
    /// <response code="204">Questão deletada</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Questão não encontrada</response>
    /// <response code="501">Funcionalidade não implementada</response>
    [HttpDelete("questions/{questionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> DeleteQuestion(
     Guid questionId,
        [FromHeader(Name = "X-User-Id")] Guid userId)
 {
    _logger.LogInformation("Deleting question {QuestionId}", questionId);

    await _questionnaireService.DeleteQuestionAsync(questionId, userId);

   return NoContent();
    }

    // ==================== Option Endpoints ====================

    /// <summary>
    /// Adiciona uma ou mais opções a uma questão de múltipla escolha
    /// </summary>
    /// <param name="questionId">ID da questão</param>
    /// <param name="optionsDto">Lista de opções a adicionar (pode ser 1 ou mais)</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Questão com as novas opções</returns>
    /// <response code="200">Opções adicionadas com sucesso</response>
    /// <response code="400">Dados inválidos ou questão não é múltipla escolha</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Questão não encontrada</response>
    /// <remarks>
    /// Você pode enviar uma ou mais opções de uma vez.
    /// 
    /// Exemplo com 1 opção:
    /// 
    ///     POST /api/questionnaires/questions/{questionId}/options
    ///     [
    ///       { "text": "Nova opção", "order": 1 }
    /// ]
    /// 
  /// Exemplo com múltiplas opções:
    /// 
    ///     POST /api/questionnaires/questions/{questionId}/options
    ///     [
    ///       { "text": "Opção 1", "order": 1 },
    ///    { "text": "Opção 2", "order": 2 },
    ///       { "text": "Opção 3", "order": 3 }
    ///     ]
    /// 
    /// </remarks>
    [HttpPost("questions/{questionId:guid}/options")]
  [ProducesResponseType(typeof(QuestionResponseDto), StatusCodes.Status200OK)]
 [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddOptions(
     Guid questionId,
        [FromBody] List<AddOptionRequestDto> optionsDto,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        _logger.LogInformation("Adding {Count} option(s) to question {QuestionId}", optionsDto.Count, questionId);

        var options = optionsDto.Select(dto => _mapper.Map<AddOptionRequest>(dto));
        var response = await _questionnaireService.AddOptionsToQuestionAsync(questionId, options, userId);
        var responseDto = _mapper.Map<QuestionResponseDto>(response);

        _logger.LogInformation("Added {Count} option(s) to question {QuestionId} successfully", optionsDto.Count, questionId);

        return Ok(responseDto);
  }
}
