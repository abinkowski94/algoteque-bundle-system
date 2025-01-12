using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Monads;

namespace AlgotequeCourseBundler.Application.Providers.Repositories;

public interface IProviderRepository
{
    ValueTask<Result<List<Provider>>> GetAllAsync(CancellationToken cancellationToken);
}
