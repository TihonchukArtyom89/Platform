
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var servicesConfig = builder.Configuration;// - use configuration services to services

var app = builder.Build();
var pipelineConfig = app.Configuration;// - use configuration services to pipeline
app.MapGet("config", async (HttpContext context, IConfiguration config) => 
{
    string defaultDebug = config["Logging:LogLevel:Default"];
    await context.Response.WriteAsync($"The config setting is: {defaultDebug}");
});
app.MapGet("/", async context => 
{
    await context.Response.WriteAsync("Hello World");
});
app.Run();














































/*
//chapter 14 

//using Platform;
//using Platform.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(typeof(ICollection<>), typeof(List<>));
//IWebHostEnvironment env = builder.Environment;
//if(env.IsDevelopment())
//{
//    builder.Services.AddScoped<IResponseFormatter, TimeResponseFormatter>();
//    builder.Services.AddScoped<ITimeStamper, DefaultTimeStamper>();
//}
//else
//{
//    builder.Services.AddSingleton<IResponseFormatter, HtmlResponseFormatter>();
//}
//builder.Services.AddSingleton<IResponseFormatter, HtmlResponseFormatter>();
//IConfiguration config = builder.Configuration;
//builder.Services.AddScoped<IResponseFormatter>(serviceProvider => 
//{
//    string? typeName = config["services:IResponseFormatter"];
//    return (IResponseFormatter)ActivatorUtilities.CreateInstance(serviceProvider, typeName == null ? typeof(GuidService) : Type.GetType(typeName, true)!);
//});
//builder.Services.AddScoped<IResponseFormatter, TextResponseFormatter>();
//builder.Services.AddScoped<IResponseFormatter, HtmlResponseFormatter>();
//builder.Services.AddScoped<IResponseFormatter, GuidService>();
//builder.Services.AddScoped<ITimeStamper,DefaultTimeStamper>();
var app = builder.Build();
app.MapGet("string", async context =>
{
    ICollection<string> collection = context.RequestServices.GetRequiredService<ICollection<string>>();
    collection.Add($"Request: {DateTime.Now.ToLongTimeString()}");
    foreach (string str in collection)
    {
        await context.Response.WriteAsync($"String: {str}\n");
    }
});
app.MapGet("int", async context =>
{
    ICollection<int> collection = context.RequestServices.GetRequiredService<ICollection<int>>();
    collection.Add(collection.Count + 1);
    foreach (int val in collection)
    {
        await context.Response.WriteAsync($"Intðôòïó: {val}\n");
    }
});
//app.MapGet("single", async context =>
//{
//    IResponseFormatter formatter = context.RequestServices.GetRequiredService<IResponseFormatter>();
//    await formatter.Format(context, "Single service");
//});
//app.MapGet("/", async context =>
//{
//    IResponseFormatter formatter = context.RequestServices.GetServices<IResponseFormatter>().First(f => f.RichOutput);
//    await formatter.Format(context, "Multiple service");
//});
//app.UseMiddleware<WeatherMiddleware>();
//IResponseFormatter formatter = new TextResponseFormatter();
//app.MapGet("middleware/function",async (HttpContext context,IResponseFormatter formatter) => { await formatter.Format(context, "Middleware Functon: It is snowing in  Chicago"); } );
//app.MapGet("endpoint/class", WeatherEndpoint.Endpoint);
//app.MapWeather("endpoint/class");
//app.MapEndpoint<WeatherEndpoint>("endpoint/class");
//app.MapGet("endpoint/function", async (HttpContext context, IResponseFormatter formatter) => { await formatter.Format(context, "Endpoint Functon: It is sunny in  LA"); });
//app.MapGet("endpoint/function", async (HttpContext context) => { IResponseFormatter formatter = context.RequestServices.GetRequiredService<IResponseFormatter>(); await formatter.Format(context, "Endpoint Functon: It is sunny in  LA"); });
app.Run();*/

/*
//Chapter 12-13
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
//app.MapGet("capital/{country:countryName}", Capital.Endpoint);
////app.MapGet("capital/{country:regex(^uk|france|monaco$)}", Capital.Endpoint);
//app.MapGet("size/{city?}", Population.Endpoint).WithMetadata(new RouteNameMetadata("population"));
app.Use(async (context,next) => 
{
    Endpoint? end = context.GetEndpoint();
    if(end!=null)
    {
        await context.Response.WriteAsync($"{end.DisplayName} Selected\n");
    }
    else
    {
        await context.Response.WriteAsync("No Endpoint Selected\n");
    }
    await next();
});
app.Map("{number:int}", async context => { await context.Response.WriteAsync("Routed to the int endpoint"); }).WithDisplayName("Int Endpoint").Add(b => ((RouteEndpointBuilder)b).Order = 1);
app.Map("{number:double}", async context => { await context.Response.WriteAsync("Routed to the double endpoint"); }).WithDisplayName("Double Endpoint").Add(b => ((RouteEndpointBuilder)b).Order = 2);
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
*/