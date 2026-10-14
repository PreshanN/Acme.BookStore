using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Departments;

public class DepartmentDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ContactInfoEmail { get; set; } // flattened from ContactInfo.Email
    public string? ContactInfoPhone { get; set; } // flattened from ContactInfo.Phone
    public List<DepartmentTeamDto> Teams { get; set; } = new();
}
