using System.Threading.Tasks;
using MultiTenantSample.Core.Services;
using MultiTenantSample.Models;

namespace MultiTenantSample.Services
{
    public class AuthenticationService : BaseHttpService, IAuthenticationService
    {
        public AuthenticationService() : base($"http://localhost:5000/api/")
        { }

        //public Task<Session> Authenticate(Credentials credentials) => GetAsync<Session>($"authenticate?username={credentials.Username}&password={credentials.Password}");
        public Task<Session> Authenticate(Credentials credentials) => PostAsync<Session, Credentials>("authenticate", credentials);
    }
}
