namespace funwithflags
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using System.ComponentModel.DataAnnotations;

    // The main context class. It describes the whole database.
    public class DatabaseContext : DbContext
    {
        public DbSet<Test> Tests { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
    }

    // This is used for migrations.
    // It uses connection string from the environment.
    public class DatabaseContextFactory : IDbContextFactory<DatabaseContext>
    {
        public DatabaseContext Create(DbContextFactoryOptions options)
        {
            var connectionString = System.Environment.GetEnvironmentVariable("DATABASE");
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }

    public class Test
    {
        [Key]
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
