namespace Pharmacy.Staff;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Pharmacy.Data;
using Pharmacy.Models;

public class StaffManager
{
    private readonly DatabaseManager _databaseManager;

    public StaffManager(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public void RegisterNewStaff()
    {
        Console.WriteLine("\n------Register New Staff Member------");

        Console.Write("Enter Staff Code: ");
        string staffCode = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Staff Name: ");
        string? staffName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter Staff Password: ");
        string? staffPassword = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Staff Role: ");
        string? staffRole = Console.ReadLine() ?? "1";
        string role = staffRole == "2" ? "Admin" : "Salesman";

        if (string.IsNullOrEmpty(staffCode) || string.IsNullOrEmpty(staffName))
        {
            Console.WriteLine("[INPUT ERROR] Staff Code and Name cannot be empty.");
            return;
        }

        var newSalesman = new Salesman
        {
            SalesmanCode = staffCode,
            Name = staffName,
            PasswordHash = staffPassword,
            Role = role,
            IsActive = true
        };

        _databaseManager.AddSalesman(newSalesman);
        Console.WriteLine($"\n[Success] Staff member '{staffName}' registered successfully under role '{role}'.");
    }

    public void DisplayAllStaff()
    {
        var staffList = _databaseManager.GetSalesmanList();

        Console.WriteLine("\n");
        Console.WriteLine(new string('=', 15) + " Staff List " + new string('=', 15));
        Console.WriteLine($"{"ID",-4} {"CODE",-8} {"NAME",-18} {"ROLE"}");
        Console.WriteLine(new string('=', 42));
        foreach(var s in staffList)
        {
            Console.WriteLine($"{s.Id,-4} {s.SalesmanCode,-8} {s.Name,-18} {s.Role,-8}");
        }
        Console.WriteLine(new string('=', 42) + "\n");
    }
}