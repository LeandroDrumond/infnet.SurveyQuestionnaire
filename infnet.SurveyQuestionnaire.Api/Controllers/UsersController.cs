using AutoMapper;
using infnet.SurveyQuestionnaire.Api.DTOs;
using infnet.SurveyQuestionnaire.Api.Models;
using infnet.SurveyQuestionnaire.Application.DTOs.Users;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace infnet.SurveyQuestionnaire.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService,ILogger<UsersController> logger,IMapper mapper)
    {
        _userService = userService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um novo usuário no sistema
    /// </summary>
    /// <param name="requestDto">Dados do usuário a ser criado</param>
    /// <returns>Usuário criado com sucesso</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    /// <response code="409">Email já cadastrado no sistema</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto requestDto)
    {
         _logger.LogInformation("Creating new {UserType} user with email: {Email}",requestDto.UserType, requestDto.Email);

        var request = _mapper.Map<CreateUserRequest>(requestDto);
        var response = await _userService.CreateUserAsync(request);
        var responseDto = _mapper.Map<UserResponseDto>(response);

        _logger.LogInformation("{UserType} user created successfully with ID: {UserId}",responseDto.UserType, responseDto.Id);

        return CreatedAtAction(nameof(GetUserById),new { id = responseDto.Id },responseDto);
    }

    /// <summary>
    /// Obtém um usuário específico por ID
    /// </summary>
    /// <param name="id">ID único do usuário (GUID)</param>
    /// <returns>Dados do usuário encontrado</returns>
    /// <response code="200">Usuário encontrado com sucesso</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var response = await _userService.GetUserByIdAsync(id);
        var responseDto = _mapper.Map<UserResponseDto>(response);
        return Ok(responseDto);
    }

    /// <summary>
    /// Lista todos os usuários do sistema
    /// </summary>
    /// <returns>Lista de todos os usuários</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        var responseDto = _mapper.Map<IEnumerable<UserResponseDto>>(response);
        return Ok(responseDto);
    }

    /// <summary>
    /// Atualiza os dados de um usuário existente
    /// </summary>
    /// <param name="id">ID único do usuário (GUID)</param>
    /// <param name="requestDto">Novos dados do usuário</param>
    /// <returns>Usuário atualizado</returns>
    /// <response code="200">Usuário atualizado com sucesso</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="409">Email já cadastrado para outro usuário</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequestDto requestDto)
    {
        _logger.LogInformation("Updating user: {UserId}", id);

        var request = _mapper.Map<UpdateUserRequest>(requestDto);
        var response = await _userService.UpdateUserAsync(id, request);
        var responseDto = _mapper.Map<UserResponseDto>(response);

        _logger.LogInformation("User updated successfully: {UserId}", id);

        return Ok(responseDto);
    }

    /// <summary>
    /// Desativa um usuário (soft delete)
    /// </summary>
    /// <param name="id">ID único do usuário (GUID)</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Usuário desativado com sucesso</response>
    /// <response code="400">Usuário já está inativo</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        _logger.LogInformation("Deactivating user: {UserId}", id);

        await _userService.DeactivateUserAsync(id);

        _logger.LogInformation("User deactivated successfully: {UserId}", id);

        return NoContent();
    }

    /// <summary>
    /// Reativa um usuário previamente desativado
    /// </summary>
    /// <param name="id">ID único do usuário (GUID)</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Usuário ativado com sucesso</response>
    /// <response code="400">Usuário já está ativo</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        _logger.LogInformation("Activating user: {UserId}", id);

        await _userService.ActivateUserAsync(id);

        _logger.LogInformation("User activated successfully: {UserId}", id);

        return NoContent();
    }
}
