using AlgotequeCourseBundler.DataAccess.Providers.Models;
using AlgotequeCourseBundler.Domain.Entities;

namespace AlgotequeCourseBundler.DataAccess.Providers.Mappers;

internal static class ProviderMapper
{
    internal static List<Provider> MapToProviders(this ProviderTopicsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return model.ProviderTopics
            .Select(kv => MapToProvider(kv.Key, kv.Value))
            .ToList();
    }

    private static Provider MapToProvider(string name, string topics)
    {
        var splitTopics = topics.Split("+", StringSplitOptions.RemoveEmptyEntries);

        return Provider.Create(name, splitTopics);
    }
}
