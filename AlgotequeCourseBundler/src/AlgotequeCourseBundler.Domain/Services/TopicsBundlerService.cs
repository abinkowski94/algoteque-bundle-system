using AlgotequeCourseBundler.Domain.Entities;
using AlgotequeCourseBundler.Domain.Exceptions;
using AlgotequeCourseBundler.Domain.Monads;

namespace AlgotequeCourseBundler.Domain.Services;

public class TopicsBundlerService : ITopicsBundlerService
{
    public Result<ProviderBundle> CreateBundle(IReadOnlyCollection<Provider> providers, IReadOnlyDictionary<string, decimal> requestedTopics)
    {
        ArgumentNullException.ThrowIfNull(providers);
        ArgumentNullException.ThrowIfNull(requestedTopics);

        if (providers.Count == 0)
        {
            return new DomainException("No providers provided.");
        }

        if (requestedTopics.Count == 0)
        {
            return new DomainException("No topics provided.");
        }

        var result = CreateBundler(providers, requestedTopics);
        if (result.Count == 0)
        {
            return new DomainException("No topics matched.");
        }

        return result;
    }

    private static ProviderBundle CreateBundler(IReadOnlyCollection<Provider> providers, IReadOnlyDictionary<string, decimal> requestedTopics)
    {
        var topicsToCalculate = TrimTopics(requestedTopics);
        var orderedTopics = OrderTopicsByResourceAllocation(topicsToCalculate);

        var result = ProviderBundle.Create();

        foreach (var provider in providers)
        {
            AddProvider(result, provider, topicsToCalculate, orderedTopics);
        }

        return result;
    }

    private static Dictionary<string, decimal> TrimTopics(IReadOnlyDictionary<string, decimal> requestedTopics)
    {
        return requestedTopics
            .OrderByDescending(kv => kv.Value)
            .Take(3)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private static List<string> OrderTopicsByResourceAllocation(Dictionary<string, decimal> topicsToCalculate)
    {
        return topicsToCalculate
            .OrderByDescending(kv => kv.Value)
            .Select(t => t.Key)
            .ToList();
    }

    private static void AddProvider(ProviderBundle bundle, Provider provider, Dictionary<string, decimal> topics, List<string> orderedTopics)
    {
        var matchedTopics = provider.MatchTopics(topics.Keys);

        if (matchedTopics.Count == 2)
        {
            AddProviderWithTwoMatches(bundle, provider, topics, matchedTopics);
        }
        else if (matchedTopics.Count == 1)
        {
            AddProviderWithOneMatch(bundle, provider, topics, orderedTopics, matchedTopics[0]);
        }
    }

    private static void AddProviderWithTwoMatches(
        ProviderBundle bundle,
        Provider provider,
        Dictionary<string, decimal> topics,
        IReadOnlyList<string> providerMatchedTopics)
    {
        var quote = providerMatchedTopics.Sum(t => topics[t]) * 0.1m;

        if (quote > 0)
        {
            bundle.AddToBundle(provider, quote);
        }
    }

    private static void AddProviderWithOneMatch(
        ProviderBundle bundle,
        Provider provider,
        Dictionary<string, decimal> topics,
        List<string> orderedTopics,
        string matchedTopic)
    {
        var matchedTopicAllotment = topics[matchedTopic];
        var topicIndex = orderedTopics.IndexOf(matchedTopic);

        var quote = topicIndex switch
        {
            0 => matchedTopicAllotment * 0.2m,
            1 => matchedTopicAllotment * 0.25m,
            2 => matchedTopicAllotment * 0.3m,
            _ => 0,
        };

        if (quote > 0)
        {
            bundle.AddToBundle(provider, quote);
        }
    }
}
