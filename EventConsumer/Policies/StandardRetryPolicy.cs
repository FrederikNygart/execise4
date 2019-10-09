using Microsoft.AspNetCore.Http;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventConsumer.Policies
{
    public static class StandardRetryPolicy
    {
        public static void StandardRetry(Action someaction)
        {
            Policy
              .Handle<Exception>()
              .Retry(2)
              .Execute(someaction);

        }
    }
}
