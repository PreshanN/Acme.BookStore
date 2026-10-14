 
using Acme.BookStore.Books;
using Acme.BookStore.Departments;
 
using Acme.BookStore.Departments;
using Acme.BookStore.Employees;
using Acme.BookStore.Employees;
using Acme.BookStore.Shared.Departments;
using Acme.BookStore.Shared.Departments;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
 
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Acme.BookStore.Authors;
using Acme.BookStore.Books;

namespace Acme.BookStore.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class BookStoreDbContext :
    AbpDbContext<BookStoreDbContext>,
      ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
  //  public DbSet<UserInvitation> UserInvitations { get; set; }

    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
  //  public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

 ;


        builder.Entity<Author>(b =>
        {
            b.ToTable("Authors");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            b.Property(x => x.ShortBio)
                .HasMaxLength(512);
        });

        builder.Entity<Book>(b =>
        {
            b.ToTable("Books");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            b.HasOne<Author>()
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .IsRequired();
        });

        builder.Entity<Department>(b =>
        {
            b.ToTable(BookStoreConsts.DbTablePrefix + "Departments", BookStoreConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(DepartmentConsts.MaxNameLength);
            b.Property(x => x.Description).HasMaxLength(DepartmentConsts.MaxDescriptionLength);

            b.OwnsOne(x => x.ContactInfo, c =>
            {
                c.Property(p => p.Email).HasColumnName("ContactEmail").HasMaxLength(256);
                c.Property(p => p.Phone).HasColumnName("ContactPhone").HasMaxLength(32);
            });

            b.HasMany(x => x.Teams).WithOne()
             .HasForeignKey("DepartmentId").IsRequired();
            b.Navigation(x => x.Teams).HasField("_teams");

            b.HasIndex(x => new { x.TenantId, x.Name });
        });

        builder.Entity<DepartmentTeam>(b =>
        {
            b.ToTable(BookStoreConsts.DbTablePrefix + "DepartmentTeams", BookStoreConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(DepartmentConsts.MaxTeamNameLength);
        });

        builder.Entity<Employee>(b =>
        {
            b.ToTable(BookStoreConsts.DbTablePrefix + "Employees", BookStoreConsts.DbSchema);
            b.ConfigureByConvention(); // ABP: concurrency stamp, soft delete, extra properties...

            b.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(EmployeeConsts.MaxEmployeeCodeLength);
            b.Property(x => x.FullName).IsRequired().HasMaxLength(EmployeeConsts.MaxFullNameLength);

            b.OwnsOne(x => x.Email, e =>
                e.Property(p => p.Value).HasColumnName("Email").IsRequired()
                 .HasMaxLength(EmployeeConsts.MaxEmailLength));

            b.OwnsOne(x => x.Salary, m =>
            {
                m.Property(p => p.Amount).HasColumnName("SalaryAmount").HasColumnType("decimal(18,2)");
                m.Property(p => p.Currency).HasColumnName("SalaryCurrency").HasMaxLength(EmployeeConsts.CurrencyLength);
            });

            b.OwnsOne(x => x.CurrentAddress, a =>
            {
                a.Property(p => p.Line1).HasColumnName("AddressLine1").HasMaxLength(256);
                a.Property(p => p.City).HasColumnName("AddressCity").HasMaxLength(64);
                a.Property(p => p.State).HasColumnName("AddressState").HasMaxLength(64);
                a.Property(p => p.PostalCode).HasColumnName("AddressPostalCode").HasMaxLength(16);
            });

            b.HasMany(x => x.Qualifications).WithOne()
             .HasForeignKey(q => q.EmployeeId).IsRequired();

            b.HasIndex(x => new { x.TenantId, x.EmployeeCode });
        });

        builder.Entity<EmployeeQualification>(b =>
        {
            b.ToTable(BookStoreConsts.DbTablePrefix + "EmployeeQualifications", BookStoreConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Degree).IsRequired().HasMaxLength(128);
        });
    }
}
