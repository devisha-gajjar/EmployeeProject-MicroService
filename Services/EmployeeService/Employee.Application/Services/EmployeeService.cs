using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using Employee.Application.Interfaces;
using Employee.Domain.DTOs;
using Employee.Domain.Models;
using Employee.Shared.Constants;
using Employee.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;

namespace Employee.Application.Services;

public class EmployeeService(IEmployeeUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IEmployeeService
{
    public IEnumerable<EmployeeListDto> GetEmployees()
    {
        // Access the specific repository through the Unit of Work
        var employeesQuery = unitOfWork.Employees.GetQueryableInclude(
            includes: [e => e.Department]
        );

        var employees = employeesQuery.OrderBy(e => e.Id).ToList();

        return mapper.Map<IEnumerable<EmployeeListDto>>(employees);
    }

    public AddEmployeeViewModelDto? GetEmployeeById(int id)
    {
        var emp = unitOfWork.Employees.GetQueryableInclude(
            includes: [e => e.Department]
        ).FirstOrDefault(e => e.Id == id);

        if (emp == null)
            return null;

        return mapper.Map<AddEmployeeViewModelDto>(emp);
    }

    public async Task<EmployeeList?> SaveEmployee(AddEmployeeViewModelDto employeeDto)
    {
        // Change .Result to await
        var emailExists = await unitOfWork.Employees.Exists(e => e.Email == employeeDto.Email);

        if (emailExists && employeeDto.Id == 0)
        {
            throw new AppException("Email Already Exists!");
        }

        // email is being changed during update, check if the new email exists
        if (emailExists && employeeDto.Id != 0)
        {
            var existing = unitOfWork.Employees.GetById(employeeDto.Id);
            if (existing?.Email != employeeDto.Email)
            {
                throw new AppException("Email Already Exists!");
            }
        }

        EmployeeList employee;

        if (employeeDto.Id != 0)
        {
            employee = unitOfWork.Employees.GetById(employeeDto.Id) ?? throw new AppException(GlobalConstants.EMP_NOT_FOUND);

            mapper.Map(employeeDto, employee);
            unitOfWork.Employees.Update(employee);
        }
        else
        {
            employee = mapper.Map<EmployeeList>(employeeDto);
            employee.CreatedOn = DateTime.Now;
            unitOfWork.Employees.Add(employee);
        }

        unitOfWork.Save();

        var tenantSchema = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Schema"].ToString();

        // 2. Use Unit of Work to get Company Name (Mapping handles the 'public' part)
        var tenant = await unitOfWork.Tenants.GetByInclude(t => t.SchemaName == tenantSchema);
        var companyName = tenant?.CompanyName ?? "Company name";

        try
        {
            await SendWelcomeEmailMessage(employee.Email!, employee.Name, companyName);
        }
        catch (Exception ex)
        {
            throw new AppException($"RabbitMQ Error: {ex.Message}");
        }

        return unitOfWork.Employees.GetById(employee.Id);
    }

    public bool DeleteEmployee(int id)
    {
        var emp = unitOfWork.Employees.GetById(id) ?? throw new AppException(GlobalConstants.EMP_NOT_FOUND);

        unitOfWork.Employees.Delete(emp);

        unitOfWork.Save();

        return true;
    }

    public async Task<MemoryStream> ExportEmployees()
    {
        var employeesQuery = unitOfWork.Employees.GetQueryableInclude(
            includes: [e => e.Department]
        );

        var employees = employeesQuery.OrderBy(e => e.Id).ToList();

        var employeeDtos = mapper.Map<IEnumerable<EmployeeListDto>>(employees).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Employees");

        worksheet.Cell(1, 1).InsertTable(employeeDtos);

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return await Task.FromResult(stream);
    }

    public static async Task SendWelcomeEmailMessage(string email, string name, string companyName)
    {
        var cloudAmqpUrl = "amqps://uritzckv:qA0oJpwmjRoVQsFWOqQ_f8siQATbQa6S@puffin.rmq2.cloudamqp.com/uritzckv";

        var factory = new ConnectionFactory() { Uri = new Uri(cloudAmqpUrl) };

        // Use 'await' to get the actual connection and channel
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // In V7+, QueueDeclareAsync is used
        await channel.QueueDeclareAsync(queue: "email_queue",
                                       durable: true,
                                       exclusive: false,
                                       autoDelete: false);

        var messageObj = new
        {
            To = email,
            Subject = "Welcome to the company",
            TemplateType = "Welcome",
            Data = new Dictionary<string, string>
            {
                { "companyName", companyName },
                { "user", name },
                { "email", email },
                { "registrationDate", DateTime.Now.ToString("dd-MM-yyyy") },
                { "year", DateTime.Now.Year.ToString() }
            },
            Cc = Array.Empty<string>(),
            Bcc = Array.Empty<string>()
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messageObj));
        // In V7+, BasicPublishAsync is used
        await channel.BasicPublishAsync(exchange: "",
                                       routingKey: "email_queue",
                                       body: body);
    }
}