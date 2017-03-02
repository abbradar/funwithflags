namespace funwithflags
{
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", _ => "Hello Foo");
            Get("/products/{id}", _ =>
            {
                return "Hello Bar";
            });
        }
    }
}
