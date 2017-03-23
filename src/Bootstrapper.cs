namespace funwithflags
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Nancy;
    using Nancy.Bootstrappers.Autofac;
    using Nancy.Configuration;
    using Autofac;

    /// <summary>
    /// Returns current working directory as the root path for views.
    /// </summary>
    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Directory.GetCurrentDirectory();
        }
    }
    
    /// <summary>
    /// Sets various aspects of how Nancy behaves.
    /// </summary>
    public class CustomBootstrapper : AutofacNancyBootstrapper
    {
        private DbContextOptions<DatabaseContext> dbContextOptions;
        
        public CustomBootstrapper(IConfiguration configuration)
        {
            // Build database connection options.
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseNpgsql(configuration["database"]);
            this.dbContextOptions = optionsBuilder.Options;
        }

        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);
            
            // Enable verbose error reporting.
            environment.Tracing(enabled: false, displayErrorTraces: true);
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }

        /// <summary>
        /// Called on each new request; registers objects that are provided to modules per request.
        /// </summary>
        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            var builder = new ContainerBuilder();

            // Provide database context if needed.
            // Disposable objects must be registered as single instances to ensure cleanup.
            builder.Register(c => new DatabaseContext(this.dbContextOptions)).As<DatabaseContext>().SingleInstance();

            builder.Update(container.ComponentRegistry);
        }
    }
}
