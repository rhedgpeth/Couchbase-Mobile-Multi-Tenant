using System;
using System.Threading.Tasks;
using MultiTenantSample.Core;
using MultiTenantSample.Core.Repositories;

namespace MultiTenantSample.Repositories
{
    public abstract class BaseRepository : IRepository
    {
        readonly string _databaseName;

        protected DatabaseManager _databaseManager;
        protected DatabaseManager DatabaseManager
        {
            get
            {
                if (_databaseManager == null)
                {
                    _databaseManager = new DatabaseManager(_databaseName);
                }

                return _databaseManager;
            }
        }

        protected BaseRepository(string databaseName)
        {
            _databaseName = databaseName;
        }

        public Task StartReplication()
        {
            return Task.Run(() => DatabaseManager.StartReplication(AppInstance.CurrentSession.session_id,
                                                                   new string[] { AppInstance.CurrentSession.TenantId }));
        }

        public virtual void Dispose() => DatabaseManager?.Dispose();
    }
}