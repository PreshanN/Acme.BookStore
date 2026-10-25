using Acme.BookStore.Departments;
using Acme.BookStore.Shared.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Acme.BookStore.EntityFrameworkCore.EntityTypeConfigurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department> 
    { 
        public void Configure(EntityTypeBuilder<Department> b) 

        {
            b.ToTable(BookStoreConsts.DbTablePrefix + "Departments", BookStoreConsts.DbSchema); 
            b.ConfigureByConvention(); b.Property(d => d.Name).IsRequired().HasMaxLength(DepartmentConsts.MaxNameLength); 
            b.Property(d => d.Description).HasMaxLength(DepartmentConsts.MaxDescriptionLength); 
            b.OwnsOne(d => d.ContactInfo, c => { c.Property(p => p.Email).HasColumnName("ContactEmail").HasMaxLength(256);
            c.Property(p => p.Phone).HasColumnName("ContactPhone").HasMaxLength(32); }); 
            b.HasMany(d => d.Teams).WithOne().HasForeignKey("DepartmentId").IsRequired();
            b.Navigation(d => d.Teams).HasField("_teams"); b.HasIndex(d => new { d.TenantId, d.Name }); 
        } 
    }
}