using MediatR;

namespace Tenant.Api.Features.Tenants.Commands;

public record CreateTenantCommand(
    string CompanyName,
    string AdminEmail,
    string Username,
    string FirstName,
    string LastName,
    string Password) : IRequest<string>;