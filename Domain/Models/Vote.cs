using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Vote
    {
        public Guid PollId { get; set; }
        public string UserId { get; set; }

        public DateTime VotedAt { get; set; } = DateTime.Now;
    }
}
