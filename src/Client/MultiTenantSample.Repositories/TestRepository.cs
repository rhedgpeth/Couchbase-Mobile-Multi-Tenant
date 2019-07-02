using System;
using System.Collections.Generic;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using MultiTenantSample.Core;
using MultiTenantSample.Core.Repositories;
using MultiTenantSample.Models;

namespace MultiTenantSample.Repositories
{
    public sealed class TestRepository : BaseRepository, ITestRepository
    {
        public TestRepository() : base("test")
        { }

        IQuery _userQuery;
        ListenerToken _userQueryToken;

        public List<Item> GetItems(Action<List<Item>> userProfileUpdated)
        {
            List<Item> items = new List<Item>();

            try
            {
                var database = DatabaseManager.Database;

                if (database != null)
                {
                    _userQuery = QueryBuilder
                                    .Select(SelectResult.All())
                                    .From(DataSource.Database(database))
                                    .Where((Expression.Property("type").EqualTo(Expression.String("item"))));
                                           //.And(Expression.String("tenant_2").In(Expression.Property("channels"))));
                                       
                    if (userProfileUpdated != null)
                    {
                        _userQueryToken = _userQuery.AddChangeListener((object sender, QueryChangedEventArgs e) => 
                        {
                            if (e?.Results != null && e.Error == null)
                            {
                                items = e.Results.AllResults()?.ToObjects<Item>("test") as List<Item>;

                                if (items != null)
                                {
                                    userProfileUpdated.Invoke(items);
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestRepository Exception: {ex.Message}");
            }

            return items;
        }

        public void SaveItem(Item item) => DatabaseManager.Database.Save(item.ToMutableDocument($"item::{item.Name}"));

        public void DeleteItem(Item item)
        {
            var document = DatabaseManager.Database.GetDocument($"item::{item.Name}");

            if (document != null)
            {
                DatabaseManager.Database.Delete(document);
            }
        }

        public override void Dispose()
        {
            // Remove the live query change listener
            _userQuery.RemoveChangeListener(_userQueryToken);

            base.Dispose();
        }
    }
}

