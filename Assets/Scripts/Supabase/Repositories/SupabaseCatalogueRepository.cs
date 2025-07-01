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
        Guid currentUserId = Guid.Parse(_client.Auth.CurrentUser.Id);

        try
        {
            var result = await _client
                .From<Models.Catalogue>()
                .Where(c => c.TopicName == topic)
                .Where(c => c.IsPrivate == false || c.CreatedBy == currentUserId)
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

                List<Models.Answer> answerModels = answers.Models.FindAll(a => a.QuestionId == q.Id).OrderBy(a => a.Id).ToList();
                List<Answer> answerEntities = answerModels.Select(model => model.ToEntity()).ToList();

                Debug.Log("Answer " + answerEntities[0].id + ": " + answerEntities[0].isCorrect);
                Debug.Log("Answer " + answerEntities[1].id + ": " + answerEntities[1].isCorrect);
                Debug.Log("Answer " + answerEntities[2].id + ": " + answerEntities[2].isCorrect);
                Debug.Log("Answer " + answerEntities[3].id + ": " + answerEntities[3].isCorrect);

                Entities.Question question = q.ToEntity(qProgress, answerEntities, new List<AnswerHistory>());

                questionLst.Add(question);
            }

            Debug.Log("Cat Repo - Question Count: " + questionLst.Count);

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
                catalogueResult.TopicName,
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
        try
        {
            Debug.Log("Topic Name: " + catalogue.topicName);

            // upsert catalogue
            Models.Catalogue catalogueModel = new Models.Catalogue
            {
                Name = catalogue.name,
                TopicName = catalogue.topicName,
                IsPrivate = catalogue.isPrivate,
                CreatedBy = catalogue.createdBy,
            };

            if (!EditorManager.isNewCatalogue)
            {
                catalogueModel.Id = catalogue.id;
                await _client.From<Models.Catalogue>().Update(catalogueModel);
            }
            else
            {
                var result = await _client.From<Models.Catalogue>().Insert(catalogueModel);
                catalogue.id = result.Model.Id;
            }

            // upsert questions and answers
            if (catalogue.questions.Count > 0)
            {
                Debug.Log("Question Count: " + catalogue.questions.Count);

                List<Models.Question> questionModels = new List<Models.Question>();
                List<Models.Answer> answerModels = new List<Models.Answer>();

                List<Entities.Question> questions = catalogue.questions;
                List<Answer> answers = new List<Answer>();

                foreach (var question in questions)
                {
                    Debug.Log("Answe Count for Question: " + question.answers.Count);

                    Models.Question q = new Models.Question
                    {
                        CatalogueId = catalogue.id,
                        Text = question.text,
                        DisplayName = question.name,
                    };

                    if (question.id >= 0)
                    {
                        q.Id = question.id;
                        questionModels.Add(q);
                    }
                    else
                    {
                        var qResult = await _client.From<Models.Question>().Insert(q);
                        Debug.Log("Created Question " + qResult.Model.Id);
                        question.id = qResult.Model.Id;
                    }

                    foreach (var answer in question.answers)
                    {
                        answer.questionId = question.id;
                        answers.Add(answer);
                    }
                }

                foreach (var answer in answers)
                {
                    Debug.Log("Loop Answer " + answer.id);
                    Debug.Log("Question Id: " + answer.questionId);
                    Debug.Log(answer.text);
                    Debug.Log(answer.isCorrect);

                    Models.Answer a = new Models.Answer
                    {
                        QuestionId = answer.questionId,
                        Text = answer.text,
                        IsCorrect = answer.isCorrect,
                    };

                    if (answer.id >= 0)
                    {
                        a.Id = answer.id;
                        answerModels.Add(a);
                    }
                    else
                    {
                        var aResult = await _client.From<Models.Answer>().Insert(a);
                        Debug.Log("Created Answer " + aResult.Model.Id);
                        answer.id = aResult.Model.Id;
                    }
                }

                if (questionModels.Count > 0)
                {
                    await _client.From<Models.Question>().Upsert(questionModels);
                }

                if (answerModels.Count > 0)
                {
                    await _client.From<Models.Answer>().Upsert(answerModels);
                }
            }
        }
        catch (Exception e)
        {
            throw new SupabaseRequestException("Fehler beim Aktualisieren eines Katalogs: " + e.Message);
        }
    }

    public async Task UpdateCatalogueUserData(Entities.Catalogue catalogue)
    {
        Guid userId = Guid.Parse(_client.Auth.CurrentUser.Id);

        Models.UserCatalogueProgress progress = catalogue.ToModel(userId);

        List<Models.UserCatalogueHistory> uchList = catalogue.sessionHistories.Select(session => session.ToModel(userId)).ToList();

        List<Models.UserQuestionProgress> uqpList = catalogue.questions.Select(q => q.ToQuestionProgressModel(userId)).ToList();

        List<Models.UserQuestionHistory> uqhList = new List<Models.UserQuestionHistory>();

        foreach (var question in catalogue.questions)
        {
            uqhList.AddRange(question.ToQuestionHistoryModels(userId));
        }
        
        try
        {
            await _client.From<Models.UserCatalogueProgress>().Upsert(progress);

            await _client.From<UserCatalogueHistory>().Upsert(uchList);

            await _client.From<Models.UserQuestionProgress>().Upsert(uqpList);

            await _client.From<UserQuestionHistory>().Insert(uqhList);
        }
        catch (Exception e)
        {
            throw new SupabaseRequestException("Fehler beim Ausführen eines Supabase Requests: " + e.Message);
        }
    }

    public async Task StoreTopic(Models.Topic topic)
    {
        try
        {
            var result = await _client.From<Models.Topic>().Upsert(topic);
        }
        catch (Exception e)
        {
            throw new SupabaseRequestException("Fehler beim speichern eines Topics: " + e.Message);
        }
    }

    public async Task DeleteCatalogue(int catalogueId)
    {
        try
        {
            await _client.From<Models.Catalogue>().Where(c => c.Id == catalogueId).Delete();
        }
        catch (Exception e)
        {
            throw new SupabaseRequestException("Fehler beim Löschen eines Katalogs: " + e.Message);
        }
    }

    public async Task DeleteQuestion(int questionId)
    {
        try
        {
            await _client.From<Models.Question>().Where(q => q.Id == questionId).Delete();
        }
        catch (Exception e)
        {
            throw new SupabaseRequestException("Fehler beim Löschen einer Frage: " + e.Message);
        }
    }

    public async Task UpdateTopic(Models.Topic newTopic, string oldTopicName)
    {
        try
        {
            await _client.From<Models.Topic>().Where(t => t.Name == oldTopicName).Set(t => t.Name, newTopic.Name).Update();
        }
        catch(Exception e)
        {
            throw new SupabaseRequestException("Fehler bei Überschreiben des Topics: " + e.Message);
        }
    }

    public async Task DeleteTopic(string topicName)
    {
        try
        {
            await _client.From<Models.Topic>().Where(t => t.Name == topicName).Delete();
        }
        catch (Exception e)
        {
            throw new SupabaseRequestException("Fehler beim Löschen eines Topics: " + e.Message);
        }
    }
}
