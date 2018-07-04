﻿using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.EventStorage.EFCore
{
    public class EventStorageSQLRepository : IEventStorageRepository, IMementoRepository
    {
        private readonly EventStorageSQLContext context;

        public EventStorageSQLRepository(EventStorageSQLContext context)
        {
            this.context = context;
        }

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey eventId, int afterVersion = 0)
            where TKey : IEquatable<TKey>
        {
            return context.Set<StoredEvent>().Where(e => e.EventId == eventId.ToString() && e.Version >= afterVersion);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public StoredEvent GetLastEvent<TKey>(TKey eventId)
            where TKey : IEquatable<TKey>
        {
            return context.Set<StoredEvent>().Where(e => e.EventId == eventId.ToString()).OrderBy(e => e.Timestamp).LastOrDefault();
        }

        public void Store(StoredEvent theEvent)
        {
            context.Set<StoredEvent>().Add(theEvent);
            context.SaveChanges();
        }

        public Memento GetMemento<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            return context.Set<Memento>().Where(m => m.AggregateId == aggregateId.ToString()).OrderBy(m => m.Version).LastOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            context.Set<Memento>().Add(memento);
            context.SaveChanges();
        }
    }
}