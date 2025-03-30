using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Poll
    {
        public Poll()
        {
            Option1VotesCount = 0;
            Option2VotesCount = 0;
            Option3VotesCount = 0;
            DateCreated = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Option1Text { get; set; }

        public int Option1VotesCount { get; set; }

        [Required]
        public string Option2Text { get; set; }

        public int Option2VotesCount { get; set; }

        public string? Option3Text { get; set; }

        public int? Option3VotesCount { get; set; }

        public string AuthorId { get; set; }

        public IdentityUser Author { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
    }
}
