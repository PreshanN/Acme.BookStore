using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using NSubstitute;

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

        // 1. Global Override Switch: Stops Application and Domain test projects from running real migrations
        context.Services.AddTransient<Data.IBookStoreDbSchemaMigrator, NullBookStoreDbSchemaMigrator>();

        // 2. OpenIddict Dynamic Fake: Explicit registration prevents interface compilation errors
        var mockOpenIddictRepo = Substitute.For<Volo.Abp.OpenIddict.Applications.IOpenIddictApplicationRepository>();
        context.Services.AddSingleton<Volo.Abp.OpenIddict.Applications.IOpenIddictApplicationRepository>(mockOpenIddictRepo);
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

// ─── MIGRATOR STUB IMPLEMENTATION ───
public class NullBookStoreDbSchemaMigrator : Data.IBookStoreDbSchemaMigrator
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
