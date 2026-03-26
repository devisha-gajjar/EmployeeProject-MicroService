using MediatR;
using Tenant.Api.Features.Tenants.Commands;
using Tenant.API.Features.Tenants.Handler;

namespace Tenant.API.Endpoints;

public static class TenantEndpoints
{
    public static void MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenants")
                       .WithTags("Tenants");

        group.MapPost("/create", async (CreateTenantCommand command, IMediator mediator) =>
        {
            try
            {
                var schemaName = await mediator.Send(command);

                return Results.Created($"/api/tenants/{schemaName}", new
                {
                    Message = "Tenant created and workspace initialized successfully.",
                    Schema = schemaName
                });
            }
            catch (Exception ex)
            {
                // You can log the error here
                return Results.BadRequest(new
                {
                    Error = "Tenant Creation Failed",
                    ex.Message
                });
            }
        })
        .WithName("CreateTenant")
        .WithOpenApi();

        group.MapGet("/list", async (IMediator mediator) =>
        {
            var tenants = await mediator.Send(new GetAllTenantsQuery());
            return Results.Ok(tenants);
        })
        .WithName("GetAllTenants")
        .WithOpenApi();
    }
}