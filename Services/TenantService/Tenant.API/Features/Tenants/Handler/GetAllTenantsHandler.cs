using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tenant.Domain.Dtos;
using Tenant.Infrastructure.Interfaces;

namespace Tenant.API.Features.Tenants.Handler;

public record GetAllTenantsQuery : IRequest<List<TenantDto>>;

public class GetAllTenantsHandler(ITenantUnitOfWork uow, IMapper mapper) : IRequestHandler<GetAllTenantsQuery, List<TenantDto>>
{
    private readonly ITenantUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;

    public async Task<List<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        return await _uow.Tenants.GetAll()
         .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
         .ToListAsync(cancellationToken);
    }
}
