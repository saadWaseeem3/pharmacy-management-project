using System;
using System.Collections.Generic;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.Inventory;

namespace Pharmacy.Sales;
public class SalesManager
{
    private DatabaseManager _dbManager;
    public SalesManager(DatabaseManager dbManager)
    {
        _dbManager = dbManager;

        
    }

    Medicine? medicine;

    

    public void SearchByCode(string? code)
    {
        Console.WriteLine("Search Results:");
        medicine = _dbManager.SearchMedicineByCode(code);
        if (medicine == null)
        {
            Console.WriteLine("Medicine not found.");
        }
        else
        Console.WriteLine($"ID: {medicine.Id}, Medicine Code: {medicine.MedicineCode}, Name: {medicine.Name}, Category: {medicine.Category}, Price: {medicine.Price}, Quantity: {medicine.Quantity}");
    }

    private double CalculateBill(int quantity)
    {
        double totalBill = medicine.Price * quantity;
        return totalBill;
    }
    public void ShowReceipt(int quantity)
    {
        Console.WriteLine("----Receipt----");
        Console.WriteLine($"\nID: {medicine.Id}\n Name: {medicine.Name}\n Category: {medicine.Category}\n Price: {medicine.Price}\n");
        Console.WriteLine($"Total Bill: {CalculateBill(quantity)}");
    }

























}