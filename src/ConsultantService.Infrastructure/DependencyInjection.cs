using Application.Common.Persistence;
using ConsultantService.Api.Consumers;
using ConsultantService.Application.Contracts;
using ConsultantService.Infrastructure.Clients;
using ConsultantService.Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence.Mongo;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {

            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(config.GetConnectionString("Mongo")));


            services.AddScoped<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>()
                  .GetDatabase(config["Mongo:DatabaseName"]));


            services.AddScoped<IConsultantRepository, MongoConsultantRepository>();
            services.AddScoped<IUnitOfWork, MongoUnitOfWork>();

            services.AddHttpClient<ICaseServiceClient, CaseServiceClient>(c =>
            {
                c.BaseAddress = new Uri(config["CaseService:BaseUrl"]);
            });

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<CaseSubmittedConsumer>();
                busConfigurator.AddConsumer<UserRegisteredConsumer>();
                busConfigurator.AddConsumer<CreateConsultantProfileConsumer>();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(config["MessageBroker:Host"]!), h =>
                    {
                        h.Username(config["MessageBroker:Username"]);
                        h.Password(config["MessageBroker:Password"]);
                    });

                    cfg.ConfigureEndpoints(context);          
                });
            });

            return services;
        }

    }
}
