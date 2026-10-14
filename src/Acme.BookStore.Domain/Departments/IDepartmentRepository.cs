using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Departments
{
    public interface IDepartmentRepository : IRepository<Department, Guid>
    {
        Task<bool> ExistsWithNameAsync(string name, Guid? excludedId = null);
    }
}
