using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Client = Supabase.Client;


namespace Controllers
{
    public class DataController
    {
        public async Task<Models.Topic> InsertTopic(int id, string name, Client client)
        {
            try
            {
                var topic = new Models.Topic
                {
                    Name = name,
                };

                var insert = await client.From<Models.Topic>().Insert(topic);
                return insert.Model;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return null;
            }
        }
        
    }
}
