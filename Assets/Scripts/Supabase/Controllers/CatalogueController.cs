using Entities;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UseCases;

namespace Controllers
{
    public class CatalogueController
    {
        private FetchCataloguesUseCase _fetchCataloguesUseCase;
        private SupabaseRequestUseCase _supabaseRequestUseCase;

        public CatalogueController(FetchCataloguesUseCase fetchCataloguesUseCase, SupabaseRequestUseCase supabaseRequestUseCase)
        {
            _fetchCataloguesUseCase = fetchCataloguesUseCase;
            _supabaseRequestUseCase = supabaseRequestUseCase;
        }

        public async Task<List<CatalogueDTO>> GetCataloguesByTopic(string topic)
        {
            return await _fetchCataloguesUseCase.GetCataloguesByTopic(topic);
        }

        public async Task<Catalogue> GetCatalogueById(int id)
        {
            return await _fetchCataloguesUseCase.GetCatalogueById(id);
        }

        public async Task UpdateCatalogueUserData(Catalogue catalogue)
        {
            await _supabaseRequestUseCase.UpdateCatalogueUserData(catalogue);
        }

        public async Task StoreTopic(Models.Topic topic)
        {
            await _supabaseRequestUseCase.StoreTopic(topic);
        }

        public async Task UpdateCatalogue(Catalogue catalogue)
        {
            await _supabaseRequestUseCase.UpdateCatalogue(catalogue);
        }

        public async Task DeleteCatalogue(int catalogueId)
        {
            await _supabaseRequestUseCase.DeleteCatalogue(catalogueId);
        }

        public async Task DeleteQuestion(int questionId)
        {
            await _supabaseRequestUseCase.DeleteQuestion(questionId);
        }

        public async Task UpdateTopic(Models.Topic newTopic, string oldTopicName)
        {
            await _supabaseRequestUseCase.UpdateTopic(newTopic, oldTopicName);
        }

        public async Task DeleteTopic(string topicName)
        {
            await _supabaseRequestUseCase.DeleteTopic(topicName);
        }
    }
}
