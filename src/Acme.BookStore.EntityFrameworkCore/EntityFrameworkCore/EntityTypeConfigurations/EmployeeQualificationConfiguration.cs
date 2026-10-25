using Acme.BookStore.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Acme.BookStore.EntityFrameworkCore.EntityTypeConfigurations
{
    public class EmployeeQualificationConfiguration : IEntityTypeConfiguration<EmployeeQualification>
    {
        public void Configure(
                 EntityTypeBuilder<EmployeeQualification> b)
        {
            b.ToTable(
                BookStoreConsts.DbTablePrefix + "EmployeeQualifications",
                BookStoreConsts.DbSchema
            );

            b.ConfigureByConvention();

            b.Property(x => x.Degree)
                .IsRequired()
                .HasMaxLength(128);
        }
    }
}
