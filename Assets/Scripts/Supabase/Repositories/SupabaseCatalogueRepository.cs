using Postgrest;
using Repositories;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;
using Entities;
using System.Linq;
using DataConversion;
using System;
using System.Text.Json;

public class SupabaseCatalogueRepository : ICatalogueRepository
{
    private Client _client;

    public SupabaseCatalogueRepository(Client client)
    {
        _client = client;
    }

    public async Task<List<CatalogueDTO>> GetCataloguesByTopic(string topic)
    {
        try
        {
            var result = await _client.From<Models.Catalogue>().Where(c => c.TopicName == topic).Where(c => c.IsPrivate == false).Get();

            List<Models.Catalogue> models = result.Models;
            List<CatalogueDTO> catalogues = models.Select(model => model.ToDTO()).ToList();

            return catalogues;
        }
        catch (Exception e)
        {
            throw new FetchDataException("Error fetching catalogues: " + e.Message);
        }
    }

    public async Task<Catalogue> GetCatalogueById(int catalogueId)

    {
        try
        {
            var currentUser = _client.Auth.CurrentUser?.Id;

            if (currentUser == null)
            {
                throw new FetchDataException("No current user exists while trying to fetch catalogue data");
            }

            var result = await _client.Rpc("get_full_catalogue", new { p_user_id = currentUser, p_catalogue_id = catalogueId});

            Catalogue c = JsonSerializer.Deserialize<Catalogue>(result.Content);

            return c;
        }
        catch (Exception e)
        {
            throw new FetchDataException("Fehler beim Laden von Katalog " + catalogueId + ": " + e.Message);
        }
    }
}
