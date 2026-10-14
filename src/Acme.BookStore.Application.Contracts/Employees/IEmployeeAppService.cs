using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Employees;

public interface IEmployeeAppService : IApplicationService
{
    Task<EmployeeDto> GetAsync(Guid id);
    Task<PagedResultDto<EmployeeDto>> GetListAsync(GetEmployeeListInput input);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto input);
    Task<EmployeeDto> UpdateSalaryAsync(Guid id, UpdateEmployeeSalaryDto input);
    Task<EmployeeDto> ChangeAddressAsync(Guid id, ChangeEmployeeAddressDto input);
    Task<EmployeeDto> TransferAsync(Guid id, TransferEmployeeDto input);
    Task DeleteAsync(Guid id);
}
