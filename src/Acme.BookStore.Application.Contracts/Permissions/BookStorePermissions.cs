namespace Acme.BookStore.Permissions;

public static class BookStorePermissions
{
    public const string GroupName = "BookStore";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    public static class Books
    {
        public const string Default = GroupName + ".Books";
    }

    public static class Authors
    {
        public const string Default = GroupName + ".Authors";
    }

     

   
    public static class Employees
    {
        public const string Default = GroupName + ".Employees";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Transfer = Default + ".Transfer";
        public const string Confirm = Default + ".Confirm";
        public const string Delete = Default + ".Delete";
        public const string ViewSalary = Default + ".ViewSalary";
        public const string ManageSalary = ViewSalary + ".Manage";
    }

    public static class Departments
    {
        public const string Default = GroupName + ".Departments";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ManageTeams = Default + ".ManageTeams";
    }
}