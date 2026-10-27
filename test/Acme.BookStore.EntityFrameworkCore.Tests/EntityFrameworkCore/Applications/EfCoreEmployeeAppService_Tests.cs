using Acme.BookStore.EntityFrameworkCore;
using Xunit;

namespace Acme.BookStore.Employees;

/// <summary>
/// Concrete integration test runner executing the abstract Employee App Service tests 
/// on top of an isolated, transactional in-memory SQLite relational database.
/// </summary>
public class EfCoreEmployeeAppService_Tests : EmployeeAppService_Tests<BookStoreEntityFrameworkCoreTestModule>
{
    // The class body remains empty! 
    // Inheriting EmployeeAppService_Tests and passing BookStoreEntityFrameworkCoreTestModule
    // tells the ABP Framework to boot the database container and execute every [Fact].
}
