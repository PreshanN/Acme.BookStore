using Acme.BookStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Acme.BookStore.EntityFrameworkCore;

[DependsOn(
    typeof(BookStoreApplicationTestModule),
    typeof(BookStoreEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
)]
public class BookStoreEntityFrameworkCoreTestModule : AbpModule
{
    private AbpUnitTestSqliteDatabase? _database;

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpSqliteOptions>(x => x.BusyTimeout = null);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.SaveStaticFeaturesToDatabase = false;
            options.IsDynamicFeatureStoreEnabled = false;
        });
        Configure<PermissionManagementOptions>(options =>
        {
            options.SaveStaticPermissionsToDatabase = false;
            options.IsDynamicPermissionStoreEnabled = false;
        });
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

        ConfigureInMemorySqlite(context.Services);

        // ─── ADD THIS REGISTERED MOCK REPLACEMENT LINE RIGHT HERE ───
        // It tells the test application layer container to resolve our safe, 
        // empty implementation instead of executing the production migration files.
        context.Services.AddTransient<IBookStoreDbSchemaMigrator, NullBookStoreDbSchemaMigrator>();
    }

    private void ConfigureInMemorySqlite(IServiceCollection services)
    {
        _database = new AbpUnitTestSqliteDatabase();
        _database.CreateTables(
            new BookStoreDbContext(new DbContextOptionsBuilder<BookStoreDbContext>().UseSqlite(_database.ConnectionString).Options));

        services.Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = _database.ConnectionString;
        });

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(context =>
            {
                context.UseSqlite();
            });
        });
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _database?.Dispose();
    }
}

// ─── SAFE HOVER CONTAINER STUB IMPLEMENTATION AT THE VERY BOTTOM ───
public class NullBookStoreDbSchemaMigrator : IBookStoreDbSchemaMigrator, Volo.Abp.DependencyInjection.ITransientDependency
{
    public System.Threading.Tasks.Task MigrateAsync()
    {
        // No-op: Neutrals migration executions within integration test projects safely.
        return System.Threading.Tasks.Task.CompletedTask;
    }
}
