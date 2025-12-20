using infnet.SurveyQuestionnaire.Application.DTOs.Submissions;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using infnet.SurveyQuestionnaire.Domain;
using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Exceptions;
using infnet.SurveyQuestionnaire.Domain.Repositories;

namespace infnet.SurveyQuestionnaire.Application.Services;

/// <summary>
/// Serviço responsável por processar submissions de forma assíncrona
/// Não publica mensagens - apenas processa
/// </summary>
public class SubmissionProcessor : ISubmissionProcessor
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmissionProcessor(
        ISubmissionRepository submissionRepository,
        IUnitOfWork unitOfWork)
    {
        _submissionRepository = submissionRepository;
        _unitOfWork = unitOfWork;
    }

  public async Task ProcessSubmissionAsync(SubmissionMessage message)
    {
        try
        {
   var submission = await LoadSubmissionForProcessingAsync(message.SubmissionId);

            if (ShouldSkipProcessing(submission))
                return;

            if (await TryCompletePartialSubmissionAsync(submission))
       return;

            await ResetSubmissionIfNecessaryAsync(submission);
     await StartAndProcessSubmissionAsync(submission, message.Answers);
await CompleteSubmissionAsync(submission);
      }
        catch (Exception)
     {
       await HandleProcessingFailureAsync(message.SubmissionId);
            throw;
     }
    }

    private async Task<Submission> LoadSubmissionForProcessingAsync(Guid submissionId)
    {
      return await _submissionRepository.GetByIdWithItemsAsync(submissionId)
      ?? throw new SubmissionNotFoundException(submissionId);
    }

private static bool ShouldSkipProcessing(Submission submission)
    {
        return submission.Status == SubmissionStatus.Completed;
    }

  private async Task<bool> TryCompletePartialSubmissionAsync(Submission submission)
    {
        if (submission.Status != SubmissionStatus.Processing || !submission.Items.Any())
       return false;

        submission.Complete();
        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task ResetSubmissionIfNecessaryAsync(Submission submission)
    {
if (submission.Status == SubmissionStatus.Processing ||
        submission.Status == SubmissionStatus.Failed)
        {
       submission.ResetToPending();
        await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task StartAndProcessSubmissionAsync(Submission submission, List<SubmissionAnswerMessage> answers)
    {
        // Adicionar items ANTES de StartProcessing (regra de negócio: só adiciona em Pending)
        foreach (var answer in answers)
   {
         submission.AddItem(answer.QuestionId, answer.Answer, answer.SelectedOptionId);
        }

        // Marcar como Processing
        submission.StartProcessing();

        // Salvar items + status
        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync();
  }

    private async Task CompleteSubmissionAsync(Submission submission)
    {
        submission.Complete();
        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync();
    }

 private async Task HandleProcessingFailureAsync(Guid submissionId)
    {
     var submission = await _submissionRepository.GetByIdAsync(submissionId);

        if (submission != null && submission.Status != SubmissionStatus.Failed)
    {
       submission.Fail("Error processing submission");
        _submissionRepository.Update(submission);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
