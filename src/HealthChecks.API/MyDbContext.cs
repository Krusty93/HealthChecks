#define WORKAROUND

using Microsoft.EntityFrameworkCore;

namespace HealthChecks.API
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<DbContext> opts)
            : base(opts)
        {
        }

#if WORKAROUND
        public MyDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=TestDb;User Id=sa;Password=Password1!;Encrypt = False");
        }
#endif
    }
}
