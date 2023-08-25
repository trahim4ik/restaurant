using Microsoft.AspNetCore.Mvc;
using Restaurant.Api.Domain;
using Restaurant.Api.Models;

namespace Restaurant.Api.Endpoints
{
    public static class ClientsGroupsEndpoints
    {
        public static IEndpointRouteBuilder AddClientsGroupsEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            var group = routeBuilder.MapGroup("clientsGroups");

            group.MapPost("arrive", ([FromBody] ClientsGroupArriveRequest request, RestManager manager) =>
            {
                if (manager.HasNoTableForClientsGroupSize(request.Size))
                {
                    return Results.BadRequest("Restaurant doesn't have table for such clients group size");
                }

                manager.Arrive(request.Size);

                return Results.Ok();
            })
                .WithName("ArriveClientsGroup")
                .WithSummary("Arrive clients group")
                .WithOpenApi();

            group.MapPost("leave", ([FromBody] ClientsGroup clientsGroup, RestManager manager) =>
            {
                manager.Leave(clientsGroup);

                return Results.Ok();
            })
                .WithName("LeaveClientsGroup")
                .WithSummary("Leave clients group")
                .WithOpenApi();

            return routeBuilder;
        }
    }
}

