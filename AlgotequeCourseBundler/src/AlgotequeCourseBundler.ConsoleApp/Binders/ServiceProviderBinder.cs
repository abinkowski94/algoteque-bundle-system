using System.CommandLine.Binding;
using AlgotequeCourseBundler.Application.Bundlers.Services;
using AlgotequeCourseBundler.Application.Providers.Repositories;
using AlgotequeCourseBundler.ConsoleApp.Handlers;
using AlgotequeCourseBundler.DataAccess.Providers.Repositories;
using AlgotequeCourseBundler.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlgotequeCourseBundler.ConsoleApp.Binders;

internal sealed class ServiceProviderBinder : BinderBase<IServiceProvider>
{
    protected override IServiceProvider GetBoundValue(BindingContext bindingContext)
    {
        var services = new ServiceCollection();
        services.AddLogging(cfg => cfg.ClearProviders().AddConsole());
        services.AddTransient(sp =>
        {
            using var factory = sp.GetRequiredService<ILoggerFactory>();
            ILogger logger = factory.CreateLogger("Bundler");

            return logger;
        });

        services.AddSingleton<ITopicsBundlerService, TopicsBundlerService>();
        services.AddSingleton<IProviderRepository>(new ProviderRepository(Path.Combine(".", "Providers", "Data", "staticProviders.json")));
        services.AddSingleton<IBundlerService, BundlerService>();
        services.AddSingleton<BundleTopicsHandler>();

        return services.BuildServiceProvider();
    }
}
