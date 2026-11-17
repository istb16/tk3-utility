using CommonLib;

using Microsoft.AspNetCore.Mvc;

using System.Security.Authentication;

using Tk3UtilityApi.Params.Zoom;

namespace Tk3UtilityApi.Controllers;

[Route("zoom/subtitles")]
public class ZoomSubtitlesController(IHttpClientFactory httpClientFactory) : BaseController
{
    // The zoom-token header is the closed caption URL that Zoom issues ("API token" in the meeting).
    // Only forward requests to Zoom over https, so the API cannot be used to call arbitrary hosts.
    private static readonly string[] ZoomHosts = ["zoom.us", "zoom.com", "zoomgov.com"];

    private string GetZoomToken()
    {
        var ztoken = Request.Headers.FirstOrDefault(h => h.Key.Equals("zoom-token", StringComparison.CurrentCultureIgnoreCase)).Value;
        if (string.IsNullOrWhiteSpace(ztoken)) throw new AuthenticationException();
        if (!IsZoomUrl(ztoken!)) throw new AuthenticationException();
        return ztoken!;
    }

    private static bool IsZoomUrl(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)) return false;
        if (uri.Scheme != Uri.UriSchemeHttps) return false;
        var host = uri.IdnHost.ToLowerInvariant();
        return ZoomHosts.Any(z => host == z || host.EndsWith("." + z, StringComparison.Ordinal));
    }

    [HttpPost]
    public async Task<DateTime> Post([FromBody] RequestZoomSubtitle body)
    {
        var ztoken = GetZoomToken();
        var client = httpClientFactory.CreateClient();
        var url = $"{ztoken}&seq={body.Sequence}";

        var res = await client.PostAsync(url, new StringContent(body.Message));
        if (!res.IsSuccessStatusCode || res.StatusCode != System.Net.HttpStatusCode.OK)
            throw new ArgumentException(null, nameof(body));

        var resBody = await res.Content.ReadAsStringAsync();
        return resBody.ToDateTime();
    }

    [HttpGet("seq")]
    public async Task<int> GetSequence()
    {
        var ztoken = GetZoomToken();
        var client = httpClientFactory.CreateClient();
        var url = ztoken.Replace("/closedcaption?", "/closedcaption/seq?");

        var res = await client.GetAsync(url);
        if (!res.IsSuccessStatusCode || res.StatusCode != System.Net.HttpStatusCode.OK)
            throw new ArgumentException();

        var resBody = await res.Content.ReadAsStringAsync();
        return resBody.ToInt();
    }
}