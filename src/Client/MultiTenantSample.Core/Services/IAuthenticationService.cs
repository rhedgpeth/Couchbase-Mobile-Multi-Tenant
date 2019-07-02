using System.Threading.Tasks;
using MultiTenantSample.Models;

namespace MultiTenantSample.Core.Services
{
    public interface IAuthenticationService
    {
        Task<Session> Authenticate(Credentials credentials);
    }
}
