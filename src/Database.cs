namespace funwithflags
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    public class DatabaseContext : DbContext
    {
        public DbSet<Test> Tests { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
    }

    public class Test
    {
        [Key]
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
