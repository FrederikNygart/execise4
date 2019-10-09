using Marten;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public class ShoppingCartEventStore
    {
        private static readonly IList<ShoppingCartEvent> database = new List<ShoppingCartEvent>();
        private IDocumentStore documentStore;

        public const string PRODUCT_ADDED_TO_CART = "PRODUCT_ADDED_TO_CART";

        public ShoppingCartEventStore(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        internal void RaiseProductAddedToCart(ShoppingCart cart, Product product)
        {
            var seqNumber = DateTime.Now;
            var id = Guid.NewGuid();
            var @event = new ShoppingCartEvent(id, seqNumber, DateTimeOffset.UtcNow, PRODUCT_ADDED_TO_CART, cart,product);
            //database.Add(@event);

            using (var session = documentStore.LightweightSession())
            {
                session.Store(@event);
                session.SaveChanges();
            }
        }

        internal void DeleteEvents()
        {
            using (var session = documentStore.LightweightSession())
            {
                session.DeleteWhere<ShoppingCartEvent>(x => x.Name == PRODUCT_ADDED_TO_CART);
                session.SaveChanges();
            }
        }

        public IEnumerable<ShoppingCartEvent> GetEvents(string eventName, string start, string end)
        {
            var from = long.Parse(start);
            var to = long.Parse(end);
            using (var session = documentStore.LightweightSession())
            {
                return session.Query<ShoppingCartEvent>().ToList();
            }


            //return database
            //.Where(e => e.Name.Equals(eventType))
            //    .Where(e => e.SequenceNumber >= start && e.SequenceNumber <= end)
            //    .OrderBy(e => e.SequenceNumber);
        }
    }

    public class ShoppingCartEvent
    {
        public Guid Id { get; }
        public DateTime SequenceNumber { get; }
        public DateTimeOffset OccuredAt { get; }
        public string Name { get; }
        public ShoppingCart Cart { get; }
        public Product Product { get; set; }

        public ShoppingCartEvent(Guid id, DateTime sequenceNumber, DateTimeOffset occuredAt, string name, ShoppingCart cart, Product product)
        {
            this.Id = id;
            this.SequenceNumber = sequenceNumber;
            this.OccuredAt = occuredAt;
            this.Name = name;
            this.Cart = cart;
            this.Product = product;
        }
    }
}
