using DataAccess.DataContext;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PollRepository
    {
        private PollDbContext _pollContext;

        public PollRepository(PollDbContext pollContext)
        {
            _pollContext = pollContext;
        }

        public void AddPoll(Poll poll)
        {
            _pollContext.Polls.Add(poll);
            _pollContext.SaveChanges();
        }

        public bool UpdatePoll(Poll poll)
        {

            var pollToUpdate = GetPoll(poll.Id);
            if (pollToUpdate != null)
            {
                pollToUpdate.Title = poll.Title;
                pollToUpdate.Option1Text = poll.Option1Text;
                pollToUpdate.Option2Text = poll.Option2Text;
                pollToUpdate.Option3Text = poll.Option3Text;
                pollToUpdate.Option1VotesCount = poll.Option1VotesCount;
                pollToUpdate.Option2VotesCount = poll.Option2VotesCount;
                pollToUpdate.Option3VotesCount = poll.Option3VotesCount;
                _pollContext.SaveChanges();
                return true;
            }

            return false;
        }

        public Poll? GetPoll(Guid id)
        {
            return _pollContext.Polls.FirstOrDefault(p => p.Id == id);
        }

        public IQueryable<Poll> GetPolls()
        {
            return _pollContext.Polls;
        }

        public void RemovePoll(Guid id)
        {
            var poll = GetPoll(id);
            if (poll != null)
            {

                _pollContext.Polls.Remove(poll);
                _pollContext.SaveChanges();

            }
        }
    }
}
