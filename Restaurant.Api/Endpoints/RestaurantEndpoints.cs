using Restaurant.Api.Domain;

namespace Restaurant.Api.Endpoints
{
    public static class RestaurantEndpoints
    {
        public static IEndpointRouteBuilder AddRestaurantEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            var group = routeBuilder.MapGroup("restaurant");

            group.MapGet("/", (RestManager manager) => manager.GetRestaurantState())
                .WithName("GetsRestaurantState")
                .WithSummary("Gets restaurant current state")
                .WithOpenApi();

            return routeBuilder;
        }
    }
}
