using System;
using System.Collections.Generic;

namespace Wuthink.RestServices.Models
{
    public class QuestionOutputModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public List<AnswerOutputModel> Answers { get; set; }

        public List<TagOutputModel> Tags { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatorEmail { get; set; }

        public bool IsHidden { get; set; }

        public bool IsMultiChoice { get; set; }

        public int Votes { get; set; }
    }
}