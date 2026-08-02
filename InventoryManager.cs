using System;
using System.Collections.Generic;
using Pharmacy.Models;
using Pharmacy.Data;
using System.Text.RegularExpressions;

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

        List<Medicine> stock = _dbManager.GetAllMedicines() ?? new List<Medicine>();

        if (stock.Count == 0)
        {
            Console.WriteLine("No medicines found in the inventory.");
            return;
        }

        foreach (var medicine in stock)
        {
            Console.WriteLine($"ID: {medicine.Id}, Medicine Code: {medicine.MedicineCode}, Name: {medicine.Name}, Category: {medicine.Category}, Price: {medicine.Price}, Quantity: {medicine.Quantity}");
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

        // 4. Isolated Quantity Parsing
        Console.Write("Quantity: ");
        string? qtyInput = Console.ReadLine();
        if (!int.TryParse(qtyInput, out int qty) || qty < 0)
        {
            Console.WriteLine("[INPUT ERROR] Quantity must be a valid positive integer. Restock cancelled.");
            return;
        }

        // 5. Instantiation (medCode, name, category are all guaranteed 'string')
        Medicine newMed = new Medicine
        {
            MedicineCode = medCode,
            Name = name,
            Category = category,
            Price = price,
            Quantity = qty
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

    public void SearchAndDisplayByName(string nameQuery)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(nameQuery))
            {
                Console.WriteLine("[INPUT ERROR] Search term cannot be empty.");
                return;
            }

            List<Medicine> matches = _dbManager.SearchMedicinesByName(nameQuery);

            if(matches == null || matches.Count == 0)
            {
                Console.WriteLine($"\n[No medicines found matching '{nameQuery}']\n");
                return;
            }

            Console.WriteLine($"\n--- SEARCH RESULTS FOR '{nameQuery}' ({matches.Count} found) ---");
            Console.WriteLine($"{"Code",-10} {"Name",-20} {"Stock",-8} {"Price ($)",-10}");
            Console.WriteLine(new string('-', 52));

            foreach(var med in matches)
            {
                Console.WriteLine($"{med.MedicineCode,-10} {med.Name,-20} {med.Quantity,-8} {med.Price,-10:F2}");
            }
            Console.WriteLine(new string('-', 52) + "\n");
            
            
        }
        catch(Exception ex)
        {
            Console.WriteLine($"[Error] Search failed: {ex.Message}");
        }
    }
}