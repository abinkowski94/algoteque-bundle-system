using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AlgotequeCourseBundler.Domain.Entities;

public class ProviderBundle : IReadOnlyDictionary<Provider, decimal>
{
    private readonly Dictionary<Provider, decimal> _items;

    public decimal this[Provider key] => _items[key];

    public IEnumerable<Provider> Keys => _items.Keys;

    public IEnumerable<decimal> Values => _items.Values;

    public int Count => _items.Count;

    private ProviderBundle()
    {
        _items = [];
    }

    public static ProviderBundle Create()
    {
        return new ProviderBundle();
    }

    public void AddToBundle(Provider provider, decimal quote)
    {
        _items.Add(provider, quote);
    }

    public bool ContainsKey(Provider key)
    {
        return _items.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<Provider, decimal>> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public bool TryGetValue(Provider key, [MaybeNullWhen(false)] out decimal value)
    {
        return _items.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
