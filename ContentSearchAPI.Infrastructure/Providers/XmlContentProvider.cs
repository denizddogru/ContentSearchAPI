using ContentSearchAPI.Domain.Entities;
using ContentSearchAPI.Domain.Enums;
using System.Xml.Linq;

namespace ContentSearchAPI.Infrastructure.Providers;

public class XmlContentProvider : BaseContentProvider
{
    public XmlContentProvider(HttpClient httpClient, ProviderConfig config) : base(httpClient, config)
    {
    }

    public override async Task<IEnumerable<Content>> FetchContentAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync(_config.Endpoint, cancellationToken);
        return await ParseResponseAsync(response, cancellationToken);
    }

    protected override Task<IEnumerable<Content>> ParseResponseAsync(string response, CancellationToken cancellationToken)
    {
        var doc = XDocument.Parse(response);
        var items = doc.Descendants("item").Select(item => new Content
        {
            Id = Guid.NewGuid(),
            Title = item.Element("title")?.Value ?? string.Empty,
            Description = item.Element("description")?.Value ?? string.Empty,
            Type = Enum.Parse<ContentType>(item.Element("type")?.Value ?? "Text", true),
            ProviderId = ProviderId,
            SourceUrl = item.Element("url")?.Value ?? string.Empty,
            Views = int.TryParse(item.Element("views")?.Value, out var views) ? views : null,
            Likes = int.TryParse(item.Element("likes")?.Value, out var likes) ? likes : null,
            ReadingTime = int.TryParse(item.Element("readingTime")?.Value, out var readingTime) ? readingTime : null,
            Reactions = int.TryParse(item.Element("reactions")?.Value, out var reactions) ? reactions : null,
            CreatedDate = DateTime.TryParse(item.Element("createdDate")?.Value, out var date) ? date : DateTime.UtcNow
        });

        return Task.FromResult(items);
    }
}
