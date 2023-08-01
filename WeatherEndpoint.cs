using Platform.Services;

namespace Platform;

public class WeatherEndpoint
{
    public static async Task Endpoint(HttpContext context)
    {
        IResponseFormatter? formatter = context.RequestServices.GetService<IResponseFormatter>();
        if (formatter == null) 
        { 
            await context.Response.WriteAsync($"No service for {typeof(WeatherEndpoint)} found"); 
        }
        else
        {
            await formatter.Format(context, "Endpoint Class: It is cloudly in Milan");
        }
    }
}
