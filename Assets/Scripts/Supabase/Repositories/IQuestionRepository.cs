using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Repositories
{
    public interface IQuestionRepository
    {
        Task<List<Models.Question>> GetQuestionsForCatalogue(int catalogueId);
        Task<Models.Question> GetQuestionById(int questionId);
        Task AddQuestion(Models.Question question, List<Models.Answer> answers);
        Task UpdateQuestionName(int questionId, string name);
        Task UpdateQuestionText(int questionId, string text);
        Task DeleteQuestion(int questionId);
    }
}
