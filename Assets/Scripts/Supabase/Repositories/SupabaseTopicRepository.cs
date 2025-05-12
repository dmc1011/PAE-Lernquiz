using Postgrest;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;
using DataConversion;
using Entities;
using System.Linq;

namespace Repositories
{
    public class  SupabaseTopicRepository : ITopicRepository
    {
        private Client _client;

        public SupabaseTopicRepository(Client client)
        {
            _client = client;
        }

        public async Task<List<Topic>> GetAllTopics()
        {
            try
            {
                var result = await _client.From<Models.Topic>().Get();

                List<Models.Topic> models = result.Models;
                List<Topic> topics = models.Select(model => model.ToEntity()).ToList();

                return topics;
            }
            catch (Exception e)
            {
                throw new FetchDataException("Error fetching Topics: " + e.Message);
            }
        }
    }
}