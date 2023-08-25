using Microsoft.AspNetCore.Mvc;
using Restaurant.Api.Domain;

namespace Restaurant.Api.Endpoints
{
    public static class TablesEndpoints
    {
        public static IEndpointRouteBuilder AddTablesEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            var group = routeBuilder.MapGroup("tables");

            group.MapPost("init", ([FromBody] List<int>? tables, RestManager manager) =>
            {
                var defaultTables = new List<int> { 2, 3, 4, 5, 6 };
                manager.InitTables(tables?.Any() == true ? tables : defaultTables);

                return Results.Ok();
            })
                .WithName("InitializeTables")
                .WithSummary("Initialize restaurant with custom tables set. Send empty or null to prepopulate with default tables set.")
                .WithOpenApi();

            group.MapGet("forClientsGroup", ([FromQuery] Guid id, RestManager manager) =>
            {
                return manager.Lookup(id);
            })
                .WithName("GetClientsGroupTable")
                .WithSummary("Return table for clients group")
                .WithOpenApi();

            return routeBuilder;
        }
    }
}

