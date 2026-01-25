using CA.API.Middlewares;
using CA.Application.Abstractions;
using CA.Application.DomainEvents.Handler;
using CA.Application.Interfaces;
using CA.Application.Services;
using CA.Domain.Events;
using CA.Domain.IRepositories;
using CA.Infrastructures.EventBus;
using CA.Infrastructures.Persistence;
using CA.Infrastructures.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();
