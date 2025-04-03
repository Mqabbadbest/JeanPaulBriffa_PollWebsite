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
    public class PollController : Controller
    {
        /// <summary>
        /// This method returns the Index view for the Polls.
        /// It retrieves the polls from the database or polls.json (based on the set configurations setting) and orders them by DateCreated.
        /// Then the View is returned with the polls.
        /// </summary>
        /// <param name="pollRepo">The instance of the poll repository being used.</param>
        /// <param name="userManager"></param>
        /// <returns>The Index view with the retrieved polls.</returns>
        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IPollRepository pollRepo, [FromServices] UserManager<IdentityUser> userManager)
        {

            var polls = pollRepo.GetPolls().OrderByDescending(p => p.DateCreated);

            //If the PollRepository is PollFileRepository, we need to get the Author of the poll from the UserManager.
            //As the Author is not stored in the polls.json file.
            //To make sure that this call is efficient and doesn't make multiple calls, I get all the dstinct AuthorIds from the polls and get all the users with those Ids.
            //Then give the Author to the polls based on the AuthorId.
            //This way I only make one call to the database to get all the users with the AuthorIds.
            if (pollRepo is PollFileRepository)
            {
                var authorIds = polls.Select(p => p.AuthorId).Distinct().ToList();
                var authors = userManager.Users.Where(u => authorIds.Contains(u.Id)).ToList();
                foreach (var p in polls)
                {
                    p.Author = authors.FirstOrDefault(u => u.Id == p.AuthorId);
                }
            }

            var pollsJsonData = polls.Select(p => new
            {
                p.Id,
                p.Title,
                p.Description,
                p.Option1Text,
                p.Option2Text,
                p.Option3Text,
                p.Option1VotesCount,
                p.Option2VotesCount,
                p.Option3VotesCount
            }).ToList();

            ViewBag.PollJsonData = System.Text.Json.JsonSerializer.Serialize(pollsJsonData);

            return View(polls);
        }


        /// <summary>
        /// This method returns the Create view for the Polls.
        /// </summary>
        /// <returns>The Create Poll view</returns>
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// This method creates a new poll.
        /// First it checks if the ModelState is valid, otherwise it returns the Create view with the PollCreateViewModel, so the user can correct the errors.
        /// Then it creates a new Poll object with the data from the PollCreateViewModel, and redirects to the Index view.
        /// </summary>
        /// <param name="pollRepo">The repository that is currently being used</param>
        /// <param name="userManager"></param>
        /// <param name="pollCreateViewModel">The data submitted by the user in the form to create the poll.</param>
        /// <returns></returns>
        [Authorize]
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

        /// <summary>
        /// This method returns the Vote view for the Polls.
        /// It retrieves the poll from the database or polls.json (based on the set configurations setting) and checks if it exists.
        /// Then it returns the Vote view with the poll, otherwise it redirects to the Index view.
        /// </summary>
        /// <param name="pollId">The Id of the poll the user wishes to vote on.</param>
        /// <param name="pollRepo">The repository being used.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/Poll/Vote")]
        [PreventDuplicateVoteActionFilter]
        public IActionResult Vote(Guid pollId, [FromServices] PollRepository pollRepo)
        {
            var poll = pollRepo.GetPolls().FirstOrDefault(p => p.Id == pollId);
            if (poll == null)
            {
                return RedirectToAction("Index");
            }

            return View(poll);
        }

        /// <summary>
        /// This method registers a vote for the poll with the given pollId.
        /// First it checks if the selectedOption is valid.
        /// Then it gives the vote details to the PollRepository to register the vote, increment the vote count, and save the changes to the database.
        /// If that is successful, it adds the vote to the VoteRepository and redirects to the Index view.
        /// </summary>
        /// <param name="pollId">The Id of the poll that the user is voting on</param>
        /// <param name="selectedOption">The option that the user voted for</param>
        /// <param name="pollRepo"></param>
        /// <param name="voteRepo"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventDuplicateVoteActionFilter]
        public IActionResult Vote(Guid pollId, int selectedOption, [FromServices] PollRepository pollRepo, [FromServices] VoteRepository voteRepo, [FromServices] UserManager<IdentityUser> userManager)
        {
            if (selectedOption > 0)
            {
                if (pollRepo.Vote(pollId, selectedOption))
                {
                    voteRepo.AddVote(new Vote
                    {
                        PollId = pollId,
                        UserId = userManager.GetUserId(User),
                        VotedAt = DateTime.Now
                    });
                    TempData["VotingState"] = "Vote Successful!";
                    return RedirectToAction("Index");
                }

                TempData["VotingState"] = "Error Voting!";
                return RedirectToAction("Index");
            }
            TempData["VotingState"] = "Error! Please select an option.";
            return RedirectToAction("Index");
        }
    }
}
