using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Acme.BookStore.Departments
{
    public class DepartmentManager : DomainService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentManager(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Department> CreateAsync(string name, string? description = null)
        {
            if (await _departmentRepository.ExistsWithNameAsync(name))
            {
                throw new BusinessException("BookStore:DuplicateDepartmentName").WithData("Name", name);
            }
            return new Department(GuidGenerator.Create(), name, description, CurrentTenant.Id);
        }
        public async Task ChangeNameAsync(Department department, string newName)
        {
            if (department.Name == newName) { return; }
            if (await _departmentRepository.ExistsWithNameAsync(newName, department.Id))
            {
                throw new BusinessException("BookStore:DuplicateDepartmentName").WithData("Name", newName);
            }
            department.SetName(newName);
        }
    }
}
