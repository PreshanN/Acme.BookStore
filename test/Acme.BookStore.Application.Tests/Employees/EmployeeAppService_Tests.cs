using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.BookStore.Departments;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;
using Xunit;

namespace Acme.BookStore.Employees;

public abstract class EmployeeAppService_Tests<TStartupModule> : BookStoreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IEmployeeAppService _employeeAppService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IClock _clock;

    protected EmployeeAppService_Tests()
    {
        _employeeAppService = GetRequiredService<IEmployeeAppService>();
        _departmentRepository = GetRequiredService<IDepartmentRepository>();
        _clock = GetRequiredService<IClock>();
    }

    [Fact]
    public async Task Should_Start_On_Probation_With_End_Date_Six_Months_After_Joining()
    {
        var joined = _clock.Now.Date;

        var created = await CreateEmployeeAsync(joined);

        created.IsConfirmed.ShouldBeFalse();
        created.ConfirmationDate.ShouldBeNull();
        created.ProbationEndDate.ShouldBe(joined.AddMonths(EmployeeConsts.ProbationMonths));
    }

    [Fact]
    public async Task Should_Confirm_After_Probation_Ends()
    {
        var joined = _clock.Now.Date.AddMonths(-(EmployeeConsts.ProbationMonths + 1));
        var created = await CreateEmployeeAsync(joined);

        var confirmed = await _employeeAppService.ConfirmAsync(created.Id);

        confirmed.IsConfirmed.ShouldBeTrue();
        confirmed.ConfirmationDate.ShouldNotBeNull();
        confirmed.ConfirmationDate.Value.Date.ShouldBe(_clock.Now.Date);

        var reloaded = await _employeeAppService.GetAsync(created.Id);
        reloaded.IsConfirmed.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Confirm_On_The_Probation_End_Date()
    {
        var joined = _clock.Now.Date.AddMonths(-EmployeeConsts.ProbationMonths);
        var created = await CreateEmployeeAsync(joined);
        created.ProbationEndDate.ShouldBe(_clock.Now.Date);

        var confirmed = await _employeeAppService.ConfirmAsync(created.Id);

        confirmed.IsConfirmed.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Not_Confirm_Before_Probation_Ends()
    {
        var created = await CreateEmployeeAsync(_clock.Now.Date);

        var exception = await Should.ThrowAsync<BusinessException>(
            () => _employeeAppService.ConfirmAsync(created.Id));

        exception.Code.ShouldBe(EmployeeManagementSystemDomainErrorCodes.ProbationNotCompleted);

        var reloaded = await _employeeAppService.GetAsync(created.Id);
        reloaded.IsConfirmed.ShouldBeFalse();
        reloaded.ConfirmationDate.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Not_Confirm_Twice()
    {
        var joined = _clock.Now.Date.AddMonths(-(EmployeeConsts.ProbationMonths + 1));
        var created = await CreateEmployeeAsync(joined);
        var first = await _employeeAppService.ConfirmAsync(created.Id);

        var exception = await Should.ThrowAsync<BusinessException>(
            () => _employeeAppService.ConfirmAsync(created.Id));

        exception.Code.ShouldBe(EmployeeManagementSystemDomainErrorCodes.EmployeeAlreadyConfirmed);

        var reloaded = await _employeeAppService.GetAsync(created.Id);
        reloaded.ConfirmationDate.ShouldBe(first.ConfirmationDate);
    }

    private async Task<EmployeeDto> CreateEmployeeAsync(DateTime dateOfJoining)
    {
        var department = (await _departmentRepository.GetListAsync()).First();
        var suffix = Guid.NewGuid().ToString("N").Substring(0, 8);

        return await _employeeAppService.CreateAsync(new CreateEmployeeDto
        {
            EmployeeCode = "P-" + suffix,
            FullName = "Probation " + suffix,
            Email = suffix + "@example.com",
            DepartmentId = department.Id,
            SalaryAmount = 1000,
            SalaryCurrency = "INR",
            DateOfJoining = dateOfJoining
        });
    }
}
