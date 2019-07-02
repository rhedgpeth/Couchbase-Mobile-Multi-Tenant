﻿using System;
using System.Linq;
using Couchbase.Lite;
using Couchbase.Lite.Sync;

namespace MultiTenantSample.Repositories
{
    public class DatabaseManager : IDisposable
    {
        // Note: User 'localhost' when using a simulator
        readonly Uri _remoteSyncUrl = new Uri("ws://localhost:4984");

        // Note: Use '10.0.2.2' when using an emulator
        //readonly Uri _remoteSyncUrl = new Uri("ws://10.0.2.2:4984");

        readonly string _databaseName;

        Replicator _replicator;
        ListenerToken _replicatorListenerToken;

        Database _database;
        public Database Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new Database(_databaseName);
                }

                return _database;
            }
        }

        public DatabaseManager(string databaseName)
        {
            _databaseName = databaseName;
        }

        public void StartReplication(string sessionId,
                                     string[] channels,
                                     ReplicatorType replicationType = ReplicatorType.PushAndPull,
                                     bool continuous = true)
        {
            var targetUrlEndpoint = new URLEndpoint(new Uri(_remoteSyncUrl, _databaseName));

            var configuration = new ReplicatorConfiguration(Database, targetUrlEndpoint) 
            {
                ReplicatorType = replicationType, 
                Continuous = continuous, 
                Authenticator = new SessionAuthenticator(sessionId),
                Channels = channels
            };

            _replicator = new Replicator(configuration);

            _replicatorListenerToken = _replicator.AddChangeListener(OnReplicatorUpdate);

            _replicator.Start();
        }

        void OnReplicatorUpdate(object sender, ReplicatorStatusChangedEventArgs e)
        {
            var status = e.Status;

            switch (status.Activity)
            {
                case ReplicatorActivityLevel.Busy:
                    Console.WriteLine("Busy transferring data.");
                    break;
                case ReplicatorActivityLevel.Connecting:
                    Console.WriteLine("Connecting to Sync Gateway.");
                    break;
                case ReplicatorActivityLevel.Idle:
                    Console.WriteLine("Replicator in idle state.");
                    break;
                case ReplicatorActivityLevel.Offline:
                    Console.WriteLine("Replicator in offline state.");
                    break;
                case ReplicatorActivityLevel.Stopped:
                    Console.WriteLine("Completed syncing documents.");
                    break;
            }

            if (status.Progress.Completed == status.Progress.Total)
            {
                Console.WriteLine("All documents synced.");
            }
            else
            {
                Console.WriteLine($"Documents {status.Progress.Total - status.Progress.Completed} still pending sync");
            }
        }

        public void StopReplication()
        {
            _replicator.RemoveChangeListener(_replicatorListenerToken);
            _replicator.Stop();
        }

        public void Dispose()
        {
            if (_replicator != null)
            {
                StopReplication();

                // Because the 'Stop' method for a Replicator instance is asynchronous 
                // we must wait for the status activity to be stopped before closing the database. 
                while (true)
                {
                    if (_replicator.Status.Activity == ReplicatorActivityLevel.Stopped)
                    {
                        break;
                    }
                }

                _replicator.Dispose();
            }

            _database.Close();
            _database = null;
        }
    }
}
