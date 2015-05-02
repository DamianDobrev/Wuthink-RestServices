using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wuthink.RestServices.Models
{
    public class AnswerOutputModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public int Votes { get; set; }
    }
}
