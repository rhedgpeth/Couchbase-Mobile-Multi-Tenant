using System;
using System.Threading.Tasks;

namespace MultiTenantSample.Core.Repositories
{
    public interface IRepository : IDisposable
    {
        Task StartReplication();
    }
}
