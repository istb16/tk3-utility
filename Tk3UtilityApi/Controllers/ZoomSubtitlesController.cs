using CommonLib;

using Microsoft.AspNetCore.Mvc;

using System.Security.Authentication;

using Tk3UtilityApi.Params.Zoom;

namespace Tk3UtilityApi.Controllers;

[Route("zoom/subtitles")]
public class ZoomSubtitlesController(IHttpClientFactory httpClientFactory) : BaseController
{
    private string GetZoomToken()
    {
        var ztoken = Request.Headers.FirstOrDefault(h => h.Key.Equals("zoom-token", StringComparison.CurrentCultureIgnoreCase)).Value;
        if (string.IsNullOrWhiteSpace(ztoken)) throw new AuthenticationException();
        return ztoken!;
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