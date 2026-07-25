using System;
using System.Collections.Generic;
using Pharmacy.Models;
using Pharmacy.Data;

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

        List<Medicine> Stock = _dbManager.GetAllMedicines();

        if (Stock.Count == 0)
        {
            Console.WriteLine("No medicines found in the inventory.");
            return;
        }

        foreach (var medicine in Stock)
        {
            Console.WriteLine($"ID: {medicine.Id},Medicine Code: {medicine.MedicineCode}, Name: {medicine.Name}, Category: {medicine.Category}, Price: {medicine.Price}, Quantity: {medicine.Quantity}");
        }
        Console.WriteLine("-----------------------------");
    }



    public void RestockMedicine()
    {
        Console.WriteLine("Enter the medicine details:");

        Console.Write("Name: ");
        string? name = Console.ReadLine();

        Console.Write("Category: ");
        string? category = Console.ReadLine();

        if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("[INPUT ERROR] Name and Category cannot be empty. Restock cancelled.");
            return;
        }

        // 1. Validate inputs and generate code upfront
        string? medCode = GenerateMedCode(name, category);
        if (medCode == null)
        {
            Console.WriteLine("[RESTOCK CANCELLED] Failed to generate valid medicine code.");
            return;
        }

        // 2. Safe Parsing for Price
        Console.Write("Price: ");
        if (!double.TryParse(Console.ReadLine(), out double price) || price < 0)
        {
            Console.WriteLine("[INPUT ERROR] Price must be a valid positive number. Restock cancelled.");
            return;
        }

        // 3. Safe Parsing for Quantity
        Console.Write("Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 0)
        {
            Console.WriteLine("[INPUT ERROR] Quantity must be a valid positive integer. Restock cancelled.");
            return;
        }

        // 4. Create and persist valid object (Name and Category guaranteed non-null here)
        Medicine newMed = new Medicine
        {
            MedicineCode = medCode,
            Name = name!.Trim(),
            Category = category!.Trim(),
            Price = price,
            Quantity = qty
        };

        _dbManager.AddMedicine(newMed);

        Console.WriteLine("Medicine added successfully.");
    }
    public string? GenerateMedCode(string? name, string? category)
    {
        // 1. Guard Clause: Check for null, empty, or whitespace-only inputs
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("[INPUT ERROR] Cannot generate code: Name and Category are required.");
            return null;
        }

        string cleanName = name.Trim();
        string cleanCategory = category.Trim();

        // 2. Guard Clause: Ensure inputs have at least 3 characters
        if (cleanName.Length < 3 || cleanCategory.Length < 3)
        {
            Console.WriteLine("[INPUT ERROR] Name and Category must be at least 3 characters long.");
            return null;
        }

        // 3. Safe string extraction (Guaranteed not to throw exceptions)
        string categoryPart = cleanCategory.Substring(0, 3);
        string namePart = cleanName.Substring(0, 3);

        return $"{categoryPart}-{namePart}".ToUpper();
    }
















}