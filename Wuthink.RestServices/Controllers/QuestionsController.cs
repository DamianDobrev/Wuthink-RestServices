using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Wuthink.Data;
using Wuthink.Data.Models;
using Wuthink.RestServices.Models;

namespace Wuthink.RestServices.Controllers
{
    public class QuestionsController : ApiController
    {
        private WuthinkDbContext db = new WuthinkDbContext();
        //GET: api/questions/by/date
        [Route("api/questions/by/{sortby}")]
        public IHttpActionResult GetQuestionsByDate(string sortby)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var questions = db.Questions;
            var questionsOutput = new List<QuestionOutputModel>();
            var result = new List<Question>();

            if (sortby.ToLowerInvariant() == "popularity")
            {
                result = questions
                    .Where(q => q.IsHidden == false)
                    .OrderByDescending(q => q.VotesCount)
                    .ThenByDescending(q => q.DateCreated).ToList();
            }

            else if (sortby.ToLowerInvariant() == "date")
            {
                result = questions
                    .Where(q => q.IsHidden == false)
                    .OrderByDescending(q => q.DateCreated)
                    .ThenByDescending(q => q.VotesCount).ToList();
            }
            else
            {
                return BadRequest("Invalid sorting method.");
            }

            questionsOutput.AddRange(result.Select(question => NewQuestionOutputModelFromQuestion(question)));
            return Ok(questionsOutput);
        }

        //GET: api/questions/by/date/tags/baba|qga
        [Route("api/questions/by/{sortby}/tags/{tags}")]
        public IHttpActionResult GetQuestions(string sortby, string tags)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            if (string.IsNullOrEmpty(tags))
            {
                return BadRequest("Tags should exist");
            }

            var questions = new HashSet<Question>();
            var questionsOutput = new List<QuestionOutputModel>();
            var tagsArray = tags.Split('|');
            var tagsAll = db.Tags.ToList();
            var usedIds = new List<Guid>();

            foreach (var tag in tagsArray)
            {
                var currentTag = tagsAll.FirstOrDefault(t => t.Name == tag);
                if (currentTag != null)
                {
                    foreach (var question in currentTag.Questions)
                    {
                        if (!usedIds.Contains(question.Id))
                        {
                            questions.Add(question);
                            usedIds.Add(question.Id);
                        }
                    }
                }
            }

            if (sortby == "popularity")
            {
                var result = questions
                    .Where(q => q.IsHidden == false)
                    .OrderByDescending(q => q.VotesCount)
                    .ThenBy(q => q.DateCreated).ToList();
                questionsOutput.AddRange(result.Select(question => NewQuestionOutputModelFromQuestion(question)));
            }

