using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Departments;

public interface IDepartmentAppService : IApplicationService
{
    Task<DepartmentDto> GetAsync(Guid id);
    Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentListInput input);
    Task<ListResultDto<DepartmentLookupDto>> GetLookupAsync();
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto input);
    Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input);
    Task DeleteAsync(Guid id);
    Task<DepartmentDto> AddTeamAsync(Guid id, AddDepartmentTeamDto input);
    Task<DepartmentDto> RemoveTeamAsync(Guid id, Guid teamId);
}
