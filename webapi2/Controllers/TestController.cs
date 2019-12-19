using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IDistributedCache _cache;

    public TestController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpGet("{id}")]
    public Task<string> Get(string id)
    {
        return _cache.GetStringAsync($"item_{id}");
    }
}