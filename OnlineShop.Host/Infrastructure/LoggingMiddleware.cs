using System.Text;

namespace OnlineShop.Host.Infrastructure;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        await LogRequest(context);

        var originalResponseBody = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        await next.Invoke(context);
            
        await LogResponse(context, responseBody, originalResponseBody);
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalResponseBody)
    {
        var responseContent = new StringBuilder();
        responseContent.AppendLine("=== Response Info ===");
        
        responseContent.AppendLine("-- headers");
        foreach (var (headerKey, headerValue) in context.Response.Headers)
        {
            responseContent.AppendLine($"header = {headerKey}    value = {headerValue}");
        }

        responseContent.AppendLine("-- body");
        responseBody.Position = 0;
        var content = await new StreamReader(responseBody).ReadToEndAsync();
        responseContent.AppendLine($"body = {content}");
        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalResponseBody);
        context.Response.Body = originalResponseBody;

        logger.LogDebug(responseContent.ToString());
    }

    private async Task LogRequest(HttpContext context)
    {
        var requestContent = new StringBuilder();

        requestContent.AppendLine("=== Request Info ===");
        requestContent.AppendLine($"protocol = {context.Request.Protocol}");
        requestContent.AppendLine($"method = {context.Request.Method.ToUpper()}");
        requestContent.AppendLine($"path = {context.Request.Path}");

        requestContent.AppendLine("-- headers");
        foreach (var (headerKey, headerValue) in context.Request.Headers)
        {
            requestContent.AppendLine($"header = {headerKey}    value = {headerValue}");
        }

        requestContent.AppendLine("-- body");
        context.Request.EnableBuffering();
        var requestReader = new StreamReader(context.Request.Body);
        var content = await requestReader.ReadToEndAsync();
        requestContent.AppendLine($"body = {content}");

        logger.LogDebug(requestContent.ToString());
        context.Request.Body.Position = 0;
    }
}