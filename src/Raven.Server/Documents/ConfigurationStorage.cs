﻿using System;
using System.IO;
using Raven.Server.Documents.Indexes;
using Raven.Server.Documents.Transformers;
using Raven.Server.NotificationCenter;
using Raven.Server.ServerWide.Context;
using Voron;

namespace Raven.Server.Documents
{
    public class ConfigurationStorage : IDisposable
    {
        private TransactionContextPool _contextPool;

        public TransactionContextPool ContextPool => _contextPool;

        public IndexesEtagsStorage IndexesEtagsStorage { get; }

        public ActionsStorage ActionsStorage { get; }

        public StorageEnvironment Environment { get; }

        public ConfigurationStorage(DocumentDatabase db)
        {
            var options = db.Configuration.Core.RunInMemory
                ? StorageEnvironmentOptions.CreateMemoryOnly(Path.Combine(db.Configuration.Core.DataDirectory, "Configuration"))
                : StorageEnvironmentOptions.ForPath(Path.Combine(db.Configuration.Core.DataDirectory, "Configuration"));

            options.SchemaVersion = 1;

            Environment = new StorageEnvironment(options);
            
            ActionsStorage = new ActionsStorage(db.Name);
            
            IndexesEtagsStorage = new IndexesEtagsStorage(db.Name);

            _contextPool = new TransactionContextPool(Environment);
        }

        public void InitializeActionsStorage()
        {
            ActionsStorage.Initialize(Environment, _contextPool);
        }

        public void InitializeIndexesEtagsStorage(IndexStore indexStore, TransformerStore transformerStore)
        {
            IndexesEtagsStorage.Initialize(Environment, _contextPool, indexStore, transformerStore);
        }

        public void Dispose()
        {
            _contextPool?.Dispose();
            Environment.Dispose();
        }
    }
}