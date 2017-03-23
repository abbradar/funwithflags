namespace funwithflags
{
    using System.Linq;
    using System.Dynamic;
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule(DatabaseContext database)
        {
            Get("/", _ =>
                {
                    dynamic model = new ExpandoObject();
                    model.Count = database.Tests.Count();
                    return View["Index", model];
                });

            Get("/products/{id}", parameters =>
            {
                return $"Hello Bar, id: {parameters.id}";
            });
        }
    }
}
