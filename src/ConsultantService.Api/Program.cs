using Application;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);



builder.Host.UseSerilog((context, conifguration) =>
    conifguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMassTransit(busConfigurator =>
{
    //x.AddConsumer<UserRegisteredConsumer>();
    //x.AddConsumer<CaseSubmittedConsumers>();
    busConfigurator.SetKebabCaseEndpointNameFormatter();

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
