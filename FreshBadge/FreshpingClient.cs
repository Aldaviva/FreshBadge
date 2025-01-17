using FreshBadge.Data;
using System.Net;
using System.Text.Json;
using Tavis.UriTemplates;

namespace FreshBadge;

public interface FreshpingClient {

    /// <exception cref="FreshBadgeException">the request to Freshping failed</exception>
    public Task<CheckStatus> fetchCheckStatus(long checkId, TimeSpan period);

}

public record CheckStatus(bool isUp, double uptime);

public class FreshpingClientImpl(HttpClient http): FreshpingClient {

    private static readonly JsonSerializerOptions JSON_OPTIONS = new(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private static UriTemplate checkUptimeUrl => new("https://api.freshping.io/v1/public-check-stats-reports/{check_id}/{?start_time,end_time}");

    /// <inheritdoc />
    public async Task<CheckStatus> fetchCheckStatus(long checkId, TimeSpan period) {
        try {
            DateTime now = DateTime.UtcNow;

            Task<PublicCheckStatusReport> stateTask  = fetchCheckStatusReport(checkId, TimeSpan.FromSeconds(1), now);
            Task<PublicCheckStatusReport> uptimeTask = fetchCheckStatusReport(checkId, period, now);

            PublicCheckStatusReport state  = await stateTask;
            PublicCheckStatusReport uptime = await uptimeTask;

            return new CheckStatus(state.outagesCountTotal == 0, uptime.uptimePercentage);
        } catch (HttpRequestException e) when (e.StatusCode is { } statusCode) {
            throw new FreshBadgeException($"{(int) statusCode} error from Freshping");
        } catch (HttpRequestException e) {
            throw new FreshBadgeException("Network error while connecting to Freshping", e);
        } catch (TaskCanceledException e) {
            throw new FreshBadgeException("Timeout while connecting to Freshping", e);
        }
    }

    /// <exception cref="FreshBadgeException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    private async Task<PublicCheckStatusReport> fetchCheckStatusReport(long checkId, TimeSpan period, DateTime utcNow = default) {
        try {
            utcNow = utcNow == default ? DateTime.UtcNow : utcNow;

            return (await http.GetFromJsonAsync<PublicCheckStatusReport>(checkUptimeUrl.AddParameters(new {
                check_id   = checkId,
                start_time = utcNow.Subtract(period).ToString("O"),
                end_time   = utcNow.ToString("O")
            }).Resolve(), JSON_OPTIONS))!;
        } catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound) {
            throw new FreshBadgeException("Check must be added to a Freshping Status Page");
        }
    }

}