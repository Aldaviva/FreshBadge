using System.Text.Json.Serialization;

namespace FreshBadge.Data;

/// <summary>
/// JSON returned from the Status Page secret API
/// </summary>
public class PublicCheckStatusReport {

    public uint checkId { get; init; }
    public uint durationSecondsTotalDowntime { get; init; }
    public uint outagesCountTotal { get; init; }
    public uint durationSecondsPerformanceGood { get; init; }
    public uint durationSecondsPerformanceDegraded { get; init; }

    [JsonIgnore]
    public double uptimePercentage => 1 - (double) durationSecondsTotalDowntime / (durationSecondsPerformanceGood + durationSecondsPerformanceDegraded);

}