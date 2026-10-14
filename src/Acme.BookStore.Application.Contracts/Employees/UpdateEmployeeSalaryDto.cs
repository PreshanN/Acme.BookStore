namespace Acme.BookStore.Employees;

public class UpdateEmployeeSalaryDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
}
