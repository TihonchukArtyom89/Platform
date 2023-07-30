//using Microsoft.Extensions.Options;
using Platform;
var builder = WebApplication.CreateBuilder(args);
//builder.Services.Configure<MessageOptions>(options => { options.CityName = "Albany"; });
builder.Services.Configure<RouteOptions>(opts => { opts.ConstraintMap.Add("countryName", typeof(CountryRouteConstraint)); });
var app = builder.Build();
//app.Use(async (context, next) => 
//{
//    await next();
//    await context.Response.WriteAsync($"\n Status Code:  {context.Response.StatusCode}");
//});
//app.Use(async (context, next) =>
//{
//    if(context.Request.Path == "/short")
//    {
//        await context.Response.WriteAsync($"Request Short Circuited");
//    }
//    else
//    {
//        await next();
//    }
//});
//app.Use(async (context, next) =>
//{
//    if ((context.Request.Method == HttpMethods.Get) && (context.Request.Query["custom"] == "true"))
//    {
//        context.Response.ContentType = "text/plain";
//        await context.Response.WriteAsync("Custom Middleware \n");
//    }
//    await next();
//});
//((IApplicationBuilder)app).Map("/branch", branch =>
//{
//    //branch.UseMiddleware<Platform.QueryStringMiddleware>();
//    //branch.Run(async (context) => 
//    //{
//    //    await context.Response.WriteAsync($"Branch Middleware");
//    //});
//    branch.Run(new Platform.QueryStringMiddleware().Invoke);
//});
//app.UseMiddleware<Platform.QueryStringMiddleware>();
//app.MapGet("/location", async (HttpContext context, IOptions<MessageOptions> msgOpts) => { Platform.MessageOptions opts = msgOpts.Value; await context.Response.WriteAsync($"{opts.CityName},{opts.CountryName}"); }) ;
//app.UseMiddleware<LocationMiddleware>();
//app.MapGet("/", () => "Hello World!");
//app.UseMiddleware<Population>();
//app.UseMiddleware<Capital>();
//app.UseRouting();
//app.MapGet("{first}/{second}/{third}",async context =>
//app.MapGet("{first:alpha:length(3)}/{second:bool}", async context =>
//{
//    await context.Response.WriteAsync("Request was routed\n");
//    foreach (var kvp in context.Request.RouteValues)
//    {
//        await context.Response.WriteAsync($"{kvp.Key}: {kvp.Value}\n");
//    }
//});
app.MapGet("capital/{country:countryName}", Capital.Endpoint);
//app.MapGet("capital/{country:regex(^uk|france|monaco$)}", Capital.Endpoint);
app.MapGet("size/{city?}", Population.Endpoint).WithMetadata(new RouteNameMetadata("population"));
app.MapFallback(async context => { await context.Response.WriteAsync("Routed to fallback endpoint"); });
//app.UseEndpoints(endpoints => 
//{
//    endpoints.MapGet("routing", async context =>
//    {
//        await context.Response.WriteAsync("Request was routed");
//    });
//    endpoints.MapGet("capital/uk", new Capital().Invoke);
//    endpoints.MapGet("population/paris", new Population().Invoke);
//});
//app.Run(async (context) => { await context.Response.WriteAsync("Terminal Middleware Reached"); });
app.Run();