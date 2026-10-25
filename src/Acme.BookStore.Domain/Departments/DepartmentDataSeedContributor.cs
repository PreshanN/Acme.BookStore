using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
namespace Acme.BookStore.Departments
{
    public class DepartmentDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly DepartmentManager _departmentManager;
        private readonly ICurrentTenant _currentTenant;

        public DepartmentDataSeedContributor(
            IDepartmentRepository departmentRepository,
            DepartmentManager departmentManager,
            ICurrentTenant currentTenant)
        {
            _departmentRepository = departmentRepository;
            _departmentManager = departmentManager;
            _currentTenant = currentTenant;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            using (_currentTenant.Change(context?.TenantId))
            {
                if (await _departmentRepository.GetCountAsync() > 0)
                {
                    return; // Seed only once
                }

                var general = await _departmentManager.CreateAsync(
                    "General",
                    "Default department created by the data seeder"
                );

                await _departmentRepository.InsertAsync(
                    general,
                    autoSave: true
                );
            }
        }
    }
}