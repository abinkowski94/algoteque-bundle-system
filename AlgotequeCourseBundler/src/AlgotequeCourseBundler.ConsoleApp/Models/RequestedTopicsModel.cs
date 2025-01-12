using System.Text.Json.Serialization;

namespace AlgotequeCourseBundler.ConsoleApp.Models;

internal sealed class RequestedTopicsModel
{
    [JsonPropertyName("topics")]
    public required Dictionary<string, decimal> Topics { get; init; }
}
