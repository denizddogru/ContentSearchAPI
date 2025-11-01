using ContentSearchAPI.Domain.Common;
using ContentSearchAPI.Domain.Enums;

namespace ContentSearchAPI.Domain.Entities;

public class ProviderConfig : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public DataFormat Format { get; set; }
    public int RequestLimitPerHour { get; set; }
    public bool IsActive { get; set; }
}
