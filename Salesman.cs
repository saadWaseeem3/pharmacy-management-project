namespace Pharmacy.Models;

public class Salesman
{
    public int Id { get; set; }
    public string SalesmanCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Salesman"; // "Admin" or "Salesman"
    public bool IsActive { get; set; } = true;
}