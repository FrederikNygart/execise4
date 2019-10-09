using Marten;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ShoppingCart
{

    public class EventStore<TEvent> where TEvent : class
    {
        private static long currentSequenceNumber = 0;
        private IDocumentStore documentStore;
        internal string eventName;

        public EventStore(IDocumentStore documentStore, string eventName)
        {
            this.documentStore = documentStore;
            this.eventName = eventName;
        }
    }

    public class Event
    {
        public Guid Id { get; }
        public long SequenceNumber { get; }
        public DateTimeOffset OccuredAt { get; }
        public string Name { get; }

        public Event(Guid id, long sequenceNumber, DateTimeOffset occuredAt, string name)
        {
            this.Id = id;
            this.SequenceNumber = sequenceNumber;
            this.OccuredAt = occuredAt;
            this.Name = name;
        }
    }


}
