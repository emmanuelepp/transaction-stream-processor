using Confluent.Kafka;
using Consumer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Observability;

var builder = WebApplication.CreateBuilder(args);

var bootstrapServers = builder.Configuration["KAFKA:BOOTSTRAPSERVERS"] ?? "localhost:9092";
var topic = builder.Configuration["KAFKA:TOPIC"] ?? "transactions";
var groupId = builder.Configuration["KAFKA:GROUPID"] ?? "finstream-consumer";
var connectionString = builder.Configuration["DATABASE:CONNECTIONSTRING"] ?? "Host=localhost;Database=finstream;Username=finstream;Password=finstream";
var otlpEndpoint = builder.Configuration["OTEL:ENDPOINT"] ?? "http://localhost:4317";

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

builder.Services.AddTelemetry("consumer", otlpEndpoint);
builder.Services.AddSingleton(new EventRepository(connectionString));
builder.Services.AddSingleton<TransactionProcessor>();
builder.Services.AddSingleton(sp => new KafkaConsumer(kafkaConsumer, topic, sp.GetRequiredService<TransactionProcessor>()));
builder.Services.AddHostedService<ConsumerWorker>();

var app = builder.Build();
app.MapPrometheusScrapingEndpoint();

await app.RunAsync();