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
                    ViewBag.Count = database.Tests.Count();
                    return View["Index"];
                });
            Get("/products/{id}", _ =>
            {
                return "Hello Bar";
            });
        }
    }
}
