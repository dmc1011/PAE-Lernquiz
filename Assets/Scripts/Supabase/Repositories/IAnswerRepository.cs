using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Repositories
{
    public interface IAnswerRepository
    {
        Task<List<Models.Answer>> GetAnswersForQuestion(int questionId);
        Task AddAnswer(Models.Answer answer);
        Task UpdateAnswerText(int answerId, string text);

        // Achievements Repo?
        Task<int> GetCorrectAnswerCountForCatalogue(int catalogueId);
    }
}

