using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using System.Threading.Tasks;

namespace Acme.BookStore;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBackgroundJobsAbstractionsModule)
)]
public class BookStoreTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddAlwaysAllowAuthorization();

        // ─── THE GLOBAL OVERRIDE SWITCH ───
        // We inject our empty placeholder class to override the IBookStoreDbSchemaMigrator interface registry.
        // This stops all Application and Domain test projects from running real migrations.
        context.Services.AddTransient<Data.IBookStoreDbSchemaMigrator, NullBookStoreDbSchemaMigrator>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}

// ─── STUB IMPLEMENTATION ───
// This neutralizes migration executions inside all integration test projects safely.
public class NullBookStoreDbSchemaMigrator : Data.IBookStoreDbSchemaMigrator
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
