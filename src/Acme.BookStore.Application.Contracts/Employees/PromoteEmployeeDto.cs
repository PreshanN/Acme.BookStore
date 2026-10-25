using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.BookStore.Employees
{
    public class PromoteEmployeeDto
    {
        public Guid NewDepartmentId { get; set; }
        public decimal NewSalaryAmount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}
