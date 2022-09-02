using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineBanking.API.Filters;

public class ValidateBankAccountOwner : ActionFilterAttribute
{
    private readonly List<string> _keys;

    public ValidateBankAccountOwner(params string[] keys)
    {
        _keys = keys.ToList();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
    }
}