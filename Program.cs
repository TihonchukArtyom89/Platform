//using Microsoft.Extensions.Options;
using Platform;
var builder = WebApplication.CreateBuilder(args);
//builder.Services.Configure<MessageOptions>(options => { options.CityName = "Albany"; });
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
app.UseMiddleware<Population>();
app.UseMiddleware<Capital>();
app.Run(async context => { await context.Response.WriteAsync("Terminal Middleware Reached"); });
app.Run();