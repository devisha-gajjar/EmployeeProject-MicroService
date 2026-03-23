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
using RabbitMQ.Client;

namespace Employee.Application.Services;

public class EmployeeService(
    IEmployeeUnitOfWork unitOfWork,
    IMapper mapper) : IEmployeeService
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

    // public EmployeeList? AddEmployee(AddEmployeeViewModelDto employeeDto)
    // {
    //     var emailExists = unitOfWork.Employees.Exists(e => e.Email == employeeDto.Email).Result;

    //     if (emailExists)
    //         throw new AppException("Email Alredy Exist!");

    //     var emp = mapper.Map<EmployeeList>(employeeDto);
    //     emp.CreatedOn = DateTime.UtcNow;

    //     unitOfWork.Employees.Add(emp);

    //     unitOfWork.Save();

    //     return unitOfWork.Employees.GetById(emp.Id);
    // }

    // public bool UpdateEmployee(int id, AddEmployeeViewModelDto employeeDto)
    // {
    //     if (id != employeeDto.Id)
    //         return false;

    //     var existing = unitOfWork.Employees.GetById(id);

    //     if (existing == null)
    //         return false;

    //     if (existing.Email != employeeDto.Email)
    //     {
    //         var emailExists = unitOfWork.Employees.Exists(e => e.Email == employeeDto.Email).Result;
    //         if (emailExists) return false;
    //     }

    //     mapper.Map(employeeDto, existing);

    //     unitOfWork.Employees.Update(existing);

    //     // Save via Unit of Work
    //     unitOfWork.Save();

    //     return true;
    // }

    public async Task<EmployeeList?> SaveEmployee(AddEmployeeViewModelDto employeeDto)
    {
        var emailExists = unitOfWork.Employees.Exists(e => e.Email == employeeDto.Email).Result;

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

        try
        {
            await SendWelcomeEmailMessage(employee.Email!, employee.Name);
        }
        catch (Exception ex)
        {
            // Log error but don't stop the app (so the employee stays saved)
            Console.WriteLine($"RabbitMQ Error: {ex.Message}");
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

    public static async Task SendWelcomeEmailMessage(string email, string name)
    {
        var cloudAmqpUrl = "amqp://user:pass@hostname/vhost";

        var factory = new ConnectionFactory() { Uri = new Uri(cloudAmqpUrl) };

        // Use 'await' to get the actual connection and channel
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // In V7+, QueueDeclareAsync is used
        await channel.QueueDeclareAsync(queue: "email_queue",
                                       durable: true,
                                       exclusive: false,
                                       autoDelete: false);

        var messageObj = new { Email = email, Name = name, Type = "Welcome" };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messageObj));

        // In V7+, BasicPublishAsync is used
        await channel.BasicPublishAsync(exchange: "",
                                       routingKey: "email_queue",
                                       body: body);
    }
}