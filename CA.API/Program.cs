using CA.API.Middlewares;
using CA.Application.Abstractions;
using CA.Application.Abstractions.DTO;
using CA.Application.DomainEvents.Handler;
using CA.Application.Interfaces;
using CA.Application.Services;
using CA.Domain.Events;
using CA.Domain.IRepositories;
using CA.Infrastructures.EventBus;
using CA.Infrastructures.EventBus.RabbiqMq.DTO;
using CA.Infrastructures.MessageBroker.Kafka;
using CA.Infrastructures.MessageBroker.RabbiqMq;
using CA.Infrastructures.OutboxEvent;
using CA.Infrastructures.Persistence;
using CA.Infrastructures.Persistence.Interfaces;
using CA.Infrastructures.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

var serviceName = "loan-service";
//var serviceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);


var endpoint = builder.Configuration["Otel:Endpoint"]
               ?? "http://localhost:4318";

Console.WriteLine("OTEL ENDPOINT = " + endpoint);

builder.Logging.AddOpenTelemetry(x =>
{
    x.IncludeFormattedMessage = true;
    x.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "loan-service"))
    .WithTracing(tracing => tracing
        .SetSampler(new AlwaysOnSampler())
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("loan-service")
        .AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(endpoint);
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
            //opt.ExportProcessorType = ExportProcessorType.Batch;
            opt.HttpClientFactory = () =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return new HttpClient(handler);
            };
        })
     );

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
builder.Services.AddScoped<IInMemoryDomainEventBus, InMemoryDomainEventBus>();

builder.Services.AddScoped<IDomainEventHandler<LoanApplicationCreatedDomainEvent>, LoanCreatedAuditHandler>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IIntegrationEventPublisher, OutboxEventPublisher>();
builder.Services.AddScoped<IApiIdempotencyRepository, ApiIdempotencyRepository>();


builder.Services.AddSingleton<RabbitMqMessageQueue>();
builder.Services.AddSingleton<KafkaMessageQueue>();

builder.Services.AddSingleton<IMessageQueue, MessageBroker>();


builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.Configure<KafkaOptions>(
    builder.Configuration.GetSection("Kafka"));


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.MapGet("/trace-test", () => "trace created");

app.Run();
