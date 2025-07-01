using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client = Supabase.Client;
using UseCases;
using System.Threading.Tasks;
using Entities;

namespace Controllers
{
    public class TopicController
    {
        private FetchTopicsUseCase _fetchTopicsUseCase;

        public TopicController(FetchTopicsUseCase fetchTopicUseCase)
        {
            _fetchTopicsUseCase = fetchTopicUseCase;
        }

        public async Task<List<Topic>> GetAllTopics()
        {
            return await _fetchTopicsUseCase.GetAllTopics();
        }
    }
}
