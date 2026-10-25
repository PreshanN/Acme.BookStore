using Acme.BookStore.Employees;
using Xunit;

namespace Acme.BookStore.EntityFrameworkCore.Applications.Employees;

[Collection(BookStoreTestConsts.CollectionDefinitionName)]
public class EfCoreEmployeeAppService_Tests : EmployeeAppService_Tests<BookStoreEntityFrameworkCoreTestModule>
{

}
