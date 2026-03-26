using AutoMapper;
using Tenant.Domain.Dtos;
using Tenant.Domain.Models;

namespace Tenant.Application.Mapping;

public class TenantMappingProfile : Profile
{
    public TenantMappingProfile()
    {
        CreateMap<User, TenantUserDto>();

        CreateMap<Domain.Models.Tenant, TenantDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? false))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn ?? DateTime.Now))
            .ForMember(dest => dest.AdminUser, opt => opt.MapFrom(src =>
                src.Users.FirstOrDefault(u => u.RoleId == 1)))
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));
    }
}