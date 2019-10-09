using Nancy;

namespace ShoppingCart
{
    public class EventFeed : NancyModule
    {
        public EventFeed(ShoppingCartEventStore eventStore) : base("events")
        {

            Get("/", _ =>
            {
                //return eventStore.GetEvents("PRODUCT_ADDED_TO_CART",
                //   this.Request.Query.From.Value,
                //    this.Request.Query.To.Value);

                return HttpStatusCode.InternalServerError;
            });

            Delete("/", _ =>
            {
                eventStore.DeleteEvents();
                return HttpStatusCode.OK;
            });
        }
    }
}