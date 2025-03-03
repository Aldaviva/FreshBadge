using FreshBadge;
using FreshBadge.Data.Shields;
using FreshBadge.Internationalization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.Text;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unfucked.HTTP;
using Unfucked.HTTP.Config;

ShieldLogo  freshpingLogo   = new(Resources.freshpingLogo);
CultureInfo defaultCulture  = CultureInfo.CurrentCulture;
Duration    minimumDuration = Duration.FromMinutes(1);
Duration    maximumDuration = Duration.FromDays(90);
Duration    defaultDuration = maximumDuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<FreshpingClient, FreshpingClientImpl>()
    .ConfigureHttpJsonOptions(options => {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
        options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddSingleton(_ => new HttpClient(new UnfuckedHttpHandler(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromHours(1), MaxConnectionsPerServer = 16 }))
            { Timeout = TimeSpan.FromSeconds(30) }
        .Property(PropertyKey.JsonSerializerOptions, new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

await using WebApplication webApp = builder.Build();

webApp.MapGet("/{checkId:long}", async ([FromRoute] long checkId,
                                        [FromQuery] string? period,
                                        [FromQuery] byte? precision,
                                        [FromQuery] string? locale,
                                        [FromServices] FreshpingClient client) => {
    try {
        precision ??= 4; // 1/90/24/60*100 = 0.0007716 (all increments affect 4 digits after the decimal point)
        Duration reportDuration = period is not null && PeriodPattern.NormalizingIso.Parse(period) is { Success: true, Value: var p } && p.ToDuration() is var d
            ? d < minimumDuration ? minimumDuration
            : d > maximumDuration ? maximumDuration : d
            : defaultDuration;

        CheckStatus status = await client.fetchCheckStatus(checkId, reportDuration);

        try {
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = locale != null ? CultureInfo.GetCultureInfo(locale) : defaultCulture;
        } catch (CultureNotFoundException) {
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = defaultCulture;
        }

        return new ShieldsBadgeResponse(
            label: Resources.uptime,
            message: Math.Round(status.uptime, (int) precision + 2, MidpointRounding.ToNegativeInfinity).ToString("P" + precision),
            messageColor: status switch {
                { isUp: false }      => ShieldColor.CRITICAL,
                { uptime: >= 0.995 } => ShieldColor.BRIGHT_GREEN,
                { uptime: >= 0.990 } => ShieldColor.GREEN,
                { uptime: >= 0.980 } => ShieldColor.YELLOW_GREEN,
                { uptime: >= 0.975 } => ShieldColor.YELLOW,
                { uptime: >= 0.950 } => ShieldColor.ORANGE,
                { uptime: >= 0.910 } => ShieldColor.IMPORTANT,
                _                    => ShieldColor.CRITICAL
            },
            isError: !status.isUp,
            logo: freshpingLogo);
    } catch (FreshBadgeException e) {
        return new ShieldsBadgeResponse(Resources.error, e.Message, messageColor: ShieldColor.CRITICAL, isError: true, logo: freshpingLogo);
    }
});

await webApp.RunAsync();