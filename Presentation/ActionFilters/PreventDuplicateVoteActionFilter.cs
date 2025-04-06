using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.ActionFilters
{
    public class PreventDuplicateVoteActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// This method will validate if the user has already voted for a poll.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Getting the appropriate services needed for the action filter
            var httpContext = context.HttpContext;
            var services = httpContext.RequestServices;

            var voteRepo = services.GetRequiredService<VoteRepository>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var userId = userManager.GetUserId(httpContext.User);

            //Checking if the user is logged in.
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
            }

            var pollId = context.ActionArguments["pollId"] as Guid?;

            //Checking if the user has already voted for the poll
            if (pollId.HasValue && voteRepo.HasUserVoted(pollId.Value, userId))
            {
                var controller = context.Controller as Controller;
                controller.TempData.Add("VoteState", "duplicate");
                context.Result = new RedirectToActionResult("Index", "Poll", null);
            }


            base.OnActionExecuting(context);
        }

    }
}
