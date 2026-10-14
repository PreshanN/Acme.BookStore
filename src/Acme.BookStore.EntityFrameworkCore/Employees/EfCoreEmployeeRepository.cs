using Acme.BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.BookStore.Employees
{
    public class EfCoreEmployeeRepository
    : EfCoreRepository<BookStoreDbContext, Employee, Guid>, IEmployeeRepository
    {
        public EfCoreEmployeeRepository(
            IDbContextProvider<BookStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<Employee?> FindByEmployeeCodeAsync(string employeeCode)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
        }
    }
}
