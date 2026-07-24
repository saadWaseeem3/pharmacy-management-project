using System;
using System.Collections.Generic;
using Pharmacy.Models;
using Pharmacy.Data;

namespace Pharmacy.Inventory;
public class InventoryManager
{
    private readonly DatabaseManager db = new DatabaseManager();
    
    

    public void DisplayTotalInventory()
    {
        Console.WriteLine("----Total Inventory Report----");

        List<Medicine> Stock = db.GetAllMedicines();

        if(Stock.Count == 0)
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
        Medicine medicine = new Medicine();
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Category: ");
        string category = Console.ReadLine();
        Console.Write("Price: ");
        double price = Convert.ToDouble(Console.ReadLine());
        Console.Write("Quantity: ");
        int qty = Convert.ToInt32(Console.ReadLine());

        string medCode = GenerateMedCode(name, category);

        Medicine newMed = new Medicine
        {
            MedicineCode = medCode,
            Name = name,
            Category = category,
            Price = price,
            Quantity = qty
        };

        db.AddMedicine(newMed);
        
        Console.WriteLine("Medicine added successfully.");
    }

    public string GenerateMedCode(string name, string category)
    {
        string medCode = (category.Substring(0, 3) +"-"+ name.Substring(0, 3)).ToUpper();  
        return medCode;
     }
        















}