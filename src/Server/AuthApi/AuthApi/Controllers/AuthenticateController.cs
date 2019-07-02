using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthApi.Core.Models;
using AuthApi.Core.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticateController : Controller
    {
        const string BucketName = "test";

        SyncService syncService;
        SyncService SyncService
        {
            get
            {
                if (syncService == null)
                {
                    syncService = new SyncService(BucketName);
                }

                return syncService;
            }
        }

        [HttpGet]
        public Task<Session> Get(string username, string password)
        {
            return SyncService.GetSession(username, password);
        }

        [HttpPost]
        public Task<Session> Post([FromBody]Models.AuthRequest credentials)
        {
            if (!string.IsNullOrEmpty(credentials?.Username) &&
                !string.IsNullOrEmpty(credentials?.Password))
            {
                return SyncService.GetSession(credentials.Username, credentials.Password);
            }

            throw new InvalidOperationException("Invalid credentials!");
        }
    }
}
