using Acme.BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.BookStore.Departments
{
    public class EfCoreDepartmentRepository
        : EfCoreRepository<BookStoreDbContext, Department, Guid>, IDepartmentRepository
    {
        public EfCoreDepartmentRepository(
            IDbContextProvider<BookStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludedId = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.AnyAsync(d =>
                d.Name == name && (excludedId == null || d.Id != excludedId));
        }

        // Load the child entities whenever the aggregate is loaded "with details"
        public override async Task<IQueryable<Department>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).Include(d => d.Teams);
        }
    }
}
