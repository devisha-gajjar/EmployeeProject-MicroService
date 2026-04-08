using AutoMapper;
using Dashboard.Domain.DTOs;
using Employee.API.Protos;

namespace Dashboard.Domain.Mapping;

public class DashboardMappingProfile : Profile
{
    public DashboardMappingProfile()
    {
        CreateMap<EmployeeDetailResponse, EmployeeDashboardDTO>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
            // handles the 'seconds' and 'nanos' conversion automatically
            .ForMember(dest => dest.EmploymentStartDate, opt => opt.MapFrom(src => src.StartDate.ToDateTime()))
            .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks))
            .ForMember(dest => dest.LeaveRequests, opt => opt.MapFrom(src => src.Leaves));

        // Map the repeated message types
        CreateMap<TaskInfo, TaskDTO>()
            .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<LeaveInfo, LeaveRequestDTO>()
            .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
    }
}