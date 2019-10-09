using System;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace ShoppingCart
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseSerilog()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
