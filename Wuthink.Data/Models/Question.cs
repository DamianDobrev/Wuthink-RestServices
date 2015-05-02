using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wuthink.Data.Models;

namespace Wuthink.Data
{
    public class Question
    {
        public Question()
        {
            this.Answers = new HashSet<Answer>();
            this.Tags = new HashSet<Tag>();
            this.IpAddresses = new HashSet<VoterIp>();
            this.Id = Guid.NewGuid();
            this.DeleteUrl = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Text { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public string CreatorEmail { get; set; }

        [Required]
        public bool IsHidden { get; set; }

        [Required]
        public bool IsMultiChoice { get; set; }

        [Required]
        public virtual ICollection<VoterIp> IpAddresses { get; set; }

        [Required]
        public Guid DeleteUrl { get; set; }

        [Required]
        public int VotesCount { get; set; }
    }
}
