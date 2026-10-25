using Acme.BookStore.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Acme.BookStore.Departments;
using Acme.BookStore.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.BookStore.EntityFrameworkCore.EntityTypeConfigurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> b)
        {
            b.ToTable(
                BookStoreConsts.DbTablePrefix + "Employees",
                BookStoreConsts.DbSchema
            );

            b.ConfigureByConvention();

            // Employee base columns
            b.Property(x => x.EmployeeCode)
                .IsRequired()
                .HasMaxLength(EmployeeConsts.MaxEmployeeCodeLength);

            b.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(EmployeeConsts.MaxFullNameLength);

            // Email value object
            b.OwnsOne(x => x.Email, e =>
            {
                e.Property(p => p.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(EmployeeConsts.MaxEmailLength);
            });

            // Salary value object
            b.OwnsOne(x => x.Salary, m =>
            {
                m.Property(p => p.Amount)
                    .HasColumnName("SalaryAmount")
                    .HasPrecision(18, 2);

                m.Property(p => p.Currency)
                    .HasColumnName("SalaryCurrency")
                    .HasMaxLength(EmployeeConsts.CurrencyLength);
            });

            // Current address value object
            b.OwnsOne(x => x.CurrentAddress, a =>
            {
                a.Property(p => p.Line1)
                    .HasColumnName("AddressLine1")
                    .HasMaxLength(256);

                a.Property(p => p.City)
                    .HasColumnName("AddressCity")
                    .HasMaxLength(64);

                a.Property(p => p.State)
                    .HasColumnName("AddressState")
                    .HasMaxLength(64);

                a.Property(p => p.PostalCode)
                    .HasColumnName("AddressPostalCode")
                    .HasMaxLength(16);
            });

            // Department relationship
            b.HasOne<Department>()
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Employee qualifications
            b.HasMany(x => x.Qualifications)
                .WithOne()
                .HasForeignKey(q => q.EmployeeId)
                .IsRequired();

            // Unique employee code per tenant
            b.HasIndex(x => new { x.TenantId, x.EmployeeCode })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            // Employee name index
            b.HasIndex(x => x.FullName);
        }
    }
}
