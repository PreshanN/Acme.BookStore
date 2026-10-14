using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Departments;

public class DepartmentTeamDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
}
