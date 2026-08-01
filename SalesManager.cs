using System;
using System.Collections.Generic;
using Pharmacy.Data;
using Pharmacy.Models;

namespace Pharmacy.Sales;

public class SalesManager
{
    private readonly DatabaseManager _dbManager;
    private readonly List<SaleRecord> _currentCart = new List<SaleRecord>();
    private string _currentTransactionId = string.Empty;

    public SalesManager(DatabaseManager dbManager)
    {
        _dbManager = dbManager;
        GenerateNewTransactionId();
    }

    private void GenerateNewTransactionId()
    {
        _currentTransactionId = "TXN-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
    }

    public void AddToCart(string code, int quantity)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("[Error] Medicine code cannot be empty.");
                return;
            }

            if (quantity <= 0)
            {
                Console.WriteLine("[Error] Quantity must be greater than zero.");
                return;
            }

            Medicine? med = _dbManager.SearchMedicineByCode(code);
            if (med == null)
            {
                Console.WriteLine($"[Error] Medicine with code '{code}' not found.");
                return;
            }

            if (med.Quantity < quantity)
            {
                Console.WriteLine($"[Error] Insufficient stock! Available: {med.Quantity}, Requested: {quantity}");
                return;
            }

            double totalPrice = med.Price * quantity;

            _currentCart.Add(new SaleRecord
            {
                TransactionId = _currentTransactionId,
                MedicineCode = med.MedicineCode,
                MedicineName = med.Name,
                Quantity = quantity,
                UnitPrice = med.Price,
                TotalPrice = totalPrice,
                SaleDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            Console.WriteLine($"[Success] Added {quantity}x {med.Name} to cart.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[System Error] Failed to add item to cart: {ex.Message}");
        }
    }

    public void DisplayCart()
    {
        try
        {
            if (_currentCart.Count == 0)
            {
                Console.WriteLine("\n[Cart is currently empty.]");
                return;
            }

            Console.WriteLine($"\n--- CURRENT CART ({_currentTransactionId}) ---");
            Console.WriteLine($"{"Code",-10} {"Name",-20} {"Qty",-6} {"Price",-10} {"Total",-10}");
            Console.WriteLine(new string('-', 56));

            double grandTotal = 0;
            foreach (var item in _currentCart)
            {
                Console.WriteLine($"{item.MedicineCode,-10} {item.MedicineName,-20} {item.Quantity,-6} {item.UnitPrice,-10:F2} {item.TotalPrice,-10:F2}");
                grandTotal += item.TotalPrice;
            }
            Console.WriteLine(new string('-', 56));
            Console.WriteLine($"{"Grand Total:",-46} {grandTotal,-10:F2}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UI Error] Could not render active cart: {ex.Message}");
        }
    }

    public bool CompleteCheckout()
    {
        try
        {
            if (_currentCart.Count == 0)
            {
                Console.WriteLine("[Error] Cannot checkout an empty cart.");
                return false;
            }

            bool success = _dbManager.AddSaleRecords(_currentCart);
            if (success)
            {
                Console.WriteLine($"\n[SUCCESS] Checkout completed successfully!");
                Console.WriteLine($"Transaction ID: {_currentTransactionId}");
                DisplayCart();

                // Clear cart and reset Transaction ID for the next customer
                _currentCart.Clear();
                GenerateNewTransactionId();
                return true;
            }

            Console.WriteLine("[ERROR] Checkout failed during database execution.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Critical Checkout Failure]: {ex.Message}");
            return false;
        }
    }

    // Alias methods to preserve support for both old and new naming preferences
    public void CancelSale() => ClearCart();

    public void ClearCart()
    {
        _currentCart.Clear();
        GenerateNewTransactionId();
        Console.WriteLine("[POS] Active cart session cleared.");
    }

    public void DisplaySalesHistory()
    {
        try
        {
            List<SaleRecord> history = _dbManager.GetSaleHistory();

            if (history == null || history.Count == 0)
            {
                Console.WriteLine("\n[No sales history records found.]");
                return;
            }

            Console.WriteLine("\n================ SALES HISTORY REPORT ================");
            Console.WriteLine($"{"Txn ID",-20} {"Code",-8} {"Name",-15} {"Qty",-5} {"Total",-8} {"Date"}");
            Console.WriteLine(new string('=', 65));

            foreach (var record in history)
            {
                Console.WriteLine($"{record.TransactionId,-20} {record.MedicineCode,-8} {record.MedicineName,-15} {record.Quantity,-5} {record.TotalPrice,-8:F2} {record.SaleDate}");
            }
            Console.WriteLine("======================================================\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to render sales history: {ex.Message}");
        }
    }
}