using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Entities;
using Repositories;

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
    }
}