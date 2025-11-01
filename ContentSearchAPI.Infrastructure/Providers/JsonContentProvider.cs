using ContentSearchAPI.Domain.Entities;
using ContentSearchAPI.Domain.Enums;
using System.Text.Json;

namespace ContentSearchAPI.Infrastructure.Providers;

public class JsonContentProvider : BaseContentProvider
{
    public JsonContentProvider(HttpClient httpClient, ProviderConfig config) : base(httpClient, config)
    {
    }

    public override async Task<IEnumerable<Content>> FetchContentAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync(_config.Endpoint, cancellationToken);
        return await ParseResponseAsync(response, cancellationToken);
    }

    protected override Task<IEnumerable<Content>> ParseResponseAsync(string response, CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var items = JsonSerializer.Deserialize<List<JsonContentItem>>(response, options);
        if (items == null)
            return Task.FromResult(Enumerable.Empty<Content>());

        var result = items.Select(item => new Content
        {
            Id = Guid.NewGuid(),
            Title = item.Title ?? string.Empty,
            Description = item.Description ?? string.Empty,
            Type = Enum.Parse<ContentType>(item.Type ?? "Text", true),
            ProviderId = ProviderId,
            SourceUrl = item.Url ?? string.Empty,
            Views = item.Views,
            Likes = item.Likes,
            ReadingTime = item.ReadingTime,
            Reactions = item.Reactions,
            CreatedDate = item.CreatedDate ?? DateTime.UtcNow
        });

        return Task.FromResult<IEnumerable<Content>>(result);
    }

    private class JsonContentItem
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Url { get; set; }
        public int? Views { get; set; }
        public int? Likes { get; set; }
        public int? ReadingTime { get; set; }
        public int? Reactions { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
