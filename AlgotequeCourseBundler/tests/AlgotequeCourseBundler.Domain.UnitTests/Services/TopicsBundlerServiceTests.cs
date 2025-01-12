using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Exceptions;
using AlgotequeCourseBundler.Domain.Services;
using FluentAssertions.Execution;

namespace AlgotequeCourseBundler.Domain.UnitTests.Services;

public class TopicsBundlerServiceTests
{
    private readonly TopicsBundlerService _service;

    public TopicsBundlerServiceTests()
    {
        _service = new TopicsBundlerService();
    }

    [Fact]
    public void CreateBundle_ShouldThrowArgumentNullException_WhenProvidersAreNull()
    {
        // Act
        var act = () => _service.CreateBundle(null!, new Dictionary<string, decimal>());

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateBundle_ShouldThrowArgumentNullException_WhenRequestedTopicsAreNull()
    {
        // Act
        var act = () => _service.CreateBundle(new List<Provider>(), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateBundle_ShouldReturnDomainException_WhenProvidersAreEmpty()
    {
        // Arrange
        var providers = new List<Provider>();
        var requestedTopics = new Dictionary<string, decimal> { { "Topic1", 1.0m } };

        // Act
        var result = _service.CreateBundle(providers, requestedTopics);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeFalse();
            result.Error.Should().BeOfType<DomainException>();
            result.Error.Message.Should().Be("No providers provided.");
        }
    }

    [Fact]
    public void CreateBundle_ShouldReturnDomainException_WhenRequestedTopicsAreEmpty()
    {
        // Arrange
        var providers = new List<Provider> { Provider.Create("Provider1", ["Topic1"]) };
        var requestedTopics = new Dictionary<string, decimal>();

        // Act
        var result = _service.CreateBundle(providers, requestedTopics);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeFalse();
            result.Error.Should().BeOfType<DomainException>();
            result.Error.Message.Should().Be("No topics provided.");
        }
    }

    [Fact]
    public void CreateBundle_ShouldReturnDomainException_WhenNoTopicsMatch()
    {
        // Arrange
        var providers = new List<Provider> { Provider.Create("Provider1", ["Topic2"]) };
        var requestedTopics = new Dictionary<string, decimal> { { "Topic1", 1.0m } };

        // Act
        var result = _service.CreateBundle(providers, requestedTopics);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeFalse();
            result.Error.Should().BeOfType<DomainException>();
            result.Error.Message.Should().Be("No topics matched.");
        }
    }

    [Fact]
    public void CreateBundle_ShouldReturnValidResult_WhenProvidersMatchTopics()
    {
        // Arrange
        var providers = new List<Provider> { Provider.Create("Provider1", ["Topic1", "Topic2"]) };
        var requestedTopics = new Dictionary<string, decimal>
        {
            { "Topic1", 1.0m },
            { "Topic2", 0.5m },
            { "Topic3", 0.2m }
        };

        // Act
        var result = _service.CreateBundle(providers, requestedTopics);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Count.Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public void CreateBundle_ShouldTrimTopicsToTopThreeByValue()
    {
        // Arrange
        var providers = new List<Provider>
        {
            Provider.Create("Provider1", ["Topic1"]),
            Provider.Create("Provider2", ["Topic2"]),
            Provider.Create("Provider3", ["Topic3"]),
            Provider.Create("Provider4", ["Topic4"]),
        };
        var requestedTopics = new Dictionary<string, decimal>
        {
            { "Topic1", 0.1m },
            { "Topic2", 0.2m },
            { "Topic3", 0.3m },
            { "Topic4", 0.4m }
        };

        // Act
        var result = _service.CreateBundle(providers, requestedTopics);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeTrue();
            result.Value.Count.Should().Be(3);
            result.Value.Keys.Should().Contain(p => p.SupportedTopics.Contains("Topic4"));
            result.Value.Keys.Should().Contain(p => p.SupportedTopics.Contains("Topic3"));
            result.Value.Keys.Should().Contain(p => p.SupportedTopics.Contains("Topic2"));
        }
    }

    [Fact]
    public void Provider_ShouldMatchTopicsCorrectly()
    {
        // Arrange
        var provider = Provider.Create("Provider1", new List<string> { "Topic1", "Topic2" });
        var topics = new List<string> { "Topic1", "Topic3" };

        // Act
        var matchedTopics = provider.MatchTopics(topics);

        // Assert
        matchedTopics.Should().ContainSingle().Which.Should().Be("Topic1");
    }

    [Fact]
    public void ProviderBundle_ShouldAddProviderWithQuote()
    {
        // Arrange
        var provider = Provider.Create("Provider1", new List<string> { "Topic1" });
        var bundle = ProviderBundle.Create();

        // Act
        bundle.AddToBundle(provider, 100m);

        // Assert
        using (new AssertionScope())
        {
            bundle.ContainsKey(provider).Should().BeTrue();
            bundle[provider].Should().Be(100m);
        }
    }

    [Fact]
    public void ProviderBundle_ShouldEnumerateCorrectly()
    {
        // Arrange
        var provider = Provider.Create("Provider1", new List<string> { "Topic1" });
        var bundle = ProviderBundle.Create();
        bundle.AddToBundle(provider, 50m);

        // Act
        var enumeratedItems = bundle.ToList();

        // Assert
        using (new AssertionScope())
        {
            enumeratedItems.Should().ContainSingle();
            enumeratedItems[0].Key.Should().Be(provider);
            enumeratedItems[0].Value.Should().Be(50m);
        }
    }
}
