using AlgotequeCourseBundler.Application.Providers.Repositories;
using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Monads;
using AlgotequeCourseBundler.Domain.Services;

namespace AlgotequeCourseBundler.Application.Bundlers.Services;

public class BundlerService(ITopicsBundlerService bundlerService, IProviderRepository repository) : IBundlerService
{
    private readonly ITopicsBundlerService _bundlerService = bundlerService;
    private readonly IProviderRepository _repository = repository;

    public async ValueTask<Result<ProviderBundle>> CreateBundleAsync(IReadOnlyDictionary<string, decimal> requestedTopics, CancellationToken cancellationToken)
    {
        var providersResult = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        if (providersResult.HasError)
        {
            return providersResult.Error;
        }

        return _bundlerService.CreateBundle(providersResult.Value, requestedTopics);
    }
}
