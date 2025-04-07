using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supabase;

namespace Services
{
    public static class SupabaseClientProvider
    {
        private static Client _client;

        public static Client GetClient()
        {
            if (_client == null)
            {
                _client = new Client(Strings.SupabaseUrl, Strings.SupabasePublicKey);
                _client.InitializeAsync().Wait();
            }
            return _client;
        }
    }
}
