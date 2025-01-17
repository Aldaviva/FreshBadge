using FreshBadge;
using FreshBadge.Data.Shields;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

ShieldLogo freshpingLogo = new(Resources.freshpingLogo);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<FreshpingClient, FreshpingClientImpl>()
    .ConfigureHttpJsonOptions(options => {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
        options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddSingleton(_ => new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromHours(1), MaxConnectionsPerServer = 16 }) { Timeout = TimeSpan.FromSeconds(30) });

await using WebApplication webApp = builder.Build();

webApp.MapGet("/{checkId:long}", async ([FromRoute] long checkId, [FromQuery] string? period, [FromServices] FreshpingClient client) => {
    try {
        TimeSpan    reportPeriod = TimeSpan.TryParse(period, out TimeSpan p) ? p : TimeSpan.FromDays(90);
        CheckStatus status       = await client.fetchCheckStatus(checkId, reportPeriod);
        return new ShieldsBadgeResponse("uptime", status.uptime.ToString("P7"), messageColor: status.isUp ? ShieldColor.SUCCESS : ShieldColor.CRITICAL, isError: !status.isUp, logo: freshpingLogo);
    } catch (FreshBadgeException e) {
        return new ShieldsBadgeResponse("error", e.Message, messageColor: ShieldColor.CRITICAL, isError: true, logo: freshpingLogo);
    }
});

await webApp.RunAsync();