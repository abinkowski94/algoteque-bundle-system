using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Monads;

namespace AlgotequeCourseBundler.Domain.Services;

public interface ITopicsBundlerService
{
    Result<ProviderBundle> CreateBundle(IReadOnlyCollection<Provider> providers, IReadOnlyDictionary<string, decimal> requestedTopics);
}