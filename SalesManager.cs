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


    Medicine? medicine;  //Medicine(object) that is being purchased.
    int saledquantity = 0;
    
    

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
        
        saledquantity = quantity;
        Console.WriteLine("----Receipt----");
        Console.WriteLine($"ID: {medicine.Id}\n Name: {medicine.Name}\n Category: {medicine.Category}\n Price: {medicine.Price}\n");
        Console.WriteLine($"Total Bill: {CalculateBill(quantity)}");
        AddToSalesRecord();
    }

    public void AddToSalesRecord()
    {
        SaleRecord newSaleRecord = new SaleRecord
        {
            TransactionId = $"TXN-{DateTime.Now:yyyyMMdd-HHmmss}",
            MedicineCode = medicine.MedicineCode,
            MedicineName = medicine.Name,
            Quantity = saledquantity,
            UnitPrice = medicine.Price,
            TotalPrice = CalculateBill(saledquantity),
            SaleDate = DateTime.Now.ToString("yyyy-MM-dd")
            
        };

        _dbManager.AddSaleRecord(newSaleRecord);
    }

    public void DisplaySalesHistory()
    {
        Console.WriteLine("-----Sales History-----");

        List<SaleRecord> history = _dbManager.GetSaleHistory();

        if(history.Count == 0)
        {
            Console.WriteLine($"No sales records found.");
            return;
        }

        foreach(var record in history)
        {
            Console.WriteLine($"Transaction ID: {record.TransactionId}");
            Console.WriteLine("-----------------------");
            Console.WriteLine($"ID {record.Id} Medicine Code: {record.MedicineCode} Name: {record.MedicineName} Quantity: {record.Quantity} Price: {record.UnitPrice} Total Price: {record.TotalPrice} Sale Date: {record.SaleDate}");
            Console.WriteLine("===========================================================");

        }
        Console.WriteLine("--------------End--------------");

    }























}