using Microsoft.AspNetCore.Mvc.Filters;

namespace WebExtention.GlobalFilter
{
    public class GlobalModelStatusFilter: ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(!context.ModelState.IsValid)
            {
                //var errorResults = new List<>
            }
        }
    }
}
