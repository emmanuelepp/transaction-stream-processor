using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;
using Observability;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var bootstrapServers = context.Configuration["KAFKA:BOOTSTRAPSERVERS"] ?? "localhost:9092";
        var topic = context.Configuration["KAFKA:TOPIC"] ?? "transactions";
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };
        var otlpEndpoint = context.Configuration["OTEL:ENDPOINT"] ?? "http://localhost:4317";
        services.AddTelemetry("producer", otlpEndpoint);
        var kafkaProducer = new ProducerBuilder<string, string>(producerConfig).Build();
        services.AddSingleton<TransactionGenerator>();
        services.AddHostedService<ProducerWorker>();
        services.AddSingleton<KafkaProducer>(sp => new KafkaProducer(kafkaProducer, topic));
    })
    .Build();

await host.RunAsync();