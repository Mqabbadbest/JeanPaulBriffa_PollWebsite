using DataAccess.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.Models;

namespace Presentation.Controllers
{
    [Authorize]
    public class PollController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IPollRepository pollRepo, [FromServices] UserManager<IdentityUser> userManager)
        {

            var polls = pollRepo.GetPolls().OrderByDescending(p => p.DateCreated);

            if (pollRepo is PollFileRepository)
            {
                foreach(var poll in polls)
                {
                    poll.Author = await userManager.FindByIdAsync(poll.AuthorId);
                }
            }


            return View(polls);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromServices] IPollRepository pollRepo, [FromServices] UserManager<IdentityUser> userManager, PollCreateViewModel pollCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                var poll = new Poll
                {
                    Title = pollCreateViewModel.Title,
                    Description = pollCreateViewModel.Description,
                    Option1Text = pollCreateViewModel.Option1Text,
                    Option2Text = pollCreateViewModel.Option2Text,
                    Option3Text = pollCreateViewModel.Option3Text,
                    AuthorId = userManager.GetUserId(User),
                    DateCreated = DateTime.Now
                };
                pollRepo.CreatePoll(poll);
                TempData["PollCreationState"] = "Poll Created Successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["PollCreationState"] = "Error Creating Poll";

                return View(pollCreateViewModel);
            }
        }

        [HttpGet("/Poll/Vote")]
        [PreventDuplicateVoteActionFilter]
        public IActionResult Vote(Guid pollId, [FromServices] PollRepository pollRepo)
        {
            var poll = pollRepo.GetPolls().FirstOrDefault(p => p.Id == pollId);
            if(poll == null)
            {
                return RedirectToAction("Index");
            }

            return View(poll);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventDuplicateVoteActionFilter]
        public IActionResult Vote(Guid pollId, int selectedOption, [FromServices] PollRepository pollRepo, [FromServices] VoteRepository voteRepo, [FromServices] UserManager<IdentityUser> userManager)
        {
            var poll = pollRepo.GetPoll(pollId);
            if (poll == null)
            {
                TempData["VotingState"] = "Error getting poll!";
                return RedirectToAction("Index");
            }
            pollRepo.Vote(pollId, selectedOption);
            voteRepo.AddVote(new Vote
            {
                PollId = pollId,
                UserId = userManager.GetUserId(User),
                VotedAt = DateTime.Now
            });

            TempData["VotingState"] = "Vote Successful!";
            return RedirectToAction("Index");
        }
    }
}
