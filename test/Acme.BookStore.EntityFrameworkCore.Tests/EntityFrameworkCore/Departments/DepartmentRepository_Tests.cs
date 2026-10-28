using Acme.BookStore.Departments;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Xunit;
using System.Threading.Tasks;  
 

namespace Acme.BookStore.EntityFrameworkCore.Departments
{
    [Collection(BookStoreTestConsts.CollectionDefinitionName)]
    public class DepartmentRepository_Tests : BookStoreEntityFrameworkCoreTestBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDataFilter _dataFilter;
        private readonly ICurrentTenant _currentTenant;

        public DepartmentRepository_Tests()
        {
            _departmentRepository = GetRequiredService<IDepartmentRepository>();
            _dataFilter = GetRequiredService<IDataFilter>();
            _currentTenant = GetRequiredService<ICurrentTenant>();
        }


        [Fact]
        public async Task Unique_Index_Should_Reject_Duplicate_Name_In_Same_Tenant()
        { 
            await Should.ThrowAsync<DbUpdateException>(() => WithUnitOfWorkAsync(async () =>
            {
                var copy = new Department(Guid.NewGuid(), "Engineering", tenantId: TestData.TechCorpTenantId);
                copy.SetCode("ENG-2");

                await _departmentRepository.InsertAsync(copy, autoSave: true); // bypasses DepartmentManager !
            }));
        }
    }
}   