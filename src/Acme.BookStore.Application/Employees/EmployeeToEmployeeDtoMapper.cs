using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.BookStore.Employees
{
    [Mapper]
    public partial class EmployeeToEmployeeDtoMapper
        : MapperBase<Employee, EmployeeDto>
    {
        public override partial EmployeeDto Map(Employee source);

        public override partial void Map(
            Employee source,
            EmployeeDto destination);
    }
}
