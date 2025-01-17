using System.Text.Json.Serialization;

namespace FreshBadge.Data;

/// <summary>
/// JSON returned from the Status Page secret API
/// </summary>
public class PublicCheckStatusReport {

    public uint checkId { get; init; }
    public uint durationSecondsAvailable { get; init; }
    public uint durationSecondsTotalDowntime { get; init; }
    public uint outagesCountTotal { get; init; }

    [JsonIgnore]
    public double uptimePercentage => (double) durationSecondsAvailable / (durationSecondsAvailable + durationSecondsTotalDowntime);

}