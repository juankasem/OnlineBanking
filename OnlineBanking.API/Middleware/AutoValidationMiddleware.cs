using FluentValidation;
using System.Text.Json;

namespace OnlineBanking.API.Middleware;

public class AutoValidationMiddleware
{
    private readonly RequestDelegate _next;
    public AutoValidationMiddleware(RequestDelegate next)
    {
        _next = next;
 
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldValidate(context))
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var requestType = GetRequestType(endpoint);
                if (requestType != null)
                {
                    await ValidateRequestAsync(context, requestType);
                }
            }
        }

       await _next(context);
    }


    private static bool ShouldValidate(HttpContext context)
    {
        return context.Request.Method is "POST" or "PUT"
            && context.Request.ContentLength > 0
            && context.Request.ContentType?.StartsWith("application/json") == true;
    }

    private static Type? GetRequestType(Endpoint endpoint)
    {
        // Get the first parameter of the action method that isn't from the route or query
        return endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault()
            ?.Parameters
            .FirstOrDefault(p => !p.BindingInfo?.BindingSource?.Id.Equals("Path") == true
                && !p.BindingInfo?.BindingSource?.Id.Equals("Query") == true)
            ?.ParameterType;
    }

    private static async Task ValidateRequestAsync(HttpContext context, Type requestType)
    {
        string requestBody;
        using (var reader = new StreamReader(context.Request.Body))
        {
            requestBody = await reader.ReadToEndAsync();
            // Reset the request body stream position
            context.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody));
        }

        var request = JsonSerializer.Deserialize(requestBody, requestType);
        if (request == null) return;

        // Get the generic validator type
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);

        // Try to resolve the validator from DI
        var validator = context.RequestServices.GetService(validatorType) as IValidator;
        if (validator == null) return;

        // Perform validation
        var validationContext = Activator.CreateInstance(
            typeof(ValidationContext<>).MakeGenericType(requestType),
            request) as IValidationContext;

        if (validationContext == null) return;

        var validationResult = await validator.ValidateAsync(validationContext);

        if (!validationResult.IsValid)
        {
            // If validation fails, return 400 Bad Request with validation errors
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                new { errors = errors }
            );

            // Short-circuit the pipeline
            await context.Response.CompleteAsync();
        }
    }
}

public static class AutoValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseAutoValidation(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AutoValidationMiddleware>();
    }
}