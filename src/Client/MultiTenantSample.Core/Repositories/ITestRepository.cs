using System;
using System.Collections.Generic;
using MultiTenantSample.Models;

namespace MultiTenantSample.Core.Repositories
{
    public interface ITestRepository : IRepository
    {
        List<Item> GetItems(Action<List<Item>> userProfileUpdated);
        void SaveItem(Item item);
        void DeleteItem(Item item);
    }
}
