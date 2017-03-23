namespace funwithflags
{
    using System.Linq;
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule(DatabaseContext database)
        {
            Get("/", _ =>
                {
                    var count = database.Tests.Count();
                    return $"Hello! Test count: {count}";
                });
            Get("/products/{id}", _ =>
            {
                return "Hello Bar";
            });
        }
    }
}
