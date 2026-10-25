using Acme.BookStore.Departments;
using Acme.BookStore.Shared.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Acme.BookStore.EntityFrameworkCore.EntityTypeConfigurations
{
    public class DepartmentTeamConfiguration : IEntityTypeConfiguration<DepartmentTeam>
    {
        public void Configure(EntityTypeBuilder<DepartmentTeam> b)
        {
            b.ToTable(BookStoreConsts.DbTablePrefix + "DepartmentTeams", BookStoreConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(DepartmentConsts.MaxTeamNameLength);
        }
    }
}
