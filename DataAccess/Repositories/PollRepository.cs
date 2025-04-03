using DataAccess.DataContext;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PollRepository : IPollRepository
    {
        private PollDbContext _pollContext;

        public PollRepository(PollDbContext pollContext)
        {
            _pollContext = pollContext;
        }

        /// <summary>
        /// This method creates a new poll in the database.
        /// It checks if the poll is not null, adds it to the context, and saves the changes, and then returns true.
        /// Otherwise, it returns false.
        /// </summary>
        /// <param name="poll">The poll that is to be saved.</param>
        /// <returns>State of the poll creation</returns>
        public bool CreatePoll(Poll poll)
        {
            if (poll != null)
            {
                _pollContext.Polls.Add(poll);
                _pollContext.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method will register a vote for the poll with the given pollId.
        /// It attempts to find the poll in the database.
        /// Checks that it is truly a poll and not null.
        /// Then it checks the option number and increments the corresponding vote count.
        /// It saves the changes to the database and returns true.
        /// Otherwise, it returns false.
        /// </summary>
        /// <param name="pollId"></param>
        /// <param name="optionNumber"></param>
        /// <returns></returns>
        public bool Vote(Guid pollId, int optionNumber)
        {
            var poll = GetPoll(pollId);
            if (poll != null)
            {
                switch (optionNumber)
                {
                    case 1:
                        poll.Option1VotesCount++;
                        break;
                    case 2:
                        poll.Option2VotesCount++;
                        break;
                    case 3:
                        poll.Option3VotesCount++;
                        break;
                    default:
                        return false;
                }
                _pollContext.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method gets a poll from the database by its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The poll that has the given id</returns>
        public Poll? GetPoll(Guid id)
        {
            return _pollContext.Polls.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// This method gets all the polls from the database.
        /// The polls are returned as an IQueryable, to ensure efficiency and flexibility.
        /// </summary>
        /// <returns>An IQueryable list of polls</returns>
        public IQueryable<Poll> GetPolls()
        {
            return _pollContext.Polls.Include(p => p.Author);
        }
    }
}
