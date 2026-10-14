using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Departments;

public class DepartmentLookupDto : EntityDto<Guid> // for dropdowns
{
    public string Name { get; set; } = string.Empty;
}
