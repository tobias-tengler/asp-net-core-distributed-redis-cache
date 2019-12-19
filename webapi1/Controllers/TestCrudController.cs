using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Linq;

[Route("api/test")]
public class TestCrudController : ControllerBase
{
    private static readonly IList<TestModel> Items = new List<TestModel>();
    private readonly IDistributedCache _cache;

    public TestCrudController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpGet]
    public IList<TestModel> GetItems() => Items;

    [HttpPost]
    public async Task<TestModel> Add([FromBody] string text)
    {
        var item = new TestModel
        {
            Id = Guid.NewGuid(),
            Text = text
        };

        Items.Add(item);

        await _cache.SetStringAsync($"item_{item.Id}", JsonSerializer.Serialize(item));

        return item;
    }

    [HttpGet("{id}")]
    public async Task<TestModel> Get(string id)
    {
        var cachedItem = await _cache.GetStringAsync($"item_{id}");

        if (cachedItem != null)
            return JsonSerializer.Deserialize<TestModel>(cachedItem);

        // heavy database operation
        return Items.FirstOrDefault(i => i.Id.ToString() == id);
    }

    [HttpPut("{id}")]
    public async Task<TestModel> Update(string id, [FromBody] string text)
    {
        var item = Items.FirstOrDefault(i => i.Id.ToString() == id);

        if (item == null) throw new KeyNotFoundException();

        item.Text = text;

        await _cache.SetStringAsync($"item_{item.Id}", JsonSerializer.Serialize(item));

        return item;
    }

    [HttpDelete("{id}")]
    public async Task<bool> Delete(string id)
    {
        var item = Items.FirstOrDefault(i => i.Id.ToString() == id);

        if (item == null) return false;

        Items.Remove(item);

        await _cache.RemoveAsync($"item_{item.Id}");

        return true;
    }
}