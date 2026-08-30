using System;
using System.Collections.Generic;
using Pharmacy.Models;
using Pharmacy.Data;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Pharmacy.Inventory;

public class InventoryManager
{
    private readonly DatabaseManager _dbManager;

    public InventoryManager(DatabaseManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void DisplayTotalInventory()
    {
        Console.WriteLine("----Total Inventory Report----");

        List<MedicineWithBatch> stock = _dbManager.GetAllMedicines() ?? new List<MedicineWithBatch>();

        if (stock.Count == 0)
        {
            Console.WriteLine("No medicines found in the inventory.");
            return;
        }

        foreach (var medicine in stock)
        {
            Console.WriteLine($"ID: {medicine.MedicineId}, Medicine Code: {medicine.MedicineCode}, Name: {medicine.Name}, Category: {medicine.Category}, Quantity: {medicine.TotalQuantity}, Price: {medicine.Price}");
        }
        Console.WriteLine("-----------------------------");
    }

    public void RestockMedicine()
    {
        Console.WriteLine("Enter the medicine details:");

        Console.Write("Name: ");
        string? rawName = Console.ReadLine();

        Console.Write("Category: ");
        string? rawCategory = Console.ReadLine();

        // 1. Guard Clause: Input validation
        if (string.IsNullOrWhiteSpace(rawName) || string.IsNullOrWhiteSpace(rawCategory))
        {
            Console.WriteLine("[INPUT ERROR] Name and Category cannot be empty. Restock cancelled.");
            return;
        }

        string name = rawName.Trim();
        string category = rawCategory.Trim();

        // 2. Pattern Matching: Unwraps string? into non-nullable 'string medCode'
        if (GenerateMedCode(name, category) is not string medCode)
        {
            Console.WriteLine("[RESTOCK CANCELLED] Failed to generate valid medicine code.");
            return;
        }

        // 3. Isolated Price Parsing
        Console.Write("Price: ");
        string? priceInput = Console.ReadLine();
        if (!double.TryParse(priceInput, out double price) || price < 0)
        {
            Console.WriteLine("[INPUT ERROR] Price must be a valid positive number. Restock cancelled.");
            return;
        }



        // 5. Instantiation (medCode, name, category are all guaranteed 'string')
        Medicine newMed = new Medicine
        {
            MedicineCode = medCode,
            Name = name,
            Category = category,
            Price = price

        };

        _dbManager.AddMedicine(newMed);
        Console.WriteLine("Medicine added successfully.");
    }

    public string? GenerateMedCode(string name, string category)
    {
        if (name.Length < 3 || category.Length < 3)
        {
            Console.WriteLine("[INPUT ERROR] Name and Category must be at least 3 characters long.");
            return null;
        }

        string categoryPart = category.Substring(0, 3);
        string namePart = name.Substring(0, 3);

        return $"{categoryPart}-{namePart}".ToUpper();
    }

    public void AddBatch()
    {
        Console.WriteLine("Enter the Batch details:");
        //Some conditions might be required to protect the ID
        Console.Write("Medicine ID: ");
        string? rawId = Console.ReadLine();

        if (!int.TryParse(rawId, out int id) || id <= 0)
        {
            Console.WriteLine("[INPUT ERROR] Medicine ID must be a positive number.");
            return;
        }

        Console.Write("Batch Number: ");
        string? batchNumber = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(batchNumber))
        {
            Console.WriteLine("[INPUT ERROR] Batch number cannot be empty.");
            return;
        }

        Console.Write("Quantity: ");
        string? rawQuantity = Console.ReadLine();
        if (!int.TryParse(rawQuantity, out int quantity) || quantity <= 0)
        {
            Console.WriteLine("[INPUT ERROR] Quantity must be a valid positive number.");
            return;
        }

        Console.Write("Expiry Date (YYYY-MM-DD): ");
        string? rawExpiry = Console.ReadLine();

        // 1. Force exact YYYY-MM-DD format
        if (!DateTime.TryParseExact(rawExpiry, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expiryDate))
        {
            Console.WriteLine("[INPUT ERROR] Expiry date must be in exact YYYY-MM-DD format (e.g., 2028-06-30).");
            return;
        }

        // 2. Reject dates that are today or in the past
        if (expiryDate <= DateTime.Today)
        {
            Console.WriteLine("[INPUT ERROR] Expiry date must be in the future.");
            return;
        }

        Batch newBatch = new Batch
        {
            MedicineId = id,
            BatchNumber = batchNumber,
            Quantity = quantity,
            ExpiryDate = expiryDate

        };

        _dbManager.AddBatches(newBatch);
        Console.WriteLine("Batch added successfully.");

    }

    public void AddCompany()
    {
        Console.WriteLine("Enter the company details:");

        Console.Write("Name: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("[INPUT ERROR] Name cannot be empty.");
            return;
        }

       
        Console.WriteLine("Enter Contact (Press Enter If Null): ");
        string? contact = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(contact))
        {
            contact = "None";
        }


        Company newCompany = new Company
        {
            Name = name,
            Contact = contact
        };

        _dbManager.AddCompanies(newCompany);
        Console.WriteLine("Company added successfully.");
    }

    public void SearchAndDisplayByName(string nameQuery)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nameQuery))
            {
                Console.WriteLine("[INPUT ERROR] Search term cannot be empty.");
                return;
            }

            List<MedicineWithBatch> matches = _dbManager.SearchMedicinesByName(nameQuery);

            if (matches == null || matches.Count == 0)
            {
                Console.WriteLine($"\n[No medicines found matching '{nameQuery}']\n");
                return;
            }

            
            

            Console.WriteLine($"\n--- SEARCH RESULTS FOR '{nameQuery}' ({matches.Count} found) ---");
            Console.WriteLine($"{"Code",-10} {"Name",-20} {"Stock",-8} {"Price ($)",-10}");
            Console.WriteLine(new string('-', 52));

            foreach (var med in matches)
            {
                Console.WriteLine($"{med.MedicineCode,-10} {med.Name,-20} {med.TotalQuantity,-8} {med.Price,-10:F2}");
            }
            Console.WriteLine(new string('-', 52) + "\n");


        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Search failed: {ex.Message}");
        }
    }

    public void ViewReorderList()
    {
        try
        {
            List<ReorderDTO> reorderList = _dbManager.GetReorderList() ?? new List<ReorderDTO>();

            if (reorderList.Count == 0)
            {
                Console.WriteLine("Reorder list is empty.");
                return;
            }

            Console.WriteLine("\n--- REORDER LIST ---");
            Console.WriteLine($"{"Code",-10} {"Name",-20} {"Company Name",-20} {"Stock",-8} {"Reorder Level (-)",-10}");
            Console.WriteLine(new string('-', 52));

            foreach (var med in reorderList)
            {
                Console.WriteLine($"{med.MedicineCode,-10} {med.MedicineName,-20} {med.CompanyName,-20} {med.CurrentStock,-8} {med.MinReorderLevel,-10}");
            }
            Console.WriteLine(new string('-', 52) + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to retrieve reorder list: {ex.Message}");
        }
    }
}