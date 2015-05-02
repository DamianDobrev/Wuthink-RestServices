using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wuthink.Data.Models
{
    public class VoterIp
    {
        public VoterIp()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public string Ip { get; set; }

        public Guid QuestionId { get; set; }
    }
}
