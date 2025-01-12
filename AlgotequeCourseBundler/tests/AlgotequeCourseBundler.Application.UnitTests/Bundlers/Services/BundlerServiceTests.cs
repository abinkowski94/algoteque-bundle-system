using AlgotequeCourseBundler.Application.Bundlers.Services;
using AlgotequeCourseBundler.Application.Providers.Repositories;
using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Monads;
using AlgotequeCourseBundler.Domain.Services;
using FluentAssertions.Execution;
using Moq;

namespace AlgotequeCourseBundler.Application.UnitTests.Bundlers.Services;

public class BundlerServiceTests
{
    private readonly Mock<ITopicsBundlerService> _mockBundlerService;
    private readonly Mock<IProviderRepository> _mockProviderRepository;
    private readonly BundlerService _service;

    public BundlerServiceTests()
    {
        _mockBundlerService = new();
        _mockProviderRepository = new();

        _service = new(
            _mockBundlerService.Object,
            _mockProviderRepository.Object);
    }

    [Fact]
    public async Task CreateBundleAsync_ShouldReturnError_WhenRepositoryReturnsError()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var repositoryError = new InvalidOperationException("Repository error");

        _mockProviderRepository
            .Setup(repo => repo.GetAllAsync(cancellationToken))
            .ReturnsAsync(repositoryError);

        var requestedTopics = new Dictionary<string, decimal>();

        // Act
        var result = await _service.CreateBundleAsync(requestedTopics, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.HasError.Should().BeTrue();
            result.Error.Should().Be(repositoryError);
        }
    }

    [Fact]
    public async Task CreateBundleAsync_ShouldReturnBundle_WhenRepositoryReturnsProviders()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var providers = new List<Provider>
        {
            Provider.Create("Provider1", [ "Topic1", "Topic2" ]),
            Provider.Create("Provider2", [ "Topic2", "Topic3" ])
        };

        _mockProviderRepository
            .Setup(repo => repo.GetAllAsync(cancellationToken))
            .ReturnsAsync(Result<List<Provider>>.FromT(providers));

        var requestedTopics = new Dictionary<string, decimal>
        {
            { "Topic1", 0.5m },
            { "Topic2", 0.5m }
        };

        var expectedBundle = ProviderBundle.Create();
        expectedBundle.AddToBundle(providers[0], 0.5m);
        expectedBundle.AddToBundle(providers[1], 0.5m);

        _mockBundlerService
            .Setup(service => service.CreateBundle(providers, requestedTopics))
            .Returns(expectedBundle);

        // Act
        var result = await _service.CreateBundleAsync(requestedTopics, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.HasError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedBundle);
        }

        _mockBundlerService.Verify(
            service => service.CreateBundle(providers, requestedTopics),
            Times.Once);
    }

    [Fact]
    public async Task CreateBundleAsync_ShouldCallDependenciesCorrectly()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var providers = new List<Provider>
        {
            Provider.Create("Provider1", [ "Topic1" ]),
            Provider.Create("Provider2", [ "Topic2" ])
        };

        _mockProviderRepository
            .Setup(repo => repo.GetAllAsync(cancellationToken))
            .ReturnsAsync(Result<List<Provider>>.FromT(providers));

        var requestedTopics = new Dictionary<string, decimal>
        {
            { "Topic1", 1.0m }
        };

        var bundle = ProviderBundle.Create();

        _mockBundlerService
            .Setup(service => service.CreateBundle(providers, requestedTopics))
            .Returns(bundle);

        // Act
        await _service.CreateBundleAsync(requestedTopics, cancellationToken);

        // Assert
        _mockProviderRepository.Verify(repo => repo.GetAllAsync(cancellationToken), Times.Once);
        _mockBundlerService.Verify(service => service.CreateBundle(providers, requestedTopics), Times.Once);
    }

    [Fact]
    public async Task CreateBundleAsync_ShouldReturnEmptyBundle_WhenNoMatchingTopicsFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var providers = new List<Provider>
        {
            Provider.Create("Provider1", [ "Topic3" ]),
            Provider.Create("Provider2", [ "Topic4" ])
        };

        _mockProviderRepository
            .Setup(repo => repo.GetAllAsync(cancellationToken))
            .ReturnsAsync(providers);

        var requestedTopics = new Dictionary<string, decimal>
        {
            { "Topic1", 0.5m },
            { "Topic2", 0.5m }
        };

        var emptyBundle = ProviderBundle.Create();
        _mockBundlerService
            .Setup(service => service.CreateBundle(providers, requestedTopics))
            .Returns(emptyBundle);

        // Act
        var result = await _service.CreateBundleAsync(requestedTopics, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.HasError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(emptyBundle);
        }
    }
}
