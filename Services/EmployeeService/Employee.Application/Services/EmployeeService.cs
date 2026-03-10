using AutoMapper;
using ClosedXML.Excel;
using Employee.Application.Interfaces;
using Employee.Domain.DTOs;
using Employee.Domain.Models;
using Employee.Shared.Constants;
using Employee.Shared.Exceptions;

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

    public EmployeeList? SaveEmployee(AddEmployeeViewModelDto employeeDto)
    {
        // Check if the email exists in the database
        var emailExists = unitOfWork.Employees.Exists(e => e.Email == employeeDto.Email).Result;

        // If email exists and the employee is not being updated, throw an exception for add
        if (emailExists && employeeDto.Id == 0)
        {
            throw new AppException("Email Already Exists!");
        }

        // If the email is being changed during update, check if the new email exists
        if (emailExists && employeeDto.Id != 0)
        {
            var existing = unitOfWork.Employees.GetById(employeeDto.Id);
            if (existing?.Email != employeeDto.Email)
            {
                throw new AppException("Email Already Exists!");
            }
        }

        EmployeeList employee;

        // If the employee already exists, update it
        if (employeeDto.Id != 0)
        {
            employee = unitOfWork.Employees.GetById(employeeDto.Id) ?? throw new AppException(GlobalConstants.EMP_NOT_FOUND);

            mapper.Map(employeeDto, employee);
            unitOfWork.Employees.Update(employee);
        }
        else // If no ID, it's a new employee, so create one
        {
            employee = mapper.Map<EmployeeList>(employeeDto);
            employee.CreatedOn = DateTime.Now;
            unitOfWork.Employees.Add(employee);
        }

        // Save via Unit of Work
        unitOfWork.Save();

        // Return the saved employee entity
        return unitOfWork.Employees.GetById(employee.Id);
    }

    public bool DeleteEmployee(int id)
    {
        var emp = unitOfWork.Employees.GetById(id) ?? throw new AppException(GlobalConstants.EMP_NOT_FOUND);

        unitOfWork.Employees.Delete(emp);

        // Save via Unit of Work
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
}