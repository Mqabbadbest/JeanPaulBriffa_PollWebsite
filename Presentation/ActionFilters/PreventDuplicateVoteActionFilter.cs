using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Presentation.ActionFilters
{
    public class PreventDuplicateVoteActionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var services = httpContext.RequestServices;

            var voteRepo = services.GetRequiredService<VoteRepository>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var userId = userManager.GetUserId(httpContext.User);

            if(string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
            }

            if (context.ActionArguments.TryGetValue("pollId", out var pollIdObj) && pollIdObj is Guid pollId)
            {
                if (voteRepo.HasUserVoted(pollId, userId))
                {
                    var tempData = services.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(httpContext);
                    tempData["VoteState"] = "duplicate";
                    context.Result = new RedirectToActionResult("Index", "Poll", null);
                }
            }

            base.OnActionExecuting(context);
        }

    }
}
