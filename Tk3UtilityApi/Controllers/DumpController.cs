using Microsoft.AspNetCore.Mvc;

namespace Tk3UtilityApi.Controllers;

public class DumpController : BaseController
{
    public DumpController() { }

    private async Task<string> GetBodyStringAsync()
    {
        using var sr = new StreamReader(Request.Body);
        return await sr.ReadToEndAsync();
    }

    private async Task<object> GetRequestAsync() => new
    {
        Method = Request.Method,
        ContentType = Request.ContentType,
        Url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
        Query = Request.Query,
        Headers = Request.Headers.Select(h => new { h.Key, Value = h.Value.FirstOrDefault() }),
        Cookies = Request.Cookies.Select(c => new { c.Key, Value = c.Value.FirstOrDefault() }),
        Body = await GetBodyStringAsync(),
    };

    [HttpGet]
    public async Task<object> Get() => await GetRequestAsync();

    [HttpPost]
    public async Task<object> Post() => await GetRequestAsync();

    [HttpPut]
    public async Task<object> Put() => await GetRequestAsync();

    [HttpPatch]
    public async Task<object> Patch() => await GetRequestAsync();

    [HttpDelete]
    public async Task<object> Delete() => await GetRequestAsync();
}