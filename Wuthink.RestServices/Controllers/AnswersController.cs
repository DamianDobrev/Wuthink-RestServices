using System.Linq;
using System.Web;
using System.Web.Http;
using Wuthink.Data;

namespace Wuthink.RestServices.Controllers
{
    public class AnswersController : ApiController
    {
        private WuthinkDbContext db = new WuthinkDbContext();

        //GET: api/answers/count
        [Route("api/answers/count")]
        public IHttpActionResult GetAnswersCount()
        {
            var result = db.Answers;
            int resultInt = result.Count();
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return Ok(resultInt);
        }
    }
}