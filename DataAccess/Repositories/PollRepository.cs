using DataAccess.DataContext;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
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

        public void CreatePoll(Poll poll)
        {
            _pollContext.Polls.Add(poll);
            _pollContext.SaveChanges();
        }

        public bool UpdatePollDetails(Poll poll)
        {

            var pollToUpdate = GetPoll(poll.Id);
            if (pollToUpdate != null)
            {
                pollToUpdate.Title = poll.Title;
                pollToUpdate.Option1Text = poll.Option1Text;
                pollToUpdate.Option2Text = poll.Option2Text;
                pollToUpdate.Option3Text = poll.Option3Text;
                _pollContext.SaveChanges();
                return true;
            }

            return false;
        }

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

        public Poll? GetPoll(Guid id)
        {
            return _pollContext.Polls.FirstOrDefault(p => p.Id == id);
        }

        public IQueryable<Poll> GetPolls()
        {
            return _pollContext.Polls.Include(p => p.Author);
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
