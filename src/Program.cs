namespace funwithflags
{
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.CommandLine;
    using Nancy.Owin;

    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var nancyOptions = new NancyOptions();
            // Boostrapper handles a lot of Nancy configuration.
            nancyOptions.Bootstrapper = new CustomBootstrapper(config);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .Configure(app => app.UseOwin(pl => pl.UseNancy(nancyOptions)))
                .Build();

            host.Run();
        }
    }
}
