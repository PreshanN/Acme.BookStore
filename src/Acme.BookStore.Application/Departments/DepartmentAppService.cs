using Acme.BookStore.Employees;
using Acme.BookStore.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Departments;

[Authorize(BookStorePermissions.Departments.Default)]
public class DepartmentAppService : BookStoreAppService, IDepartmentAppService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly DepartmentManager _departmentManager;

    public DepartmentAppService(
        IEmployeeRepository employeeRepository,
        EmployeeManager employeeManager,
        IDepartmentRepository departmentRepository,
        DepartmentManager departmentManager)
        : base(employeeRepository, employeeManager)
    {
        _departmentRepository = departmentRepository;
        _departmentManager = departmentManager;
    }

    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var department = await _departmentRepository.GetAsync(id); // includes Teams
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public async Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentListInput input)
    {
        var query = await _departmentRepository.GetQueryableAsync();
        query = query.WhereIf(!input.Filter.IsNullOrWhiteSpace(),
            d => d.Name.Contains(input.Filter!));

        var totalCount = await AsyncExecuter.CountAsync(query);
        var departments = await AsyncExecuter.ToListAsync(
            query.OrderBy(input.Sorting.IsNullOrWhiteSpace()
                    ? nameof(Department.Name) : input.Sorting)
                .PageBy(input));

        return new PagedResultDto<DepartmentDto>(
            totalCount,
            ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments));
    }

    public async Task<ListResultDto<DepartmentLookupDto>> GetLookupAsync()
    {
        var departments = await _departmentRepository.GetListAsync();
        return new ListResultDto<DepartmentLookupDto>(
            ObjectMapper.Map<List<Department>, List<DepartmentLookupDto>>(departments));
    }

    [Authorize(BookStorePermissions.Departments.Create)]
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        var department = await _departmentManager.CreateAsync(input.Name, input.Description);
        SetContactInfo(department, input.ContactEmail, input.ContactPhone);
        await _departmentRepository.InsertAsync(department);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    [Authorize(BookStorePermissions.Departments.Create)]
    public async Task<DepartmentDto> CreateWithTeamsAsync(
    CreateDepartmentWithTeamsDto input)
    {
        var department = await _departmentManager.CreateAsync(
            input.Name,
            input.Description);

        // Set the optional contact information if your DTO includes it.
        SetContactInfo(
            department,
            input.ContactEmail,
            input.ContactPhone);

        foreach (var teamName in input.TeamNames)
        {
            department.AddTeam(
                GuidGenerator.Create(),
                teamName);
        }

        await _departmentRepository.InsertAsync(
            department,
            autoSave: true);

        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    [Authorize(BookStorePermissions.Departments.Edit)]
    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input)
    {
        var department = await _departmentRepository.GetAsync(id);
        await _departmentManager.ChangeNameAsync(department, input.Name); // unique-name rule
        department.SetDescription(input.Description);
        SetContactInfo(department, input.ContactEmail, input.ContactPhone);
        await _departmentRepository.UpdateAsync(department);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    [Authorize(BookStorePermissions.Departments.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _departmentRepository.DeleteAsync(id); // soft delete (FullAudited)
    }

    [Authorize(BookStorePermissions.Departments.ManageTeams)]
    public async Task<DepartmentDto> AddTeamAsync(Guid id, AddDepartmentTeamDto input)
    {
        var department = await _departmentRepository.GetAsync(id);
        department.AddTeam(GuidGenerator.Create(), input.Name); // duplicate + max-10 rules
        await _departmentRepository.UpdateAsync(department);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    [Authorize(BookStorePermissions.Departments.ManageTeams)]
    public async Task<DepartmentDto> RemoveTeamAsync(Guid id, Guid teamId)
    {
        var department = await _departmentRepository.GetAsync(id);
        department.RemoveTeam(teamId);
        await _departmentRepository.UpdateAsync(department);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    private static void SetContactInfo(Department department, string? email, string? phone)
    {
        department.SetContactInfo(
            string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone)
                ? null
                : new ContactInfo(email, phone));
    }
}
