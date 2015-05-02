using System;
using System.ComponentModel.DataAnnotations;

namespace Wuthink.Data.Models
{
    public class Answer
    {
        public Answer()
        {
            this.Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        [MinLength(1)]
        [MaxLength(60)]
        [Required]
        public string Text { get; set; }

        public Guid QuestionId { get; set; }

        public virtual Question Question { get; set; }

        [Required]
        public int Votes { get; set; }
    }
}
