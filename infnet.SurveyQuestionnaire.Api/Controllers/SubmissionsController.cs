using AutoMapper;
using infnet.SurveyQuestionnaire.Api.DTOs;
using infnet.SurveyQuestionnaire.Api.Models;
using infnet.SurveyQuestionnaire.Application.DTOs.Submissions;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace infnet.SurveyQuestionnaire.Api.Controllers;

/// <summary>
/// Controller para operações de submissions (respostas de questionários)
/// </summary>
[ApiController]
[Route("api/submissions")]
[Produces("application/json")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;
    private readonly IMapper _mapper;

    public SubmissionsController(
 ISubmissionService submissionService,
        IMapper mapper)
    {
        _submissionService = submissionService;
      _mapper = mapper;
    }

 // ==================== Create Submission (Async) ====================

    /// <summary>
    /// Cria uma nova submission (resposta de questionário) - Processamento Assíncrono
    /// </summary>
    /// <param name="requestDto">Dados da submission</param>
    /// <param name="userId">ID do usuário respondente (deve ser usuário público)</param>
    /// <returns>Submission criada com status Pending</returns>
    /// <response code="202">Submission aceita para processamento assíncrono</response>
    /// <response code="400">Dados inválidos ou questionário não disponível</response>
    /// <response code="403">Usuário não é público ou já respondeu</response>
    /// <response code="404">Questionário não encontrado</response>
    /// <remarks>
    /// ?? Esta operação é ASSÍNCRONA:
    /// - A API retorna 202 Accepted imediatamente
    /// - A submission é enviada para Azure Service Bus
    /// - Uma Azure Function processa em background
    /// - Use GET /api/submissions/{id} para verificar o status
    /// 
    /// Status possíveis:
/// - Pending: Aguardando processamento
    /// - Processing: Sendo processado
    /// - Completed: Processado com sucesso
    /// - Failed: Erro no processamento
    /// 
    /// Exemplo de requisição:
    /// 
    ///     POST /api/submissions
    ///     {
    ///   "questionnaireId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  ///   "answers": [
    ///         {
///      "questionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///    "answer": "Minha resposta",
    ///           "selectedOptionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///      }
    ///       ]
    ///     }
    /// 
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(SubmissionResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSubmission(
        [FromBody] CreateSubmissionRequestDto requestDto,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
      var request = _mapper.Map<CreateSubmissionRequest>(requestDto);
  var response = await _submissionService.CreateSubmissionAsync(request, userId);
      var responseDto = _mapper.Map<SubmissionResponseDto>(response);

        // ? Retorna 202 Accepted (processamento assíncrono)
        return Accepted(responseDto);
    }

    // ==================== Get Submission by ID ====================

    /// <summary>
    /// Busca uma submission por ID
    /// </summary>
    /// <param name="id">ID da submission</param>
    /// <param name="userId">ID do usuário (para validar autorização)</param>
    /// <returns>Submission encontrada</returns>
    /// <response code="200">Submission encontrada</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="404">Submission não encontrada</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubmissionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubmissionById(
 Guid id,
  [FromHeader(Name = "X-User-Id")] Guid userId)
  {
        var response = await _submissionService.GetSubmissionByIdAsync(id, userId);
        var responseDto = _mapper.Map<SubmissionResponseDto>(response);

  return Ok(responseDto);
    }

    // ==================== Get Submission with Items ====================

    /// <summary>
    /// Busca uma submission por ID incluindo todas as respostas
    /// </summary>
  /// <param name="id">ID da submission</param>
    /// <param name="userId">ID do usuário (para validar autorização)</param>
/// <returns>Submission com respostas</returns>
    /// <response code="200">Submission encontrada com respostas</response>
 /// <response code="403">Usuário não autorizado</response>
    /// <response code="404">Submission não encontrada</response>
    [HttpGet("{id:guid}/items")]
 [ProducesResponseType(typeof(SubmissionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubmissionWithItems(
      Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
    var response = await _submissionService.GetSubmissionWithItemsAsync(id, userId);
      var responseDto = _mapper.Map<SubmissionResponseDto>(response);

        return Ok(responseDto);
    }

    // ==================== Get My Submissions ====================

    /// <summary>
    /// Lista todas as submissions do usuário logado
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Lista de submissions do usuário</returns>
    /// <response code="200">Lista de submissions</response>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<SubmissionListResponseDto>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetMySubmissions(
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
 var responses = await _submissionService.GetMySubmissionsAsync(userId);
        var responseDtos = _mapper.Map<IEnumerable<SubmissionListResponseDto>>(responses);

        return Ok(responseDtos);
    }

    // ==================== Get Questionnaire Submissions (Admin) ====================

    /// <summary>
    /// Lista todas as submissions de um questionário (apenas admin ou criador)
/// </summary>
 /// <param name="questionnaireId">ID do questionário</param>
    /// <param name="userId">ID do usuário (admin ou criador)</param>
    /// <returns>Lista de submissions do questionário</returns>
    /// <response code="200">Lista de submissions</response>
  /// <response code="403">Usuário não autorizado</response>
    /// <response code="404">Questionário não encontrado</response>
    [HttpGet("questionnaire/{questionnaireId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<SubmissionListResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestionnaireSubmissions(
      Guid questionnaireId,
        [FromHeader(Name = "X-User-Id")] Guid userId)
 {
   var responses = await _submissionService.GetQuestionnaireSubmissionsAsync(questionnaireId, userId);
        var responseDtos = _mapper.Map<IEnumerable<SubmissionListResponseDto>>(responses);

        return Ok(responseDtos);
    }

 // ==================== Count Questionnaire Submissions (Admin) ====================

    /// <summary>
    /// Conta quantas submissions um questionário tem (apenas admin ou criador)
    /// </summary>
    /// <param name="questionnaireId">ID do questionário</param>
    /// <param name="userId">ID do usuário (admin ou criador)</param>
    /// <returns>Número de submissions</returns>
    /// <response code="200">Número de submissions</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="404">Questionário não encontrado</response>
    [HttpGet("questionnaire/{questionnaireId:guid}/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CountQuestionnaireSubmissions(
      Guid questionnaireId,
  [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var count = await _submissionService.CountQuestionnaireSubmissionsAsync(questionnaireId, userId);

return Ok(new { count });
    }
}
