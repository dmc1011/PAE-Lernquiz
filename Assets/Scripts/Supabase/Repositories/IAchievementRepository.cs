using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Repositories
{
    public interface IAchievementRepository
    {
        Task<int> GetCorrectAnswerCountForCatalogue(int catalogueId);
    }
}