using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Nancy;
using Nancy.ModelBinding;

namespace ShoppingCart
{
    public class Cart : NancyModule
    {
        public Cart(IDocumentStore documentStore, ShoppingCartEventStore eventStore) : base("/shoppingcart")
        {

            // TODO: GET, ADD and Delete
            Get("/{userId:int}", async parameters =>
            {
                int userId = parameters.userId;
                using (var DB = documentStore.LightweightSession())
                {
                    var cart = await DB.LoadAsync<ShoppingCart>(userId);
                    return cart ?? (object)HttpStatusCode.NotFound;
                }
            });

            Post("/{userId:int}/items", async parameters =>
            {
                int userId = parameters.userId;
                IEnumerable<Product> products = this.Bind();

                using (var db = documentStore.LightweightSession())
                {
                    var cart = await db.LoadAsync<ShoppingCart>(userId) ?? new ShoppingCart { Id = userId, Products = new List<Product>()};
                    cart.Products.AddRange(products);
                    db.Store(cart);
                    await db.SaveChangesAsync();
                    foreach(var product in products)
                    {
                        eventStore.RaiseProductAddedToCart(cart, product);
                    }
                    return cart;
                }
            });

            Delete("/{userId:int}/items", async parameters =>
            {
                var product = this.Bind<Product>();
                int userId = parameters.userId;

                using (var session = documentStore.LightweightSession())
                {
                    session.Delete<ShoppingCart>((int)parameters.userId);

                    await session.SaveChangesAsync();

                }
                return HttpStatusCode.NoContent;

            });
        }

    }
    
    public class Product
    {
        public string Name { get; set;}
    }

    public class ShoppingCart
    {
        public int Id;
        public List<Product> Products { get; set; }
    }
}