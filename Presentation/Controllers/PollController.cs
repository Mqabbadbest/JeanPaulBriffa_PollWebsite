using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Linq;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromServices] PollRepository pollRepo)
        {
            var polls = pollRepo.GetPolls().OrderByDescending(p => p.DateCreated);
            return View(polls);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromServices] PollRepository pollRepo, [FromServices] UserManager<IdentityUser> userManager, PollCreateViewModel pollCreateViewModel)
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
                TempData["PollCreationState"] = "true";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["PollCreationState"] = "false";

                return View(pollCreateViewModel);
            }
        }

        [HttpGet("/Poll/Vote")]
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
        public IActionResult Vote(Guid pollId, int selectedOption, [FromServices] PollRepository pollRepo, [FromServices] VoteRepository voteRepo, [FromServices] UserManager<IdentityUser> userManager)
        {
            var poll = pollRepo.GetPoll(pollId);
            if (poll == null)
            {
                return RedirectToAction("Index");
            }
            pollRepo.Vote(pollId, selectedOption);
            voteRepo.AddVote(new Vote
            {
                PollId = pollId,
                UserId = userManager.GetUserId(User),
                VotedAt = DateTime.Now
            });
            return RedirectToAction("Index");
        }
    }
}
