using acme.BookStore.Departments;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Acme.BookStore.EntityFrameworkCore
{
    [Collection(BookStoreTestConsts.CollectionDefinitionName)]

    public class EfCoreDepartmentAppService_Tests : DepartmentAppService_Tests<BookStoreEntityFrameworkCoreTestModule>
    {

    }
}