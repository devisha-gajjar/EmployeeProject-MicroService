using AutoMapper;
using Employee.Domain.DTOs;
using Employee.Domain.Models;

namespace Employee.Application.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        // 1. Map from Domain Model to List DTO (For Get and Export)
        CreateMap<EmployeeList, EmployeeListDto>()
            // Handle the DepartmentName mapping custom logic (from your Export logic)
            .ForMember(dest => dest.DepartmentName, opt =>
                opt.MapFrom(src => src.Department != null ? src.Department.Name : "-"));

        // 2. Map from Domain Model to View Model DTO (For GetById)
        CreateMap<EmployeeList, AddEmployeeViewModelDto>();

        // 3. Map from View Model DTO to Domain Model (For Add and Update)
        CreateMap<AddEmployeeViewModelDto, EmployeeList>()
            // Ignore properties that shouldn't be overwritten by the user payload
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore());
    }
}