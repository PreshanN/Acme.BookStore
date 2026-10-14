using Acme.BookStore.Employees;
using Acme.BookStore.Shared.ValueObjects;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Acme.BookStore
{
    public class EmployeeManagementSystemApplicationAutoMapperProfile : Profile
    {
        public EmployeeManagementSystemApplicationAutoMapperProfile()
        {
            CreateMap<Employee, EmployeeDto>().ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Value)); 
            CreateMap<Address, AddressDto>(); 
       
        }
    }
}