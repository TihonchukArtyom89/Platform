using Microsoft.Extensions.Caching.Distributed;
using Platform.Services;
using Platform.Models;
namespace Platform;
public class SumEndpoint
{
    public async Task Endpoint(HttpContext context,CalculationContext dataContext)
    {
        int count;
        int.TryParse((string?)context.Request.RouteValues["count"],out count);
        long total = dataContext.Calculations?.FirstOrDefault(c=>c.Count == count)?.Result ?? 0;
        if(total == 0)
        {
            for (int i = 1; i <= count; i++)
            {
                total += i;
            }
            dataContext.Calculations?.Add(new() { Count=count,Result=total});
            await dataContext.SaveChangesAsync();
        }
        string totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";
        await context.Response.WriteAsync($"({DateTime.Now.ToLongTimeString()}) Total for {count} values: {totalString}");
    }
}



/*
//caching responses
using Microsoft.Extensions.Caching.Distributed;
namespace Platform;

using Microsoft.AspNetCore.Routing;
using Platform.Services;
public class SumEndpoint
{
    public async Task Endpoint(HttpContext context,IDistributedCache cache,IResponseFormatter formatter,LinkGenerator generator)
    {
        int count;
        int.TryParse((string?)context.Request.RouteValues["count"],out count);
        long total = 0;
        for (int i = 1; i <= count; i++)
        {
            total += i;
        }
        string totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";
        context.Response.Headers["Cache-Control"] = "public, max-age=120";
        string? url = generator.GetPathByRouteValues(context, null, new { count = count });
        await formatter.Format(context,$"<div>({DateTime.Now.ToLongTimeString()}) Total for {count} values:</div><div>{totalString}</div><a href={url}>Reload</a>");
    }
}
*/

/* //caching data value
using Microsoft.Extensions.Caching.Distributed;
namespace Platform;

public class SumEndpoint
{
    public async Task Endpoint(HttpContext context,IDistributedCache cache)
    {
        int count;
        int.TryParse((string?)context.Request.RouteValues["count"],out count);
        string cacheKey = $"sum_{count}";
        string totalString = await cache.GetStringAsync(cacheKey);
        if(totalString==null)
        {
            long total = 0;
            for (int i = 1; i <= count; i++)
            {
                total += i;
            }
            totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";
            await cache.SetStringAsync(cacheKey, totalString, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)});
        }
        await context.Response.WriteAsync($"({DateTime.Now.ToLongTimeString()}) Total for {count}  values:\n{totalString}\n");
    }
}
*/