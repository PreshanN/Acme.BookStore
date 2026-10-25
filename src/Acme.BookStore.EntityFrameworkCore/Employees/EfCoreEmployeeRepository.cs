using Acme.BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace Acme.BookStore.Employees
{

    public class EfCoreEmployeeRepository : EfCoreRepository<BookStoreDbContext, Employee, Guid>, IEmployeeRepository
    {
        public EfCoreEmployeeRepository(IDbContextProvider<BookStoreDbContext> dbContextProvider) : base(dbContextProvider) { }
        public async Task<Employee?> FindByEmployeeCodeAsync(string employeeCode) { var dbSet = await GetDbSetAsync(); return await dbSet.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode); }
        public async Task<List<Employee>> GetPagedListAsync(string? filter, Guid? departmentId, string? sorting, int skipCount, int maxResultCount, CancellationToken cancellationToken = default) { var query = await ApplyFilterAsync(filter, departmentId); return await query.OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Employee.FullName) : sorting).PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken)); }
        public async Task<long> GetCountAsync(string? filter, Guid? departmentId, CancellationToken cancellationToken = default) { var query = await ApplyFilterAsync(filter, departmentId); return await query.LongCountAsync(GetCancellationToken(cancellationToken)); }

        public override async Task<IQueryable<Employee>> WithDetailsAsync() { return (await GetQueryableAsync()).Include(e => e.Qualifications); }
        private async
            Task<IQueryable<Employee>> ApplyFilterAsync(string? filter, Guid? departmentId)
        {
            var dbSet = await GetDbSetAsync(); return dbSet.WhereIf(!filter.IsNullOrWhiteSpace(), e => EF.Functions.ILike(e.FullName, $"%{filter}%") || EF.Functions.ILike(e.EmployeeCode, $"%{filter}%")).WhereIf(departmentId.HasValue, e => e.DepartmentId == departmentId);
        }
    }
}