using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Interface.Impl;
using Inventory.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Uwp
{
    public static class CompositionRoot
    {
        /// <summary>
        /// Registra TUTTI i moduli in ordine:
        /// Application → Infrastructure → Persistence → Presentation → Bootstrap
        /// </summary>
        public static void AddAllService(IServiceCollection serviceCollection)
        {
            // 1. Presentation layer (viewmodel, navigation, provisioning DB UWP…)
            PresentationRegister.Register(serviceCollection);

            // 2. Interface layer (facade, servizi di dominio)
            InterfaceRegister.Register(serviceCollection);

            // 3. Application layer (use case, servizi di dominio)
            ApplicationRegister.Register(serviceCollection);

            // 4. Infrastructure layer (contratti generici, Settings, ConfigService…)
            InfrastructureRegister.Register(serviceCollection);

            // 5. Persistence layer (EF Core, repository concreti…)
            InfrastructurePersistenceRegister.Register(serviceCollection);

            // 5. Composizione finale: bootstrap dell’intera app
            serviceCollection.AddSingleton<AppBootstrapper>();
        }
    }

}
