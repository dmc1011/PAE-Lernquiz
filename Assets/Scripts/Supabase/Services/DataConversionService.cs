using Models;
using Entities;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace DataConversion
{
    public static class EntityModelConverter
    {
        public static Entities.Profile ToEntity(this Models.Profile model)
        {
            return new Entities.Profile(model.UserId, model.Name, model.Surname, model.Role);
        }

        public static Entities.Topic ToEntity(this Models.Topic model)
        {
            return new Entities.Topic(model.Name);
        }

        public static CatalogueDTO ToDTO(this Models.Catalogue model)
        {
            return new CatalogueDTO(model.Id, model.Name, model.TopicName, model.IsPrivate, model.CreatedBy);
        }

        /*
        public static Entities.Catalogue FromJson(JsonElement json)
        {
            int catalogueId = json.GetProperty("id").GetInt32();
            string catalogueName = json.GetProperty("name").GetString();
            int currentQuestionId = json.GetProperty("currentQuestionId").GetInt32();
            int totalTimeSpent = json.GetProperty("totalTimeSpent").GetInt32();
            int sessionCount = json.GetProperty("sessionCount").GetInt32();
            int errorFreeSessionCount = json.GetProperty("errorFreeSessionCount").GetInt32();
            int completedRandomQuizCount = json.GetProperty("completedRandomQuizCount").GetInt32();
            int errorFreeRandomQuizCount = json.GetProperty("errorFreeRandomQuizCount").GetInt32();
            string topicName = json.GetProperty("topicName").GetString();
            bool isPrivate = json.GetProperty("isPrivate").GetBoolean();
            Guid createdBy = json.GetProperty("createdBy").GetGuid();

            var questions = new List<Question>();
            var questionsJson = json.GetProperty("questions");

            foreach (var q in questionsJson.EnumerateArray())
            {
                int qId = q.GetProperty("id").GetInt32();
                string qText = q.GetProperty("text").GetString();
                string qDisplayName = q.GetProperty("displayName").GetString();
                int qCorrectAnsweredCount = q.GetProperty("correctAnsweredCount").GetInt32();
                int qTotalAnsweredCount = q.GetProperty("totalAnsweredCount").GetInt32();
                bool qEnabledForPractice = q.GetProperty("inPractice").GetBoolean();

                var answers = new List<Answer>();
                var answerHistory = new List<AnswerHistory>();

                foreach (var a in q.GetProperty("answers").EnumerateArray())
                {
                    answers.Add(new Answer
                    {
                        id = a.GetProperty("id").GetInt32(),
                        text = a.GetProperty("text").GetString(),
                        isCorrect = a.GetProperty("isCorrect").GetBoolean()
                    });
                }

                
                questions.Add(new Question
                {
                    qId,
                    qText,
                    qDisplayName,
                    qCorrectAnsweredCount,
                    qTotalAnsweredCount,
                    catalogueId,
                    qEnabledForPractice,
                    answers,
                    answerHistory,
                });
                
            }

            var histories = new List<CatalogueSessionHistory>();
            if (json.TryGetProperty("sessionHistories", out var historyJson) && historyJson.ValueKind == JsonValueKind.Array)
            {
                foreach (var h in historyJson.EnumerateArray())
                {
                    histories.Add(new CatalogueSessionHistory
                    {
                        id = h.GetProperty("id").GetInt32(),
                        sessionDate = h.GetProperty("sessionDate").GetDateTime(),
                        timeSpent = h.GetProperty("timeSpent").GetInt32(),
                        isComplete = h.GetProperty("isComplete").GetBoolean(),
                        isErrorFree = h.GetProperty("isErrorFree").GetBoolean()
                    });
                }
            }


            return new Entities.Catalogue(
                catalogueId,
                catalogueName,
                currentQuestionId,
                totalTimeSpent,
                sessionCount,
                errorFreeSessionCount,
                completedRandomQuizCount,
                errorFreeRandomQuizCount,
                questions: questions,
                sessionHistories: histories,
                topicName,
                isPrivate,
                createdBy
            );
        }*/
    }
}
