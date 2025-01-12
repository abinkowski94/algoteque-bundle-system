using AlgotequeCourseBundler.Application.Bundlers.Services;
using AlgotequeCourseBundler.ConsoleApp.Handlers;
using AlgotequeCourseBundler.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace AlgotequeCourseBundler.ConsoleApp.UnitTests.Handlers;

public class BundleTopicsHandlerTests : IDisposable
{
    private readonly Mock<IBundlerService> _bundlerServiceMock;
    private readonly BundleTopicsHandler _handler;

    private const string InvalidFilePath = "invalidPath.json";
    private const string EmptyFilePath = "emptyFile.json";
    private const string FilePath = "topics.json";

    public BundleTopicsHandlerTests()
    {
        _bundlerServiceMock = new();

        _handler = new(
            _bundlerServiceMock.Object,
            Mock.Of<ILogger>());
    }

    [Fact]
    public async Task HandleAsync_FileNotFound_DoesNotThrow()
    {
        // Act
        var action = async () => await _handler.HandleAsync(InvalidFilePath).ConfigureAwait(false);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HandleAsync_FileExistsButEmptyTopics_DoesNotThrow()
    {
        // Arrange
        var emptyContent = """
        {
            "topics": {}
        }
        """;

        await File.WriteAllTextAsync(EmptyFilePath, emptyContent);

        // Act
        var action = async () => await _handler.HandleAsync(EmptyFilePath).ConfigureAwait(false);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HandleAsync_BundlingFails_DoesNotThrow()
    {
        // Arrange        
        var topicsContent = """{ "topics": { "topic1": 1.0 } }""";

        await File.WriteAllTextAsync(FilePath, topicsContent);

        _bundlerServiceMock
            .Setup(service => service.CreateBundleAsync(It.IsAny<IReadOnlyDictionary<string, decimal>>(), default))
            .ReturnsAsync(new InvalidOperationException("Mock error"));

        // Act
        var action = async () => await _handler.HandleAsync(FilePath).ConfigureAwait(false);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HandleAsync_BundlingSucceeds_DoesNotThrow()
    {
        // Arrange
        var topicsContent = """{ "topics": { "topic1": 1.0 } }""";

        await File.WriteAllTextAsync(FilePath, topicsContent);

        var provider = Provider.Create("Provider1", ["topic1"]);
        var bundle = ProviderBundle.Create();
        bundle.AddToBundle(provider, 10.0m);

        _bundlerServiceMock
            .Setup(service => service.CreateBundleAsync(It.IsAny<IReadOnlyDictionary<string, decimal>>(), default))
            .ReturnsAsync(bundle);

        // Act
        var action = async () => await _handler.HandleAsync(FilePath).ConfigureAwait(false);

        // Assert
        await action.Should().NotThrowAsync();
    }

    public void Dispose()
    {
        if (File.Exists(InvalidFilePath))
        {
            File.Delete(InvalidFilePath);
        }

        if (File.Exists(EmptyFilePath))
        {
            File.Delete(EmptyFilePath);
        }

        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        GC.SuppressFinalize(this);
    }
}
