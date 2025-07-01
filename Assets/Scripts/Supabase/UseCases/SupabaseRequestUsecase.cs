using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Entities;
using Repositories;
using Cysharp.Threading.Tasks;

namespace UseCases
{
    // to do: split into several usecases
    public class SupabaseRequestUseCase
    {
        private ICatalogueRepository _repo;

        public SupabaseRequestUseCase(ICatalogueRepository repo)
        {
            _repo = repo;
        }

        public async Task UpdateCatalogueUserData(Catalogue catalogue)
        {
            await _repo.UpdateCatalogueUserData(catalogue);
        }

        public async Task StoreTopic(Models.Topic topic)
        {
            await _repo.StoreTopic(topic);
        }

        public async Task UpdateCatalogue(Catalogue catalogue)
        {
            await _repo.UpdateCatalogue(catalogue);
        }

        public async Task DeleteCatalogue(int catalogueId)
        {
            await _repo.DeleteCatalogue(catalogueId);
        }

        public async Task DeleteQuestion(int questionId)
        {
            await _repo.DeleteQuestion(questionId);
        }

        public async Task UpdateTopic(Models.Topic newTopic, string oldTopicName)
        {
            await _repo.UpdateTopic(newTopic, oldTopicName);
        }

        public async Task DeleteTopic(string topicName)
        {
            await _repo.DeleteTopic(topicName);
        }
    }
}