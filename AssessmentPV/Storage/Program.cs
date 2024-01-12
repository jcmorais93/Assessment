using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using Storage.Services;

namespace Storage;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<IRepository, Repository>();
        
        var configuration = builder.Configuration;

        builder.Services.AddSingleton<IConsumer<Null, string>>(_ =>
        {
            var config = new ConsumerConfig
            {
                GroupId = "pixel",
                BootstrapServers = configuration["StorageEndpoint"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
            return new ConsumerBuilder<Null, string>(config).Build();
        });

        var app = builder.Build();

        app.MapGet("/", Handler);

        app.Run();
        return;


        async Task Handler(IConsumer<Null, string> kafkaConsumer, IRepository repository)
        {
            await CreateTopicIfNotExists("pixel-topic", configuration["StorageEndpoint"]);
            kafkaConsumer.Subscribe("pixel-topic");

            while (true)
            {
                var consumeResult = kafkaConsumer.Consume();
                if (consumeResult == null) continue;

                var logData = consumeResult.Message.Value;
                var tracker = JsonConvert.DeserializeObject<Tracker>(logData);

                repository.LogData(tracker);
            }
        }

        async Task CreateTopicIfNotExists(string topic, string bootstrapServers)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();

            try
            {
                await adminClient.CreateTopicsAsync(new[]
                    { new TopicSpecification { Name = topic, NumPartitions = 1, ReplicationFactor = 1 } });
                Console.WriteLine($"Topic '{topic}' created successfully.");
            }
            catch (CreateTopicsException) { }
        }
    }
}