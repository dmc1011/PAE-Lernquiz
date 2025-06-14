using Entities;
using System.Collections;
using System.Collections.Generic;
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
    }
}
