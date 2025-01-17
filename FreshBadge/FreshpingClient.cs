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

    // private static UriTemplate checkStateUrl => new("https://api.freshping.io/api/v1/checks/{check_id}/");
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

    /*/// <exception cref="FreshBadgeException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    private async Task<Check> fetchState(long checkId) {
        using HttpResponseMessage stateResponse = await http.SendAsync(new HttpRequestMessage(HttpMethod.Get, checkStateUrl.AddParameter("check_id", checkId).Resolve()) {
            Headers = {
                Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Latin1.GetBytes($"{config.Value.freshpingApiKey}:{config.Value.freshpingSubdomain}")))
            }
        });

        switch (stateResponse.StatusCode) {
            case HttpStatusCode.Unauthorized:
                throw new FreshBadgeException("Incorrect Freshping account subdomain or API key");
            case HttpStatusCode.NotFound:
                throw new FreshBadgeException("No Freshping Check with the given ID exists in the configured account");
            default:
                stateResponse.EnsureSuccessStatusCode();
                break;
        }

        return (await stateResponse.Content.ReadFromJsonAsync<Check>())!;
    }*/

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