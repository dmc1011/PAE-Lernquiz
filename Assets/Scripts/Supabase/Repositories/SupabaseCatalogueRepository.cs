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

            if (ucpResult == null)
            {
                ucpResult = new UserCatalogueProgress(userId, catalogueId);
            }

            var uchResult = await _client
                .From<Models.UserCatalogueHistory>()
                .Order(uch => uch.SessionDate, Constants.Ordering.Descending)
                .Limit(1)
                .Get();

            var latestUch = uchResult.Models.FirstOrDefault();
            var uchLst = new List<CatalogueSessionHistory>();

            if (latestUch != null)
            {
                uchLst.Add(latestUch.ToEntity());
            }

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
                UserQuestionProgress qProgress = uqpResult.Models.Find(u => u.QuestionId == q.Id) ?? new UserQuestionProgress(userId, q.Id);

                List<Models.Answer> answerModels = answers.Models.FindAll(a => a.QuestionId == q.Id).OrderBy(a => a.IsCorrect).ToList();
                List<Answer> answerEntities = answerModels.Select(model => model.ToEntity()).ToList();

                Entities.Question question = q.ToEntity(qProgress, answerEntities, new List<AnswerHistory>());

                questionLst.Add(question);
            }

            Debug.Log(questionLst.Count);

            int currentQId = ucpResult?.CurrentQuestionId ?? 0;

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
                uchLst,
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

    public async Task UpdateCatalogue(Entities.Catalogue catalogue)
    {
        // to do
    }

    public async Task UpdateCatalogueUserData(Entities.Catalogue catalogue)
    {
        Guid userId = Guid.Parse(_client.Auth.CurrentUser.Id);

        Models.UserCatalogueProgress progress = catalogue.ToModel(userId);

        List<Models.UserCatalogueHistory> uchList = catalogue.sessionHistories.Select(session => session.ToModel(userId)).ToList();
        
        try
        {
            await _client.From<Models.UserCatalogueProgress>().Upsert(progress);

            await _client.From<UserCatalogueHistory>().Upsert(uchList);
        }
        catch (Exception e)
        {
            throw new SupabaseRequestException("Fehler bei Supbase Request: " + e.Message);
        }
    }
}
