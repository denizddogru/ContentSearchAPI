using ContentSearchAPI.Domain.Common;

namespace ContentSearchAPI.Domain.Entities;

public class ProviderRequestLog : BaseEntity
{
    public string ProviderId { get; set; } = string.Empty;
    public DateTime RequestTimestamp { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}
