using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Entities;
using Repositories;

namespace UseCases
{
    public class FetchCataloguesUseCase
    {
        private ICatalogueRepository _repo;

        public FetchCataloguesUseCase(ICatalogueRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CatalogueDTO>> GetCataloguesByTopic(string topicName)
        {
            return await _repo.GetCataloguesByTopic(topicName);
        }
    }
}
