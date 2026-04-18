using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Observability;
using Producer;

var builder = WebApplication.CreateBuilder(args);

var bootstrapServers = builder.Configuration["KAFKA:BOOTSTRAPSERVERS"] ?? "localhost:9092";
var topic = builder.Configuration["KAFKA:TOPIC"] ?? "transactions";
var otlpEndpoint = builder.Configuration["OTEL:ENDPOINT"] ?? "http://localhost:4317";

var producerConfig = new ProducerConfig { BootstrapServers = bootstrapServers };
var kafkaProducer = new ProducerBuilder<string, string>(producerConfig).Build();

builder.Services.AddTelemetry("producer", otlpEndpoint);
builder.Services.AddSingleton<TransactionGenerator>();
builder.Services.AddSingleton<KafkaProducer>(sp => new KafkaProducer(kafkaProducer, topic));
builder.Services.AddHostedService<ProducerWorker>();

var app = builder.Build();
app.MapPrometheusScrapingEndpoint();

await app.RunAsync();