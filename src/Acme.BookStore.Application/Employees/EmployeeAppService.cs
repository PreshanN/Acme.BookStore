using Acme.BookStore.Permissions;
using Acme.BookStore.Shared.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Employees;

[Authorize(BookStorePermissions.Employees.Default)]
public class EmployeeAppService : BookStoreAppService, IEmployeeAppService
{
    public EmployeeAppService(
      IEmployeeRepository employeeRepository,
      EmployeeManager employeeManager)
      : base(employeeRepository, employeeManager)
    {
    }
 
    public async Task<EmployeeDto> GetAsync(Guid id)
    {
        var employee = await _employeeRepository.GetAsync(id); // not found -> 404
        return await MapToDtoAsync(employee);
    }

    public async Task<PagedResultDto<EmployeeDto>> GetListAsync(GetEmployeeListInput input)
    {
        var query = await _employeeRepository.GetQueryableAsync();
        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                e => e.FullName.Contains(input.Filter!) ||
                     e.EmployeeCode.Contains(input.Filter!))
            .WhereIf(input.DepartmentId.HasValue,
                e => e.DepartmentId == input.DepartmentId);

        var totalCount = await AsyncExecuter.CountAsync(query);
        var employees = await AsyncExecuter.ToListAsync(
            query.OrderBy(input.Sorting.IsNullOrWhiteSpace()
                    ? nameof(Employee.FullName) : input.Sorting)
                .PageBy(input));

        var dtos = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employees);
        await HideSalaryIfNotAllowedAsync(dtos);
        return new PagedResultDto<EmployeeDto>(totalCount, dtos);
    }
     
    [Authorize(BookStorePermissions.Employees.Create)]
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto input)
    {
        var employee = await _employeeManager.CreateAsync(
            input.EmployeeCode,
            input.FullName,
            new EmailAddress(input.Email),
            input.DepartmentId,
            new Money(input.SalaryAmount, input.SalaryCurrency),
            input.DateOfJoining);

        await _employeeRepository.InsertAsync(employee);
        return await MapToDtoAsync(employee);
    }

    [Authorize(BookStorePermissions.Employees.ManageSalary)]
    public async Task<EmployeeDto> UpdateSalaryAsync(Guid id, UpdateEmployeeSalaryDto input)
    {
        var employee = await _employeeRepository.GetAsync(id);
        employee.SetSalary(new Money(input.Amount, input.Currency)); // domain rule: > 0
        await _employeeRepository.UpdateAsync(employee);
        return await MapToDtoAsync(employee);
    }

    [Authorize(BookStorePermissions.Employees.Edit)]
    public async Task<EmployeeDto> ChangeAddressAsync(Guid id, ChangeEmployeeAddressDto input)
    {
        var employee = await _employeeRepository.GetAsync(id);
        employee.ChangeAddress(new Address(input.Line1, input.City, input.State, input.PostalCode));
        await _employeeRepository.UpdateAsync(employee);
        return await MapToDtoAsync(employee);
    }

    [Authorize(BookStorePermissions.Employees.Transfer)]
    public async Task<EmployeeDto> TransferAsync(Guid id, TransferEmployeeDto input)
    {
        var employee = await _employeeRepository.GetAsync(id);
        await _employeeManager.TransferAsync(employee, input.NewDepartmentId);
        await _employeeRepository.UpdateAsync(employee);
        return await MapToDtoAsync(employee);
    }

    [Authorize(BookStorePermissions.Employees.ManageSalary)]
    public async Task<EmployeeDto> PromoteAsync(Guid id, PromoteEmployeeDto input)
    {
        var employee = await _employeeRepository.GetAsync(id);
        await _employeeManager.TransferAsync(employee, input.NewDepartmentId); // department must exist
        employee.SetSalary(new Money(input.NewSalaryAmount, input.Currency));  // domain rule: > 0
        await _employeeRepository.UpdateAsync(employee);
        return await MapToDtoAsync(employee);
    }

    [Authorize(BookStorePermissions.Employees.Confirm)]
    public async Task<EmployeeDto> ConfirmAsync(Guid id)
    {
        var employee = await _employeeRepository.GetAsync(id);
        employee.Confirm(Clock.Now); // domain rules: probation ended, not already confirmed
        await _employeeRepository.UpdateAsync(employee);
        return await MapToDtoAsync(employee);
    }

    [Authorize(BookStorePermissions.Employees.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _employeeRepository.DeleteAsync(id); // ISoftDelete -> IsDeleted = true
    }

    protected override async Task<EmployeeDto> MapToDtoAsync(Employee employee)
    {
        var dto = ObjectMapper.Map<Employee, EmployeeDto>(employee);
        await HideSalaryIfNotAllowedAsync(new List<EmployeeDto> { dto });
        return dto;
    }

    protected override async Task HideSalaryIfNotAllowedAsync(List<EmployeeDto> dtos)
    {
        if (await AuthorizationService.IsGrantedAsync(BookStorePermissions.Employees.ViewSalary))
        {
            return;
        }

        foreach (var dto in dtos)
        {
            dto.SalaryAmount = null;
            dto.SalaryCurrency = null;
        }
    }
}
