using System;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;
using NATS.Net;

public record StockTick(string Symbol, decimal Price, DateTime Timestamp);

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("********* NATS.Net Base Project *********");

        var opts = NatsOpts.Default with
        {
            SerializerRegistry = NatsJsonSerializerRegistry.Default
        };


        await using var nats = new NatsConnection(opts);

        using var cts = new CancellationTokenSource();

        const string subjectName = "stocks.updates";

        var subscriberTask = Task.Run(async () =>
        {
            Console.WriteLine($"[Sub] Starting background subscriber on subject '{subjectName}'...");

            await foreach (var msg in nats.SubscribeAsync<StockTick>(subject: subjectName, cancellationToken: cts.Token))
            {
                var tick = msg.Data;
                if (tick != null)
                {
                    Console.WriteLine($"[Sub] Received Alert -> {tick.Symbol}: ${tick.Price} at {tick.Timestamp:HH:mm:ss}");
                }
            }
        }, cts.Token);

        await Task.Delay(500);

        Console.WriteLine("[Pub] Press any key to start publishing messages. Press 'Ctrl + C' to exit.");
        Console.ReadLine();

        var random = new Random();
        decimal currentPrice = 150.00m;

        for (int i = 1; i <= 10; i++)
        {
            currentPrice += (decimal)(random.NextDouble() * 2 - 1);
            var mockTick = new StockTick("MSFT", Math.Round(currentPrice, 2), DateTime.Now);

            Console.WriteLine($"[Pub] Publishing tick #{i} to NATS...");

            await nats.PublishAsync<StockTick>(subject: subjectName, data: mockTick);

            await Task.Delay(1000);
        }

        Console.WriteLine("\nFinished publishing 10 ticks. Cleaning up...");
        cts.Cancel();

        try
        {
            await subscriberTask;
        }
        catch (OperationCanceledException)
        {

        }

        Console.WriteLine("Disconnected cleanly. Goodbye!");

    }
}