using Acme.BookStore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Permissions;

public class BookStorePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {

        var myGroup = context.GetGroupOrNull(BookStorePermissions.GroupName)
       ?? context.AddGroup(BookStorePermissions.GroupName, L("Permission:BookStore"));

        var authorsPermission = myGroup.AddPermission(
            BookStorePermissions.Authors.Default,
            L("Permission:Authors")
        );
     
        myGroup.AddPermission(BookStorePermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(BookStorePermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        var departments = myGroup.AddPermission(
            BookStorePermissions.Departments.Default,
            L("Permission:Departments"));

        departments.AddChild(BookStorePermissions.Departments.Create, L("Permission:Departments.Create"));
        departments.AddChild(BookStorePermissions.Departments.Edit, L("Permission:Departments.Edit"));
        departments.AddChild(BookStorePermissions.Departments.Delete, L("Permission:Departments.Delete"));
        departments.AddChild(BookStorePermissions.Departments.ManageTeams, L("Permission:Departments.ManageTeams"));

        var employees = myGroup.AddPermission(
            BookStorePermissions.Employees.Default,
            L("Permission:Employees"));

        employees.AddChild(BookStorePermissions.Employees.Create, L("Permission:Employees.Create"));
        employees.AddChild(BookStorePermissions.Employees.Edit, L("Permission:Employees.Edit"));
        employees.AddChild(BookStorePermissions.Employees.Transfer, L("Permission:Employees.Transfer"));
        employees.AddChild(BookStorePermissions.Employees.Delete, L("Permission:Employees.Delete"));

        var viewSalary = employees.AddChild(
            BookStorePermissions.Employees.ViewSalary,
            L("Permission:Employees.ViewSalary"));
        viewSalary.AddChild(
            BookStorePermissions.Employees.ManageSalary,
            L("Permission:Employees.ManageSalary"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BookStorePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BookStoreResource>(name);
    }
}
