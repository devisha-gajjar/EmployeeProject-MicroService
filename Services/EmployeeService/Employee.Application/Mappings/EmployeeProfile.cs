using AutoMapper;
using Employee.Domain.DTOs;
using Employee.Domain.Models;

namespace Employee.Application.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {

        CreateMap<EmployeeList, EmployeeListDto>()
            .ForMember(dest => dest.DepartmentName, opt =>
                opt.MapFrom(src => src.Department != null ? src.Department.Name : "-"));

        CreateMap<EmployeeList, AddEmployeeViewModelDto>();

        CreateMap<AddEmployeeViewModelDto, EmployeeList>()

            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore());
    }
}