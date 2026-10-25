using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.BookStore.Employees
{
    public static class EmployeeConsts
    {
        public const int MaxEmployeeCodeLength = 20;
        public const int MaxFullNameLength = 128;
        public const int MaxEmailLength = 256;
        public const int CurrencyLength = 3;

        /// <summary>
        /// Length of the probation period that starts on the date of joining.
        /// </summary>
        public const int ProbationMonths = 6;
    }
}
