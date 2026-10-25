using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Employees
{
    public interface IEmployeeRepository : IRepository<Employee, Guid>
    { Task<Employee?> FindByEmployeeCodeAsync(string employeeCode);
        Task<List<Employee>> GetPagedListAsync(string? filter, Guid? departmentId, string? sorting, int skipCount, int maxResultCount, CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(string? filter, Guid? departmentId, CancellationToken cancellationToken = default);
    }
}
