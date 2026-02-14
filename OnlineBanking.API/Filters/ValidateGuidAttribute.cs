
namespace OnlineBanking.API.Filters;

public class ValidateGuidAttribute(params string[] keys) : ActionFilterAttribute
{
    private readonly List<string> _keys = [.. keys];

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        bool hasError = false;
        var apiError = new ErrorResponse();

        _keys.ForEach(k =>
        {
            if (!context.ActionArguments.TryGetValue(k, out var value)) return;
            if (!Guid.TryParse(value?.ToString(), out var guid))
            {
                hasError = true;
                apiError.Errors.Add($"The identifier for {k} is not a valid Guid format");
            }
        });

        if (hasError)
        {
            apiError.StatusCode = 400;
            apiError.StatusPhrase = "Bad Request";
            context.Result = new ObjectResult(apiError)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}