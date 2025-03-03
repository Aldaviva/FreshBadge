using FreshBadge.Data;
using NodaTime;
using System.Net;
using Unfucked;
using Unfucked.HTTP;

namespace FreshBadge;

public interface FreshpingClient {

    /// <exception cref="FreshBadgeException">the request to Freshping failed</exception>
    public Task<CheckStatus> fetchCheckStatus(long checkId, Duration duration);

}

public record CheckStatus(bool isUp, double uptime);

public class FreshpingClientImpl(HttpClient httpClient): FreshpingClient {

    private static readonly UrlBuilder CHECK_UPTIME_URL = UrlBuilder.FromTemplate("https://api.freshping.io/v1/public-check-stats-reports/{check_id}/{?start_time,end_time}");

    /// <inheritdoc />
    public async Task<CheckStatus> fetchCheckStatus(long checkId, Duration duration) {
        try {
            Instant now = SystemClock.Instance.GetCurrentInstant();

            Task<PublicCheckStatusReport> stateTask  = fetchCheckStatusReport(checkId, Duration.FromSeconds(1), now);
            Task<PublicCheckStatusReport> uptimeTask = fetchCheckStatusReport(checkId, duration, now);

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
    private async Task<PublicCheckStatusReport> fetchCheckStatusReport(long checkId, Duration duration, Instant now = default) {
        try {
            now = now == default ? SystemClock.Instance.GetCurrentInstant() : now;

            return await httpClient.Target(CHECK_UPTIME_URL)
                .ResolveTemplate(new {
                    check_id   = checkId,
                    start_time = now - duration,
                    end_time   = now
                }).Get<PublicCheckStatusReport>();
        } catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound) {
            throw new FreshBadgeException("Check must be added to a Freshping Status Page");
        }
    }

}