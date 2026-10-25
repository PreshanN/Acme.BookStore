using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.BookStore.Departments
{
    public class CreateDepartmentWithTeamsDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> TeamNames { get; set; } = new();
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
    }
}
