using System.Data.Entity;
using Wuthink.Data.Models;

namespace Wuthink.Data
{
    public class WuthinkDbContext : DbContext
    {
        public WuthinkDbContext()
            :base("Wuthink")
        {
        }
        public static WuthinkDbContext Create()
        {
            return new WuthinkDbContext();
        }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Tag> Tags { get; set; }
    }
}
