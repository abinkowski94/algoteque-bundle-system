using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Monads;

namespace AlgotequeCourseBundler.Application.Bundlers.Services;

public interface IBundlerService
{
    ValueTask<Result<ProviderBundle>> CreateBundleAsync(IReadOnlyDictionary<string, decimal> requestedTopics, CancellationToken cancellationToken);
}