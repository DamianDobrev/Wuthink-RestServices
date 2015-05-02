namespace Wuthink.RestServices.Models
{
    public class QuestionInputModel
    {
        public string Text { get; set; }

        public string Email { get; set; }

        public bool IsHidden { get; set; }

        public bool IsMultiChoice { get; set; }
    }
}
