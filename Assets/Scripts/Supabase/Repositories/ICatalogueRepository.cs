using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Entities;

namespace Repositories
{
    public interface ICatalogueRepository
    {
        Task<List<CatalogueDTO>> GetCataloguesByTopic(string topic);
        //Task<Catalogue> GetCatalogueByNmae(string name);
        //Task<List<Catalogue>> GetAllCatalogues();
        Task<Catalogue> GetCatalogueById(int id);
        //Task<Catalogue> GetRandomCatalogue();

        //Task AddCatalogue(Models.Catalogue catalogue, List<Models.Question> questions);
        Task UpdateCatalogue(Entities.Catalogue catalogue);
        Task UpdateCatalogueUserData(Catalogue catalogue);
        //Task DeleteCatalogue(int catalogueId);
    }
}