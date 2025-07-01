using Repositories;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Entities;

namespace UseCases
{
    public class FetchTopicsUseCase
    {
        private ITopicRepository _repo;

        public FetchTopicsUseCase(ITopicRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Topic>> GetAllTopics()
        {
            return await _repo.GetAllTopics();
        }
    }
}
