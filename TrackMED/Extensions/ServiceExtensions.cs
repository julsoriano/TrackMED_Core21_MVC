using Microsoft.Extensions.DependencyInjection;

using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add application services.
            services.AddSingleton<IEntityService<ActivityType>, EntityService<ActivityType>>();
            services.AddSingleton<IEntityService<Category>, EntityService<Category>>();
            services.AddSingleton<IEntityService<Classification>, EntityService<Classification>>();
            services.AddSingleton<IEntityService<Component>, EntityService<Component>>();
            services.AddSingleton<IEntityService<Deployment>, EntityService<Deployment>>();
            services.AddSingleton<IEntityService<Description>, EntityService<Description>>();
            services.AddSingleton<IEntityService<EquipmentActivity>, EntityService<EquipmentActivity>>();
            services.AddSingleton<IEntityService<Event>, EntityService<Event>>();
            services.AddSingleton<IEntityService<Location>, EntityService<Location>>();
            services.AddSingleton<IEntityService<Manufacturer>, EntityService<Manufacturer>>();
            services.AddSingleton<IEntityService<Model>, EntityService<Model>>();
            services.AddSingleton<IEntityService<Model_Manufacturer>, EntityService<Model_Manufacturer>>();
            services.AddSingleton<IEntityService<Owner>, EntityService<Owner>>();
            services.AddSingleton<IEntityService<ProviderOfService>, EntityService<ProviderOfService>>();
            services.AddSingleton<IEntityService<Status>, EntityService<Status>>();
            services.AddSingleton<IEntityService<SystemsDescription>, EntityService<SystemsDescription>>();
            services.AddSingleton<IEntityService<SystemTab>, EntityService<SystemTab>>();

            // Add all other services here.
            return services;
        }
    }
}
