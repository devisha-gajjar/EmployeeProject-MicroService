using MediatR;

namespace Tenant.Api.Features.Tenants.Commands;

public record CreateTenantCommand(string CompanyName) : IRequest<string>;