using Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using Observability;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
    var bootstrapServers = context.Configuration["KAFKA:BOOTSTRAPSERVERS"] ?? "localhost:9092";
    var topic = context.Configuration["KAFKA:TOPIC"] ?? "transactions";
    var groupId = context.Configuration["KAFKA:GROUPID"] ?? "finstream-consumer";
    var connectionString = context.Configuration["DATABASE:CONNECTIONSTRING"] ?? "Host=localhost;Database=finstream;Username=finstream;Password=finstream";
    var otlpEndpoint = context.Configuration["OTEL:ENDPOINT"] ?? "http://localhost:4317";
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        services.AddSingleton(new EventRepository(connectionString));
        services.AddSingleton<TransactionProcessor>();
        services.AddSingleton(sp => new KafkaConsumer(kafkaConsumer, topic, sp.GetRequiredService<TransactionProcessor>()));
        services.AddHostedService<ConsumerWorker>();
        services.AddTelemetry("consumer", otlpEndpoint);
    })
    .Build();

await host.RunAsync();