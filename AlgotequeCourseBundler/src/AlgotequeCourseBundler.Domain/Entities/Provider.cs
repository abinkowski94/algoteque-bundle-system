
namespace AlgotequeCourseBundler.Domain.Entities;

public class Provider
{
    private Provider(string name, IReadOnlyCollection<string> supportedTopics)
    {
        Name = name;
        _supportedTopics = supportedTopics.ToList();
    }

    private readonly List<string> _supportedTopics;

    public string Name { get; private set; }

    public IReadOnlyList<string> SupportedTopics => _supportedTopics.AsReadOnly();

    public static Provider Create(string name, IReadOnlyCollection<string> supportedTopics)
    {
        return new(name, supportedTopics);
    }

    public IReadOnlyList<string> MatchTopics(IEnumerable<string> topics)
    {
        return _supportedTopics.Intersect(topics).ToList();
    }
}
