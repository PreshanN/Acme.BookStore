using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Employees;

public class GetEmployeeListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }      // name or employee code
    public Guid? DepartmentId { get; set; }
}
