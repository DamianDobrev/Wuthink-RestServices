using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Wuthink.Data;
using Wuthink.Data.Models;
using Wuthink.RestServices.Models;

namespace Wuthink.RestServices.Controllers
{
    public class TagsController : ApiController
    {
        private WuthinkDbContext db = new WuthinkDbContext();

        // GET: api/Tags
        public IHttpActionResult GetTags()
        {
            var result = db.Tags
                .OrderByDescending(t => t.Name)
                .ThenBy(t => t.Id)
                .Select(t => new TagOutputModel()
                {
                    Id = t.Id,
                    Text = t.Name
                });
            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}