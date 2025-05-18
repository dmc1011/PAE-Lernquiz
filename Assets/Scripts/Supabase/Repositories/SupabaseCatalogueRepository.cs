using Postgrest;
using Repositories;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;
using Entities;
using System.Linq;
using DataConversion;
using System;
using System.Text.Json;
using Models;
using System.Runtime.InteropServices;
using static DataManager;

public class SupabaseCatalogueRepository : ICatalogueRepository
{
    private Client _client;

    public SupabaseCatalogueRepository(Client client)
    {
        _client = client;
    }

    public async Task<List<CatalogueDTO>> GetCataloguesByTopic(string topic)
    {
        try
        {
            var result = await _client
                .From<Models.Catalogue>()
                .Where(c => c.TopicName == topic)
                .Where(c => c.IsPrivate == false)
                .Order(c => c.Id, Constants.Ordering.Ascending)
                .Get();

            List<Models.Catalogue> models = result.Models;
            List<CatalogueDTO> catalogues = models.Select(model => model.ToDTO()).ToList();

            return catalogues;
        }
        catch (Exception e)
        {
            throw new FetchDataException("Error fetching catalogues: " + e.Message);
        }
    }

    public async Task<Entities.Catalogue> GetCatalogueById(int catalogueId)

    {
        try
        {
            var currentUser = _client.Auth.CurrentUser;

            if (currentUser == null)
            {
                throw new FetchDataException("Kein Nutzer vorhanden, um den Katalog zu laden.");
            }

            Guid userId = Guid.Parse(currentUser.Id);

            // get models
            var catalogueResult = await _client
                .From<Models.Catalogue>()
                .Where(c => c.Id == catalogueId)
                .Single();

            var ucpResult = await _client
                .From<Models.UserCatalogueProgress>()
                .Where(u => u.UserId == userId)
                .Where(u => u.CatalogueId == catalogueId)
                .Single();

            var questionsResult = await _client
                .From<Models.Question>()
                .Where(q => q.CatalogueId == catalogueId)
                .Order(q => q.Id, Constants.Ordering.Ascending)
                .Get();

            var questionIds = questionsResult.Models.Select(q => q.Id).ToList();

            var uqpResult = await _client
                .From<Models.UserQuestionProgress>()
                .Where(u => u.UserId == userId)
                .Filter(u => u.QuestionId, Constants.Operator.In, questionIds)
                .Get();

            var answers = await _client
                .From<Models.Answer>()
                .Filter(a => a.QuestionId, Constants.Operator.In, questionIds)
                .Order(a => a.Id, Constants.Ordering.Ascending)
                .Get();

            // convert models
            List<Entities.Question> questionLst = new List<Entities.Question>();

            foreach (var q in questionsResult.Models)
            {
                UserQuestionProgress qProgress = uqpResult.Models.Find(u => u.QuestionId == q.Id);
                
                List<Models.Answer> answerModels = answers.Models.FindAll(a => a.QuestionId == q.Id).OrderBy(a => a.IsCorrect).ToList();
                List<Answer> answerEntities = answerModels.Select(model => model.ToEntity()).ToList();

                Entities.Question questions = q.ToEntity(qProgress, answerEntities, new List<AnswerHistory>());

                questionLst.Add(questions);
            }

            int currentQId = ucpResult.CurrentQuestionId ?? 0;

            Entities.Catalogue catalogue = new Entities.Catalogue(
                catalogueId,
                catalogueResult.Name,
                currentQId,
                ucpResult.TimeSpent,
                ucpResult.Level,
                ucpResult.ErrorFreeRuns,
                ucpResult.RandomQuizCount,
                ucpResult.ErrorFreeRandomQuizCount,
                questionLst,
                new List<CatalogueSessionHistory>(),
                isPrivate: catalogueResult.IsPrivate,
                createdBy: catalogueResult.CreatedBy
            );

            return catalogue;
        }
        catch (Exception e)
        {
            throw new FetchDataException("Fehler beim Laden von Katalog " + catalogueId + ": " + e.Message);
        }
    }
}
