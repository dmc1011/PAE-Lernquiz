using Models;
using Entities;
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Linq;

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

        public static CatalogueSessionHistory ToEntity(this Models.UserCatalogueHistory model)
        {
            return new CatalogueSessionHistory(model.Id, model.CatalogueId, model.SessionDate, model.TimeSpent, model.IsComplete, model.IsErrorFree);
        }

        public static Entities.Question ToEntity(this Models.Question q, UserQuestionProgress uqp, List<Answer> answers, List<AnswerHistory> aHistories)
        {
            return new Entities.Question(q.Id, q.Text, q.DisplayName, uqp.CorrectAnswerCount, uqp.AnswerCount, q.CatalogueId, uqp.InPractice, answers, aHistories);
        }

        public static Answer ToEntity(this Models.Answer answer)
        {
            return new Answer(answer.Id, answer.Text, answer.QuestionId, answer.IsCorrect);
        }

        public static Models.Catalogue ToModel(this Entities.Catalogue catalogue)
        {
            // to do
            return new Models.Catalogue();
        }

        public static Models.UserCatalogueProgress ToModel(this Entities.Catalogue catalogue, Guid userId)
        {
            UserCatalogueProgress ucp = new UserCatalogueProgress
            {
                CatalogueId = catalogue.id,
                UserId = userId,
                CurrentQuestionId = catalogue.currentQuestionId,
                TimeSpent = catalogue.totalTimeSpent,
                Level = catalogue.sessionCount,
                ErrorFreeRuns = catalogue.errorFreeSessionCount,
                ErrorFreeRandomQuizCount = catalogue.errorFreeRandomQuizCount
            };

            return ucp;
        }

        public static Models.UserCatalogueHistory ToModel(this CatalogueSessionHistory sessionHistory, Guid userId)
        {
            UserCatalogueHistory uch = new UserCatalogueHistory
            {
                Id = sessionHistory.id,
                UserId = userId,
                CatalogueId = sessionHistory.catalogueId,
                SessionDate = sessionHistory.sessionDate,
                TimeSpent = sessionHistory.timeSpent,
                IsComplete = sessionHistory.isCompleted,
                IsErrorFree = sessionHistory.isErrorFree
            };

            return uch;
        }

        public static Models.UserQuestionProgress ToQuestionProgressModel(this Entities.Question question, Guid userId)
        {
            UserQuestionProgress uqp = new UserQuestionProgress
            {
                QuestionId = question.id,
                UserId = userId,
                AnswerCount = question.totalAnsweredCount,
                CorrectAnswerCount = question.correctAnsweredCount,
                InPractice = question.enabledForPractice
            };

            return uqp;
        }

        public static List<Models.UserQuestionHistory> ToQuestionHistoryModels(this Entities.Question question, Guid userId)
        {
            List<UserQuestionHistory> uqhModels = new List<UserQuestionHistory>();

            foreach (var answer in question.answerHistory)
            {
                UserQuestionHistory uqh = new UserQuestionHistory
                {
                    Id = answer.id,
                    UserId = userId,
                    QuestionId = question.id,
                    Date = answer.answerDate,
                    WasCorrect = answer.wasCorrect,
                    SessionId = answer.sessionId,
                };

                uqhModels.Add(uqh);
            }

            return uqhModels;
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
