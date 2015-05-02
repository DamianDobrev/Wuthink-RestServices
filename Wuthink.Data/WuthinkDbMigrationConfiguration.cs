using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wuthink.Data
{
    public sealed class WuthinkDbMigrationConfiguration : DbMigrationsConfiguration<WuthinkDbContext>
    {
        public WuthinkDbMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WuthinkDbContext context)
        {
            // NOTHING TO SEE HERE
        }
    }
}
