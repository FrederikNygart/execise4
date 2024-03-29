﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using EventConsumer.Policies;
using Newtonsoft.Json;
using static System.Console;

namespace EventConsumer
{
  public class EventSubscriber
  {
    private readonly string shoppingCartHost;
    private long start = 0, chunkSize = 100;
        private int retryCount = 0;
        private readonly Timer timer;

    public EventSubscriber(string shoppingCartHost)
    {
            this.shoppingCartHost = shoppingCartHost;
            this.timer = new Timer(10 * 1000);
                            WriteLine("subscriber created");
                this.timer.AutoReset = false;
            this.timer.Elapsed += (_, __) =>
            {
                StandardRetryPolicy.StandardRetry(() =>
                {
                    SubscriptionCycleCallback().Wait();
                    retryCount++;
                });

            };
    }

    private async Task SubscriptionCycleCallback()
    {
      WriteLine($"Subscribtion callback {retryCount}");
      var response = await ReadEvents().ConfigureAwait(false);
      if (response.StatusCode == HttpStatusCode.OK)
        HandleEvents(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
      this.timer.Start();
    }

    private async Task<HttpResponseMessage> ReadEvents()
    {
      using (var httpClient = new HttpClient())
      {
        httpClient.BaseAddress = new Uri($"http://{this.shoppingCartHost}");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var resource = $"/events/?from={this.start}&to={this.start + this.chunkSize}";
        var response = await httpClient.GetAsync(resource).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.InternalServerError) throw new Exception();
        PrettyPrintResponse(response);
        return response;
      }
    }

    private void HandleEvents(string content)
    {
      WriteLine("Handling events");
      var events = JsonConvert.DeserializeObject<IEnumerable<Event>>(content);
      WriteLine($"Number of events to handle: {events.Count()}");
      foreach (var ev in events)
      {
        WriteLine($"Event number {ev.SequenceNumber}:");
        WriteLine(ev.Data);
        this.start = 0;
      }
    }


    public void Start()
    {
      this.timer.Start();
    }

    public void Stop()
    {
      this.timer.Stop();
    }

    private static async void PrettyPrintResponse(HttpResponseMessage response)
    {
      WriteLine("Status code: " + response?.StatusCode.ToString() ?? "command failed");
      WriteLine("Headers: " + response?.Headers.Aggregate("", (acc, h) => acc + "\n\t" + h.Key + ": " + h.Value) ?? "");
      WriteLine("Body: " + await response?.Content.ReadAsStringAsync() ?? "");
    }
  }

  public struct Event
  {
    public DateTime SequenceNumber { get; set; }
    public object Data { get; set; }
  } 



    class Program
    {
        static void Main(string[] args)
        {
          var subscriber = new EventSubscriber("localhost:32090");
          subscriber.Start();
          Console.ReadLine();
        }
    }
}
