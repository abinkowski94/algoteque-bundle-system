using System.Text.Json.Serialization;

namespace AlgotequeCourseBundler.DataAccess.Providers.Models;

internal sealed class ProviderTopicsModel
{
    [JsonPropertyName("provider_topics")]
    public required Dictionary<string, string> ProviderTopics { get; init; }
}
