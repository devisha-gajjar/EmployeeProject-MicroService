using AutoMapper;
using Dashboard.Application.Interfaces;
using Dashboard.Domain.DTOs;
using Employee.API.Protos;
using Grpc.Core; // Necessary for Metadata
using Microsoft.AspNetCore.Http;

namespace Dashboard.API.Services;

public class EmployeeGrpcClient(EmployeeGrpc.EmployeeGrpcClient client, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IEmployeeService
{
    private readonly EmployeeGrpc.EmployeeGrpcClient _client = client;
    private readonly IMapper _mapper = mapper;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<EmployeeDashboardDTO> GetEmployeeDashboardDataAsync()
    {
        // Extract the Schema Header from the current HTTP request
        var httpContext = _httpContextAccessor.HttpContext;
        var schemaName = httpContext?.Request.Headers["X-Tenant-Schema"].ToString();

        var userIdClaim = httpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID not found in token claims.");
        }

        // gRPC Metadata (this forwards header to Employee Service)
        var metadata = new Metadata();
        if (!string.IsNullOrEmpty(schemaName))
        {
            metadata.Add("X-Tenant-Schema", schemaName);
        }

        var request = new EmployeeRequest { Id = userIdClaim.ToString() };

        // Call server
        var response = await _client.GetEmployeeDetailsAsync(request, metadata);

        return _mapper.Map<EmployeeDashboardDTO>(response);
    }
}