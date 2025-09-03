using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Rise.Server.Common.Filters;

public class NoQueryParametersAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.QueryString.HasValue)
        {
            context.Result = new BadRequestObjectResult("Query parameters are not allowed for this endpoint");
            return;
        }

        base.OnActionExecuting(context);
    }
}
