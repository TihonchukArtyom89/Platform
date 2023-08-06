﻿namespace Platform.Services
{
    public class HtmlResponseFormatter:IResponseFormatter
    {
        public async Task Format(HttpContext context, string content)
        {
            context.Response.ContentType = "text/html";

            await context.Response.WriteAsync($@"
                    <!DOCTYPE>
                    <html lang=""en"">
                    <head><title>Formatted Response</title></head>
                    <body>
                        <h2>Formatted Response</h2>
                        <span>{content}</span>
                    </body>");                       
        }
        public bool RichOutput => true;
    }
}
