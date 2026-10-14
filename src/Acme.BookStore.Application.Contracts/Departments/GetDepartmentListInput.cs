using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Departments;

public class GetDepartmentListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
