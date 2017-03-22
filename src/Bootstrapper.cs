namespace funwithflags
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private DbContextOptions<DatabaseContext> dbContextOptions = null;
        
        // We get configuration from Startup class.
        public CustomBootstrapper(IConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseNpgsql(configuration["database"]);
            this.dbContextOptions = optionsBuilder.Options;
        }
        
        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            // Create new database context and make it available.
            var dbcontext = new DatabaseContext(this.dbContextOptions);
            container.Register<DatabaseContext>(dbcontext);
        }
    }
}
