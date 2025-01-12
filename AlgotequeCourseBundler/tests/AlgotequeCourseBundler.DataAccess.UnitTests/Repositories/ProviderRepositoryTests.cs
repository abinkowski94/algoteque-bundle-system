using System.Text.Json;
using AlgotequeCourseBundler.DataAccess.Providers.Exceptions;
using AlgotequeCourseBundler.DataAccess.Providers.Repositories;
using FluentAssertions.Execution;

namespace AlgotequeCourseBundler.DataAccess.UnitTests.Repositories;

public class ProviderRepositoryTests : IDisposable
{
    private readonly string _validJsonPath = "validProviders.json";
    private readonly string _invalidJsonPath = "invalidProviders.json";

    [Fact]
    public async Task GetAllAsync_ShouldReturnProviders_WhenJsonFileIsValid()
    {
        // Arrange
        var mockJsonData = """
        {
            "provider_topics": {
                "provider_a": "math+science",
                "provider_b": "reading+science",
                "provider_c": "history+math"
            }
        }        
        """;
        await File.WriteAllTextAsync(_validJsonPath, mockJsonData);
        var repository = new ProviderRepository(_validJsonPath);

        // Act
        var result = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(3);

            result.Value[0].Name.Should().Be("provider_a");
            result.Value[1].Name.Should().Be("provider_b");
            result.Value[2].Name.Should().Be("provider_c");

            result.Value[1].SupportedTopics.Should().Contain("reading");
            result.Value[1].SupportedTopics.Should().Contain("science");
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnError_WhenJsonFileIsNotFound()
    {
        // Arrange
        var repository = new ProviderRepository("nonexistent.json");

        // Act
        var result = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.HasError.Should().BeTrue();
            result.Error.Should().BeOfType<RepositoryException>();
            ((RepositoryException)result.Error).Message.Should().Contain("JSON file not found");
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnError_WhenJsonFileIsNull()
    {
        // Arrange
        await File.WriteAllTextAsync(_invalidJsonPath, "null");
        var repository = new ProviderRepository(_invalidJsonPath);

        // Act
        var result = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.HasError.Should().BeTrue();
            result.Error.Should().BeOfType<RepositoryException>();
            ((RepositoryException)result.Error).Message.Should().Contain("JSON file is null");
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnError_WhenJsonFileHasInvalidStructure()
    {
        // Arrange
        var mockInvalidJsonData = "Invalid JSON";
        await File.WriteAllTextAsync(_invalidJsonPath, mockInvalidJsonData);
        var repository = new ProviderRepository(_invalidJsonPath);

        // Act
        Func<Task> act = async () => await repository.GetAllAsync(CancellationToken.None).ConfigureAwait(false);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }

    public void Dispose()
    {
        if (File.Exists(_invalidJsonPath))
        {
            File.Delete(_invalidJsonPath);
        }

        if (File.Exists(_validJsonPath))
        {
            File.Delete(_validJsonPath);
        }

        GC.SuppressFinalize(this);
    }
}
