using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Departments
{
    public interface IDepartmentRepository : IRepository<Department, Guid>
    {
        Task<bool> ExistsWithNameAsync(string name, Guid? excludedId = null);

        Task<List<Department>> GetPagedListAsync(string? filter, string? sorting, int skipCount, int maxResultCount, CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(string? filter, CancellationToken cancellationToken = default);
    }
}
