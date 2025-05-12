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

        public CatalogueController(FetchCataloguesUseCase fetchCataloguesUseCase)
        {
            _fetchCataloguesUseCase = fetchCataloguesUseCase;
        }

        public async Task<List<CatalogueDTO>> GetCataloguesByTopic(string topic)
        {
            return await _fetchCataloguesUseCase.GetCataloguesByTopic(topic);
        }
    }
}
