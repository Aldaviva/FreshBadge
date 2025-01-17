namespace FreshBadge.Data;

/// <summary>
/// <para>JSON returned from Freshping REST API</para>
/// <para>Documentation: <see href="https://support.freshping.io/support/solutions/articles/50000003709-freshping-api-documentation"/></para>
/// </summary>
public class Check {

    public long id { get; init; }                            // ": 1,
    public required string name { get; init; }               // ": "Example",
    public required string status { get; init; }             // ": "AV",
    public string? performanceStatus { get; init; }          // ": null,
    public string? alertNote { get; init; }                  // ": "",
    public required string location { get; init; }           // ": "us-east-1",
    public IReadOnlyList<long>? alertUsers { get; init; }    // ": [1],
    public IReadOnlyList<long>? alertContacts { get; init; } // ": [],
    public int monitoringInterval { get; init; }             // ": 60,
    public required Uri url { get; init; }                   // ": "https://example.com",
    public int requestTimeout { get; init; }                 // ": 30,
    public string? basicAuthUsername { get; init; }          // ": "",
    public string? basicAuthPassword { get; init; }          // ": "",
    public object? customHeader { get; init; }               // ": {},
    public string? commandString { get; init; }              // ": null,
    public string? successString { get; init; }              // ": null,
    public string? errorString { get; init; }                // ": null

}