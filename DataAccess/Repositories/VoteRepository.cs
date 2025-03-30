using DataAccess.DataContext;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class VoteRepository
    {
        private PollDbContext _pollContext;

        public VoteRepository(PollDbContext pollContext)
        {
            _pollContext = pollContext;
        }

        public void AddVote(Vote vote)
        {
            _pollContext.Votes.Add(vote);
            _pollContext.SaveChanges();
        }

        public bool HasUserVoted(Guid pollId, string userId)
        {
            return _pollContext.Votes.Any(v => v.PollId == pollId && v.UserId == userId);
        }

    }
}
