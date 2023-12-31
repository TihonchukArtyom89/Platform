//chapter 17 caching data, persistent cache with db, entityframework core
using Platform.Services;
using Platform.Models;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDistributedMemoryCache(opts => { opts.SizeLimit = 200; });
builder.Services.AddDistributedSqlServerCache(opts =>
{
    opts.ConnectionString = builder.Configuration["ConnectionStrings:CacheConnection"];
    opts.SchemaName = "dbo";
    opts.TableName = "DataCache";
});
builder.Services.AddResponseCaching();
builder.Services.AddSingleton<IResponseFormatter, HtmlResponseFormatter>();
builder.Services.AddDbContext<CalculationContext>(opts => 
{ 
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:CalcConnection"]);
    opts.EnableSensitiveDataLogging(true);
});
builder.Services.AddTransient<SeedData>();
var app = builder.Build();
app.UseResponseCaching();
app.MapEndpoint<Platform.SumEndpoint>("/sum/{count:int=1000000000}");
app.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
bool cmdLineInit = (app.Configuration["INITDB"] ?? "false") == "true";
if(app.Environment.IsDevelopment() || cmdLineInit)
{
    var seedData = app.Services.GetRequiredService<SeedData>();
    seedData.SeedDatabase();
}
if(!cmdLineInit)
{
    app.Run();
}















































/*
//chapter 16 filtering requests with host headers 
using Microsoft.AspNetCore.HostFiltering;
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<HostFilteringOptions>(opts => { opts.AllowedHosts.Clear(); opts.AllowedHosts.Add("*.example.com"); });
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error.html");
    app.UseStaticFiles();
}
app.UseStatusCodePages("text/html", Platform.Responses.DefaultResponse);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/error")
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await Task.CompletedTask;
    }
    else
    {
        await next();
    }
});
app.Run(context =>
{
    throw new Exception("Something has gone wrong");
});
app.Run();
//chapter 16 handling exceptions and errors
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error.html");
    app.UseStaticFiles();
}
app.UseStatusCodePages("text/html", Platform.Responses.DefaultResponse);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/error")
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await Task.CompletedTask;
    }
    else
    {
        await next();
    }
});
app.Run(context =>
{
    throw new Exception("Something has gone wrong");
});
app.Run();
//chapter 16 session data,https,hsts
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts => { opts.IdleTimeout = TimeSpan.FromMinutes(30); opts.Cookie.IsEssential = true; });
builder.Services.AddHsts(opts => { opts.MaxAge = TimeSpan.FromDays(1); opts.IncludeSubDomains = true;});
var app = builder.Build();
if(app.Environment.IsProduction())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseSession();
app.UseMiddleware<Platform.ConsentMiddleware>();
app.MapGet("/session",async context => 
{
    int counter1 = (context.Session.GetInt32("counter1") ?? 0) + 1;
    int counter2 = (context.Session.GetInt32("counter2") ?? 0) + 1;
    context.Session.SetInt32("counter1", counter1);
    context.Session.SetInt32("counter2", counter2);
    await context.Session.CommitAsync();
    await context.Response.WriteAsync($"Counter 1: {counter1}, Counter 2: {counter2},");
});
app.MapFallback(async context =>
{
    await context.Response.WriteAsync($"HTTPS Request: {context.Request.IsHttps}\n");
    await context.Response.WriteAsync("Hello World!");
});

app.Run();
*/


/*
//chapter 16 use cookies
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<CookiePolicyOptions>(opts => { opts.CheckConsentNeeded = context => true; });
var app = builder.Build();
app.UseCookiePolicy();
app.UseMiddleware<Platform.ConsentMiddleware>();
app.MapGet("/cookie", async context =>
{
    int counter1 = int.Parse(context.Request.Cookies["counter1"] ?? "0") + 1;
    context.Response.Cookies.Append("counter1", counter1.ToString(), new CookieOptions { MaxAge = TimeSpan.FromMinutes(30), IsEssential = true });
    int counter2 = int.Parse(context.Request.Cookies["counter2"] ?? "0") + 1;
    context.Response.Cookies.Append("counter2", counter1.ToString(), new CookieOptions { MaxAge = TimeSpan.FromMinutes(30) });
    await context.Response.WriteAsync($"Counter 1: {counter1}, Counter 2: {counter2},");
});
app.MapGet("clear", context =>
{
    context.Response.Cookies.Delete("counter1");
    context.Response.Cookies.Delete("counter2");
    context.Response.Redirect("/");
    return Task.CompletedTask;
});
app.MapFallback(async context => await context.Response.WriteAsync("Hello World!"));

app.Run();
*/



/*
 //chapter 15 -- static files and logging to console
using Platform;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.FileProviders; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(opts => { opts.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode; });
var app = builder.Build();
app.UseHttpLogging();
app.UseStaticFiles();
var env = app.Environment;
app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider($"{env.ContentRootPath}/staticfiles"),RequestPath = "/files" }); 
//var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Pipeline");
//logger.LogDebug("Pipeline configuration starting");
app.MapGet("population/{city?}", Population.Endpoint);
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World");
});
//logger.LogDebug("Pipeline configuration complete");
app.Run();
*/

/*
chapter 15 - using configuration services
//using Microsoft.AspNetCore.Http;

using Platform;

var builder = WebApplication.CreateBuilder(args);
var servicesConfig = builder.Configuration;// - use configuration services to services
builder.Services.Configure<MessageOptions>(servicesConfig.GetSection("Location"));
var servicesEnv = builder.Environment;// - use environment tp set up services 
var app = builder.Build();
var pipelineConfig = app.Configuration;// - use configuration services to pipeline
var pipelineEnv = app.Environment;// - use environment tp set up pipeline  

app.UseMiddleware<LocationMiddleware>();
app.MapGet("config", async (HttpContext context, IConfiguration config, IWebHostEnvironment env) => 
{
    string defaultDebug = config["Logging:LogLevel:Default"];
    string environ = config["ASPNETCORE_ENVIRONMENT"];
    await context.Response.WriteAsync($"The config setting is: {defaultDebug}\nThe env setting from configuration settings is: {environ}\nThe env from parameter of middleware component or endpoint is: {env.EnvironmentName}");
    string wsID = config["WebService:ID"];
    string wsKey = config["WebService:Key"];
    await context.Response.WriteAsync($"\nThe secret ID is: {wsID}\nThe secret Key is: {wsKey}");
});
app.MapGet("/", async context => 
{
    await context.Response.WriteAsync("Hello World");
});
app.Run();
*/

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
        await context.Response.WriteAsync($"Int�����: {val}\n");
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