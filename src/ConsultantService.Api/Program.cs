using Application;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using Infrastructure;
using ConsultantService.Api.Consumers;
using ConsultantService.Application.UseCases;
using ConsultantService.Infrastructure.Messaging.Consumers;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)        // reads your appsettings.json Serilog section
    .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
    .MinimumLevel.Override("RabbitMQ.Client", LogEventLevel.Debug)
    .CreateLogger();

builder.Host.UseSerilog();

var mbHost = builder.Configuration["MessageBroker:Host"];
var mbUser = builder.Configuration["MessageBroker:Username"];
var mbPass = builder.Configuration["MessageBroker:Password"];
Log.Information("RabbitMQ Config → Host: {Host}, User: {User}", mbHost, mbUser);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ICreateProfileUseCase, CreateProfileUseCase>();



//builder.Host.UseSerilog((context, conifguration) =>
//    conifguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CaseSubmittedConsumers>();
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.AddConsumer<UserRegisteredConsumer>();
    busConfigurator.AddConsumer<CreateConsultantProfileConsumer>();

    busConfigurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        cfg.ConfigureEndpoints(context);          // auto-create endpoints
    });
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