            else if (sortby == "date")
            {
                var result = questions
                    .Where(q => q.IsHidden == false)
                    .OrderByDescending(q => q.DateCreated)
                    .ThenBy(q => q.VotesCount).ToList();
                questionsOutput.AddRange(result.Select(question => NewQuestionOutputModelFromQuestion(question)));
            }
            else
            {
                return BadRequest("Invalid sorting method.");
            }
            return Ok(questionsOutput);
        }

        //GET: api/questions/count
        [Route("api/questions/count")]
        public IHttpActionResult GetQuestionsCount()
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = db.Questions;
            int resultInt = result.Count();
            return Ok(resultInt);
        }

        //GET: api/question/5
        [Route("api/question/{id}")]
        public IHttpActionResult GetQuestion(string id)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var questionResult = db.Questions.FirstOrDefault(q => q.Id.ToString() == id);
            var result = NewQuestionOutputModelFromQuestion(questionResult);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //GET: api/questions/programming|asdf|sasa
        [Route("api/questions/{tags}")]
        public IHttpActionResult GetQuestionsByTag(string tags)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var tagsArray = tags.Split('|');
            var tagsAll = db.Tags.ToList();
            var questionsOutput = new HashSet<QuestionOutputModel>();
            var usedIds = new List<Guid>();
            foreach (var tag in tagsArray)
            {
                var currentTag = tagsAll.FirstOrDefault(t => t.Name == tag);
                if (currentTag != null)
                {
                    foreach (var question in currentTag.Questions)
                    {
                        if (!usedIds.Contains(question.Id))
                        {
                            questionsOutput.Add(NewQuestionOutputModelFromQuestion(question));
                            usedIds.Add(question.Id);
                        }
                    }
                }
            }
            return Ok(questionsOutput);
        }

        //POST: api/questions
        [Route("api/questions")]
        public IHttpActionResult PostQuestion(QuestionInputModel questionData, [FromUri]string[] answer, [FromUri]string[] tag = null)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            string clientAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(questionData.Text))
            {
                return BadRequest("Question should be at least 1 character long.");
            }

            if (answer == null || answer.Count() <= 1)
            {
                return BadRequest("Question should have at least 2 answers");
            }

            var questionToBePosted = new Question()
            {
                Text = questionData.Text,
                CreatorEmail = questionData.Email,
                DateCreated = DateTime.Now,
                IsHidden = questionData.IsHidden,
                IsMultiChoice = questionData.IsMultiChoice,
                VotesCount = 0,
                Tags = new List<Tag>()
            };

            foreach (var singleAnswer in answer)
            {
                var regex = @"[\w .?!,+-@*^%()""'$]+";
                if (!Regex.IsMatch(singleAnswer, regex))
                {
                    return BadRequest("Answer names should consist of only letters of any alphabet, numbers and those symbols: .?!,+-@*^%()\"'$]+.");
                }
                questionToBePosted.Answers.Add(new Answer()
                {
                    Text = singleAnswer,
                    Votes = 0
                });
            }

            if (tag != null && tag.Length > 0 && tag[0] != null)
            {
                var allTags = db.Tags;
                foreach (var singleTag in tag)
                {
                    var regex = @"([\p{L}0-9])\w+";
                    if (!Regex.IsMatch(singleTag, regex))
                    {
                        return BadRequest("Tag names should consist of only letters of any alphabet and numbers, therefore spaces and symbols are not allowed.");
                    }

                    var thisTag = allTags.FirstOrDefault(t => t.Name == singleTag);

                    if (thisTag != null)
                    {
                        questionToBePosted.Tags.Add(thisTag);
                    }
                    else
                    {
                        questionToBePosted.Tags.Add(new Tag()
                        {
                            Name = singleTag.ToLower()
                        });
                    }
                }
            }

            db.Questions.Add(questionToBePosted);
            db.SaveChanges();

            if (!string.IsNullOrEmpty(questionToBePosted.CreatorEmail))
            {
                SendEmail(questionToBePosted);
            }

            return Ok(new
            {
                Id = questionToBePosted.Id,
                DeleteId = questionToBePosted.DeleteUrl
            });
        }


        //Post: api/questions/20/answers?a=12&a=15&a=16&a=17
        [Route("api/questions/{id}/answers")]
        public IHttpActionResult PostAnswersToQuestion(string id, [FromUri]string[] a = null)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            string clientAddress = HttpContext.Current.Request.UserHostAddress;

            var question = db.Questions.FirstOrDefault(q => q.Id.ToString() == id);

            if (question == null)
            {
                return BadRequest("Such question doesn't exist!");
            }
            
            if (question.IpAddresses.Any(ip => ip.Ip == clientAddress))
            {
                return BadRequest("You have already voted for this question.");
            }

            if (a == null || a.Length <= 0)
            {
                return BadRequest("No votes selected.");
            }

            if (!question.IsMultiChoice && a.Length > 1)
            {
                return BadRequest("Don't try to cheat! :]");
            }

            if (a.Any(singleAnswer => question.Answers.All(ans => ans.Id.ToString() != singleAnswer)))
            {
                return BadRequest("Don't try to cheat! :]");
            }

            foreach (var answer in a)
            {
                question.Answers.FirstOrDefault(ans => ans.Id.ToString() == answer).Votes++;
            }

            question.IpAddresses.Add( new VoterIp()
            {
                Ip = clientAddress
            });

            question.VotesCount++;
            db.SaveChanges();
            return Ok("Voted successfully from ip: " + clientAddress + ", number of people voted for this question is: " + question.VotesCount);
        }


        //POST: api/questions/delete/
        [Route("api/questions/delete/{id}")]
        public IHttpActionResult PostDeleteQuestion(string id)
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var questionToBeDeleted = db.Questions.FirstOrDefault(q => q.DeleteUrl.ToString() == id);

            if (questionToBeDeleted == null)
            {
                return NotFound();
            }

            db.Answers.RemoveRange(questionToBeDeleted.Answers);
            db.Questions.Remove(questionToBeDeleted);

            db.SaveChanges();

            return Ok(new {Message = "Question: \"" + questionToBeDeleted.Text + "\" deleted successfully."});
        }

        // Private methods

        protected static QuestionOutputModel NewQuestionOutputModelFromQuestion(Question q)
        {
            return new QuestionOutputModel()
            {
                Id = q.Id,
                Text = q.Text,
                CreatorEmail = q.CreatorEmail,
                Answers = q.Answers.Select(a => new AnswerOutputModel()
                {
                    Id = a.Id,
                    Text = a.Text,
                    Votes = a.Votes
                }).ToList(),
                Tags = q.Tags.Select(t => new TagOutputModel()
                {
                    Id = t.Id,
                    Text = t.Name
                }).ToList(),
                DateCreated = q.DateCreated,
                IsHidden = q.IsHidden,
                IsMultiChoice = q.IsMultiChoice,
                Votes = q.VotesCount
            };
        }
        private void SendEmail(Question questionToBePosted)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("wuthink.me@gmail.com", "123456qazwsx");
            mail.To.Add(new MailAddress(questionToBePosted.CreatorEmail));
            mail.From = new MailAddress("wuthink.me@gmail.com");
            mail.Subject = "You've posted a new question on Wuthink.damiandobrev.me!";
            mail.Body = "Hello, you have posted a question in Wuthink with text: " + questionToBePosted.Text +
                "\n \nLink to your question: http://projects.damiandobrev.me/wuthink/#/question/" + questionToBePosted.Id +
                "\nLink to delete your question: http://projects.damiandobrev.me/wuthink/#/question/" + questionToBePosted.Id + "/delete/" + questionToBePosted.DeleteUrl;
            client.Send(mail);
        }
    }
}
