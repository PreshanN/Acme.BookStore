using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Employees
{
    public interface IEmployeeRepository : IRepository<Employee, Guid>
    {
        Task<Employee?> FindByEmployeeCodeAsync(string employeeCode);
    }

}
