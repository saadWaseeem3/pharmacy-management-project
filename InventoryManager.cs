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
        

        List<MedicineWithBatch> stock = _dbManager.GetAllMedicines() ?? new List<MedicineWithBatch>();

        if (stock.Count == 0)
        {
            Console.WriteLine("No medicines found in the inventory.");
            return;
        }

        Console.WriteLine("\n");
        Console.WriteLine(new string('=',20) + " Total Inventory " + new string('=',21));
        Console.WriteLine($"{"ID",-5} {"Medicine Code",-20} {"Name",-20} {"Category",-20} {"Quantity",-10} {"Price"}");
        Console.WriteLine(new string('=', 85));

        foreach (var medicine in stock)
        {
            Console.WriteLine($"{medicine.MedicineId,-5} {medicine.MedicineCode,-20} {medicine.Name,-20} {medicine.Category,-20} {medicine.TotalQuantity,-10} {medicine.Price}");
        }
        Console.WriteLine(new string('=', 85) + "\n");
    }

    public void DisplayAllCompanies()
    {
        
        List<Company> companies = _dbManager.GetAllCompanies() ?? new List<Company>();

        if (companies.Count == 0)
        {
            Console.WriteLine("No medicines found in the inventory.");
            return;
        }

        Console.WriteLine("\n");
        Console.WriteLine(new string('=',20) + " COMPANIES " + new string('=',21));
        Console.WriteLine($"{"ID",-10} {"Name",-20} {"Contact"}");
        Console.WriteLine(new string('=', 52));

        foreach (var company in companies)
        {
            Console.WriteLine($"{company.Id,-10} {company.Name,-20} {company.Contact}");
        }
        Console.WriteLine(new string('=', 52) + "\n");
    }

    public void DisplayAllBatches()
    {
        List<Batch> batches = _dbManager.ViewAllBatches() ?? new List<Batch>();

        if (batches.Count == 0)
        {
            Console.WriteLine("No medicines found in the inventory.");
            return;
        }

        Console.WriteLine("\n");
        Console.WriteLine(new string('=', 21) + " BATCHES " + new string('=', 21));
        Console.WriteLine($"{"ID",-5} {"MedicineID",-15} {"Batch Number",-20} {"Quantity",-10} {"Expiry Date"}");
        Console.WriteLine(new string('=', 75));

        foreach (var batch in batches)
        {
            Console.WriteLine($"{batch.Id,-5} {batch.MedicineId,-15} {batch.BatchNumber,-20} {batch.Quantity,-10} {batch.ExpiryDate}");
        }
        Console.WriteLine(new string('=', 75) + "\n");
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
        
        // 4. Comapny ID
        Console.WriteLine("Enter CompanyID: ");
        string? rawCompanyId = Console.ReadLine();
        if (!int.TryParse(rawCompanyId, out int companyId) || companyId < 0)
        {
            Console.WriteLine("[INPUT ERROR] Company ID must be a valid positive number. Restock cancelled.");
            return;
        }
        
        // 5. Minimun Reorder Level
        Console.WriteLine("Enter Minimun Reorder Level:");
        string ? rawMinReorderLevel = Console.ReadLine();
        if (!int.TryParse(rawMinReorderLevel, out int minReorderLevel) || minReorderLevel < 0)
        {
            Console.WriteLine("[INPUT ERROR] Minimum Reorder Level must be a valid positive number. Restock cancelled.");
            return;
        }

        // 6. Reorder Quantity
        Console.WriteLine("Enter Reorder Quantity:");
        string? rawReorderQuantity = Console.ReadLine();
        if(!int.TryParse(rawReorderQuantity, out int ReorderQuantity) || ReorderQuantity < 0)
        {
            Console.WriteLine("[INPUT ERROR] Minimum Reorder Quantity must be a valid positive number. Restock cancelled.");
            return;
        }



        // 7. Instantiation (medCode, name, category are all guaranteed 'string')
        Medicine newMed = new Medicine
        {
            MedicineCode = medCode,
            Name = name,
            Category = category,
            Price = price,
            CompanyId = companyId,
            MinReorderLevel = minReorderLevel,
            ReorderQuantity = ReorderQuantity


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

            Console.WriteLine("\n");
            Console.WriteLine(new string('=', 21) + " REORDER LIST " + new string('=', 21));
            Console.WriteLine($"{"Code",-10} {"Name",-20} {"Company Name",-20} {"Stock",-8} {"Reorder Quantity (-)",-10}");
            Console.WriteLine(new string('=', 85));

            foreach (var med in reorderList)
            {
                Console.WriteLine($"{med.MedicineCode,-10} {med.MedicineName,-20} {med.CompanyName,-20} {med.CurrentStock,-8} {med.MinReorderLevel,-10}");
            }
            Console.WriteLine(new string('=', 85) + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to retrieve reorder list: {ex.Message}");
        }
    }
}