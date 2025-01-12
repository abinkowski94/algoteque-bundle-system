using System.Text.Json;
using AlgotequeCourseBundler.Application.Bundlers.Services;
using AlgotequeCourseBundler.ConsoleApp.Models;
using Microsoft.Extensions.Logging;

namespace AlgotequeCourseBundler.ConsoleApp.Handlers;

internal sealed class BundleTopicsHandler(IBundlerService bundlerService, ILogger logger)
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };
    private readonly IBundlerService _providerService = bundlerService;
    private readonly ILogger _logger = logger;

    public async ValueTask HandleAsync(string topicsFilePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(topicsFilePath))
        {
            _logger.LogError("File {FilePaht} not found.", topicsFilePath);
            return;
        }

        var requestedTopicsModel = await GetRequestedTopicsAsync(topicsFilePath, cancellationToken).ConfigureAwait(false);
        if (requestedTopicsModel is null)
        {
            _logger.LogError("File {FilePaht} contains no topics.", topicsFilePath);
            return;
        }

        var bundlingResult = await _providerService.CreateBundleAsync(requestedTopicsModel.Topics, cancellationToken).ConfigureAwait(false);
        if (bundlingResult.HasError)
        {
            _logger.LogError("Bundling resulted in error: {ErrorMessage}", bundlingResult.Error.Message);
            return;
        }

        var bundleDictionary = bundlingResult.Value.ToDictionary(kv => kv.Key.Name, kv => kv.Value);
        var bundleJson = JsonSerializer.Serialize(bundleDictionary, _serializerOptions);

        _logger.LogInformation("Bundling result: {NewLine}{Result}", Environment.NewLine, bundleJson);
    }

    private static async Task<RequestedTopicsModel?> GetRequestedTopicsAsync(string topicsFilePath, CancellationToken cancellationToken)
    {
        using var fileStream = new FileStream(topicsFilePath, FileMode.Open, FileAccess.Read);

        var requestedTopicsModel = await JsonSerializer
            .DeserializeAsync<RequestedTopicsModel>(fileStream, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return requestedTopicsModel;
    }
}
