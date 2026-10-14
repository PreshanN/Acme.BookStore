using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.BookStore.Departments
{
    [Mapper]
    public partial class DepartmentToDepartmentDtoMapper
        : MapperBase<Department, DepartmentDto>
    {
        [MapProperty("ContactInfo.Email", nameof(DepartmentDto.ContactInfoEmail))]
        [MapProperty("ContactInfo.Phone", nameof(DepartmentDto.ContactInfoPhone))]
        public override partial DepartmentDto Map(Department source);

        [MapProperty("ContactInfo.Email", nameof(DepartmentDto.ContactInfoEmail))]
        [MapProperty("ContactInfo.Phone", nameof(DepartmentDto.ContactInfoPhone))]
        public override partial void Map(
            Department source,
            DepartmentDto destination);
    }
}
