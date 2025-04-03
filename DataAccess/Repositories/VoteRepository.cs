using DataAccess.DataContext;
using Domain.Models;

namespace DataAccess.Repositories
{
    public class VoteRepository
    {
        private PollDbContext _pollContext;

        public VoteRepository(PollDbContext pollContext)
        {
            _pollContext = pollContext;
        }

        /// <summary>
        /// This method will register a vote for the poll with the given pollId.
        /// </summary>
        /// <param name="vote">The details of the vote to be registered</param>
        public void AddVote(Vote vote)
        {
            _pollContext.Votes.Add(vote);
            _pollContext.SaveChanges();
        }

        /// <summary>
        /// This method checks if a user has already voted for a poll.
        /// It checks that the pollId and userId given are present in the database.
        /// If the user has already voted, it returns true.
        /// </summary>
        /// <param name="pollId">The Id of the poll that the user is trying to vote on</param>
        /// <param name="userId">The Id of the user that is trying to vote</param>
        /// <returns>Whether the user had already voted on a specific poll or not</returns>
        public bool HasUserVoted(Guid pollId, string userId)
        {
            return _pollContext.Votes.Any(v => v.PollId == pollId && v.UserId == userId);
        }

    }
}
