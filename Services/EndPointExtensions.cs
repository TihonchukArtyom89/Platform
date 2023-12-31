﻿//using Platform.Services;
using System.Reflection;

public static class EndPointExtensions
{
    public static void MapEndpoint<T>(this IEndpointRouteBuilder app, string path, string methodName = "Endpoint")
    {
        MethodInfo? methodInfo = typeof(T).GetMethod(methodName);
        if (methodInfo == null || methodInfo.ReturnType != typeof(Task))
        {
            throw new System.Exception("Method cannot be used");
        }
        T endpointInstance = ActivatorUtilities.CreateInstance<T>(app.ServiceProvider);
        //app.MapGet(path,(RequestDelegate)methodInfo.CreateDelegate(typeof(RequestDelegate),endpointInstance));
        ParameterInfo[] methodParams = methodInfo!.GetParameters();
        //app.MapGet(path,context => (Task)(methodInfo.Invoke(endpointInstance,methodParams.Select(p=>p.ParameterType==typeof(HttpContext) ? context : context.RequestServices.GetService(p.ParameterType)).ToArray()))!);
        app.MapGet(path,context => 
        {
            T endpointInstance = ActivatorUtilities.CreateInstance<T>(context.RequestServices);
            return (Task)methodInfo.Invoke(endpointInstance!, methodParams.Select(p => p.ParameterType == typeof(HttpContext) ? context : context.RequestServices.GetService(p.ParameterType)).ToArray())!;
        });
    }
}
