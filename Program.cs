using System;
using Pharmacy.Data;
using Pharmacy.Inventory;
using Pharmacy.Models;
using Pharmacy.Sales;

try
{
    Console.WriteLine("========================================");
    Console.WriteLine("    PHARMACY MANAGEMENT SYSTEM         ");
    Console.WriteLine("========================================");

    // Initialize DB and Core Services
    DatabaseManager dbManager = new DatabaseManager();
    dbManager.InitializeDatabase();

    InventoryManager inventory = new InventoryManager(dbManager);
    SalesManager sales = new SalesManager(dbManager);

    bool appRunning = true;

    while (appRunning)
    {
        try
        {
            Console.WriteLine("\n[MAIN MENU]");
            Console.WriteLine("1. Sales / POS");
            Console.WriteLine("2. Inventory");
            Console.WriteLine("3. Exit");
            Console.Write("Enter choice: ");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    RunSalesMenu(sales);
                    break;
                case "2":
                    RunInventoryMenu(inventory);
                    break;
                case "3":
                    appRunning = false;
                    Console.WriteLine("Exiting application. Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Main Menu Error] An unexpected issue occurred: {ex.Message}");
            Console.WriteLine("Returning to Main Menu...\n");
        }
    }
}
catch (Exception fatalEx)
{
    Console.WriteLine($"[CRITICAL ERROR] Application failed to start or encountered a unrecoverable error: {fatalEx.Message}");
}

// ==========================================
// SALES & POS SUB-MENU
// ==========================================
void RunSalesMenu(SalesManager sales)
{
    bool inSalesMenu = true;

    while (inSalesMenu)
    {
        try
        {
            Console.WriteLine("\n--- [SALES & POS SECTION] ---");
            Console.WriteLine("1. POS Terminal (New Checkout)");
            Console.WriteLine("2. View Sales History Report");
            Console.WriteLine("3. Return to Main Menu");
            Console.Write("Select an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RunPOSTerminal(sales);
                    break;
                case "2":
                    sales.DisplaySalesHistory();
                    break;
                case "3":
                    inSalesMenu = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Enter 1, 2, or 3.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Sales Section Error]: {ex.Message}");
        }
    }
}

void RunPOSTerminal(SalesManager sales)
{
    bool inCartSession = true;

    Console.WriteLine("\n=== POS TERMINAL STARTED ===");

    while (inCartSession)
    {
        try
        {
            Console.WriteLine("\n[POS Options]");
            Console.WriteLine("1. Scan/Add Item to Cart");
            Console.WriteLine("2. View Current Cart");
            Console.WriteLine("3. Complete Checkout & Print Receipt");
            Console.WriteLine("4. Cancel Order & Return");
            Console.Write("Choice: ");

            string? posChoice = Console.ReadLine();

            switch (posChoice)
            {
                case "1":
                    Console.Write("Enter Medicine Code: ");
                    string? code = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(code))
                    {
                        Console.WriteLine("Code cannot be empty.");
                        break;
                    }

                    Console.Write("Enter Quantity: ");
                    string? rawQty = Console.ReadLine();

                    if (!int.TryParse(rawQty, out int qty) || qty <= 0)
                    {
                        Console.WriteLine("Invalid quantity. Must be a positive integer.");
                        break;
                    }

                    // Call SalesManager to validate stock and add to active cart
                    sales.AddToCart(code, qty);
                    break;

                case "2":
                    sales.DisplayCart();
                    break;

                case "3":
                    bool success = sales.CompleteCheckout();
                    if (success)
                    {
                        inCartSession = false; // Checkout done, return to sales menu
                    }
                    break;

                case "4":
                    sales.ClearCart();
                    Console.WriteLine("Cart cleared. Exiting POS terminal.");
                    inCartSession = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
        catch (FormatException fEx)
        {
            Console.WriteLine($"[Input Format Error] Invalid data format entered: {fEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[POS Terminal Error]: {ex.Message}");
        }
    }
}

// ==========================================
// INVENTORY SUB-MENU
// ==========================================
void RunInventoryMenu(InventoryManager inventory)
{
    bool inInventoryMenu = true;

    while (inInventoryMenu)
    {
        try
        {
            Console.WriteLine("\n--- [INVENTORY MANAGEMENT] ---");
            Console.WriteLine("1. Restock / Add Medicine");
            Console.WriteLine("2. View Total Inventory");
            Console.WriteLine("3. Return to Main Menu");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    inventory.RestockMedicine();
                    break;
                case "2":
                    inventory.DisplayTotalInventory();
                    break;
                case "3":
                    inInventoryMenu = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Enter 1, 2, or 3.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Inventory Menu Error]: {ex.Message}");
        }
    }
}