using System.Text.Json;
using AlgotequeCourseBundler.Application.Providers.Repositories;
using AlgotequeCourseBundler.DataAccess.Providers.Exceptions;
using AlgotequeCourseBundler.DataAccess.Providers.Mappers;
using AlgotequeCourseBundler.DataAccess.Providers.Models;
using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Monads;

namespace AlgotequeCourseBundler.DataAccess.Providers.Repositories;

public class ProviderRepository(string providersJsonFilePath) : IProviderRepository
{
    private readonly string _providersJsonFilePath = providersJsonFilePath;

    public async ValueTask<Result<List<Provider>>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_providersJsonFilePath))
        {
            return new RepositoryException($"JSON file not found. Path: {_providersJsonFilePath}");
        }

        using var fileStream = new FileStream(_providersJsonFilePath, FileMode.Open, FileAccess.Read);

        var result = await JsonSerializer
            .DeserializeAsync<ProviderTopicsModel>(fileStream, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (result is null)
        {
            return new RepositoryException($"JSON file is null. Path: {_providersJsonFilePath}");
        }

        return result.MapToProviders();
    }
}
