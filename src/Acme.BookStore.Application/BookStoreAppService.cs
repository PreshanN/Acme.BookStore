using Acme.BookStore.Employees;
using Acme.BookStore.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore;

/* Inherit your application services from this class.
 * Only shared infrastructure lives here; endpoint methods belong on the
 * concrete services, because ABP exposes every public method of every
 * derived service as a remote API action.
 */
public abstract class BookStoreAppService : ApplicationService
{
    protected IEmployeeRepository _employeeRepository { get; }
    protected EmployeeManager _employeeManager { get; }

    protected BookStoreAppService(
        IEmployeeRepository employeeRepository,
        EmployeeManager employeeManager)
    {
        _employeeRepository = employeeRepository;
        _employeeManager = employeeManager;
    }

    protected virtual Task HideSalaryIfNotAllowedAsync(
        List<EmployeeDto> employees)
    {
        // Overridden by EmployeeAppService to redact salary fields.
        return Task.CompletedTask;
    }

    protected virtual Task<EmployeeDto> MapToDtoAsync(Employee employee)
    {
        return Task.FromResult(
            ObjectMapper.Map<Employee, EmployeeDto>(employee));
    }
}
