using System;
using System.Threading.Tasks;
using AuthApi.Core.Models;

namespace AuthApi.Core.Services
{
    public class SyncService : BaseHttpService
    {
        string _database;

        public SyncService(string database) : base($"http://localhost:4985/")
        {
            _database = database;
        }

        public async Task<Session> GetSession(string username, string password)
        {
            var user = await GetUser(username).ConfigureAwait(false);

            string tenantId = GetTenantId(username);

            if (user == null)
            {
                user = new User
                {
                    name = username,
                    password = password,
                    email = username,
                    admin_channels = new string[] { tenantId },
                    //all_channels = new string[] { tenantId }
                };

                await CreateUser(user).ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                throw new Exception("User cannot be assigned to a business");
            }

            return await CreateSession(username, tenantId);
        }

        // For sample purposes only!
        string GetTenantId(string username)
        {
            string tenantId = null;

            if (username.Contains("@tenant1.com"))
            {
                tenantId = "tenant_1";
            }
            else if (username.Contains("@tenant2.com"))
            {
                tenantId = "tenant_2";
            }
            else if (username.Contains("@tenant3.com"))
            {
                tenantId = "tenant_3";
            }

            return tenantId;
        }

        public Task<User> GetUser(string name) => GetAsync<User>($"{_database}/_user/{name}");
        
        public Task CreateUser(User user) => PostAsync($"{_database}/_user/", user);

        public async Task<Session> CreateSession(string name, string tenantId)
        { 
            var session = await PostAsync<Session, SessionRequest>($"{_database}/_session", new SessionRequest { name = name }).ConfigureAwait(false);

            if (session != null)
            {
                session.TenantId = tenantId;
                return session;
            }

            throw new Exception("Error creating session!");
        }
    }
}
