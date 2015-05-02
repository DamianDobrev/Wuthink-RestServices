using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wuthink.Data.Models
{
    public class Tag
    {
        public Tag()
        {
            this.Questions = new HashSet<Question>();
            this.Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Name { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
