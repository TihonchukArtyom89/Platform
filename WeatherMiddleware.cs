﻿namespace Platform;

public class WeatherMiddleware
{
    private RequestDelegate next;
    public WeatherMiddleware(RequestDelegate next)
    { 
        this.next = next; 
    }
    public async Task Invoke(HttpContext context)
    {
        if(context.Request.Path=="/middleware/class")
        {
            await context.Response.WriteAsync("Middleware Class: It is raining in London");
        }
        else
        {
            await next(context);
        }
    }
}
