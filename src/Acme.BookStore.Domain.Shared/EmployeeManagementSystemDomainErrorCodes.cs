using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.BookStore
{
    public static class EmployeeManagementSystemDomainErrorCodes
    {
        public const string EmployeeCodeAlreadyExists =
            "BookStore:EmployeeCodeAlreadyExists";
        public const string DepartmentNotFound =
            "BookStore:DepartmentNotFound";
        public const string InvalidDateOfJoining =
            "BookStore:InvalidDateOfJoining";
        public const string EmployeeAlreadyConfirmed =
            "BookStore:EmployeeAlreadyConfirmed";
        public const string ProbationNotCompleted =
            "BookStore:ProbationNotCompleted";
    }
}
