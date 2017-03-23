namespace funwithflags
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using Nancy.Configuration;

    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Directory.GetCurrentDirectory();
        }
    }
    
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private DbContextOptions<DatabaseContext> dbContextOptions;
        
        // We get configuration from Startup class.
        public CustomBootstrapper(IConfiguration configuration)
        {
            // Build database connection options.
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseNpgsql(configuration["database"]);
            this.dbContextOptions = optionsBuilder.Options;
        }

        protected override IRootPathProvider RootPathProvider
        {
            // Return current directory as root.
            get { return new CustomRootPathProvider(); }
        }

        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);
            
            // Enable verbose error reporting.
            environment.Tracing(enabled: false, displayErrorTraces: true);
        }
        
        // Called on each new request; registers needed objects.
        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            // Create new database context and make it available.
            container.Register<DatabaseContext>().UsingConstructor(() => new DatabaseContext(this.dbContextOptions)).AsSingleton();
        }
    }
}
