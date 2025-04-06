using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class PollCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Option1Text { get; set; }

        [Required]
        public string Option2Text { get; set; }

        public string? Option3Text { get; set; }

    }
}
