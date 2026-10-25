using Acme.BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Acme.BookStore.Departments
{
    public class EfCoreDepartmentRepository
        : EfCoreRepository<BookStoreDbContext, Department, Guid>,
          IDepartmentRepository
    {
        public EfCoreDepartmentRepository(
            IDbContextProvider<BookStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<bool> ExistsWithNameAsync(
            string name,
            Guid? excludedId = null)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet.AnyAsync(d =>
                d.Name == name &&
                (excludedId == null || d.Id != excludedId));
        }
        public async Task<List<Department>> GetPagedListAsync(
            string? filter,
            string? sorting,
            int skipCount,
            int maxResultCount,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilterAsync(filter);

            var sortingField = sorting.IsNullOrWhiteSpace()
                ? nameof(Department.Name)
                : sorting;

            return await query
                .OrderBy(sortingField)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string? filter,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilterAsync(filter);

            return await query.LongCountAsync(
                GetCancellationToken(cancellationToken));
        }

        private async Task<IQueryable<Department>> ApplyFilterAsync(
            string? filter)
        {
            var dbSet = await GetDbSetAsync();

            return dbSet.WhereIf(
                !filter.IsNullOrWhiteSpace(),
                d => EF.Functions.ILike(d.Name, $"%{filter}%"));
        }

        // Load child entities when the aggregate is loaded with details.
        public override async Task<IQueryable<Department>> WithDetailsAsync()
        {
            return (await GetQueryableAsync())
                .Include(d => d.Teams);
        }
    }
}
