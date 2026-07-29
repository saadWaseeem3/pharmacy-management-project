using System;
using Pharmacy.Data;
using Pharmacy.Sales;
using Pharmacy.Inventory;
using Pharmacy.Models;

Console.WriteLine("--Starting Pharmacy Applictaion--");
DatabaseManager dbManager = new DatabaseManager();
dbManager.InitializeDatabase();

InventoryManager inventory = new InventoryManager(dbManager);

SalesManager sales = new SalesManager(dbManager);

bool appRunning = true;
bool invalidSalesChoice = false;
bool invalidInventoryChoice = false;

do
{
    Console.WriteLine("1. Sales\n2. Inventory\n3. Exit");
    Console.Write("Enter a choice: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Invalid input.");
        continue;
    }



    switch (choice)
    {
        case 1:
            {

                do
                {
                    SalesChoice();
                } while (invalidSalesChoice);
                break;
            }

        case 2:
            do
            {
                InventoryChoice();
            } while (invalidInventoryChoice);
            break;

        case 3:

            appRunning = false;
            break;

        default:
            Console.WriteLine("You have selected an invalid option");
            break;
    }

} while (appRunning);




void SalesChoice()
{
    Console.WriteLine("You Are in the Sales Section which is currently under development!");
    Console.WriteLine("This section will contain 2 sub sections as per the current plan");
    Console.WriteLine("1. Sales Entry\n2. Sales Report\n3. Exit Sales");

    Console.WriteLine("Enter a choice: ");

    if (!int.TryParse(Console.ReadLine(), out int salesubChoice))
    {
        Console.WriteLine("Invalid input.");
        invalidSalesChoice = true;
        return;
    }
    switch (salesubChoice)
    {
        case 1:
            Console.WriteLine("Enter the Medicine Code: ");
            string? code = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("Medicine code cannot be empty.");
                return;
            }
            sales.SearchByCode(code);
            Console.WriteLine("Enter the Quantity: ");
            int quantity = Convert.ToInt32(Console.ReadLine());
            sales.ShowReceipt(quantity);
            break;
        case 2:
            sales.DisplaySalesHistory();
            break;
        case 3:
            invalidSalesChoice = false;
            break;
        default:
            Console.WriteLine("You have selected an invalid option");
            break;


    }
}

void InventoryChoice()
{
    Console.WriteLine("\n[Inventory Management]\n1. Inventory Restock\n2. Total Inventory Report\n3. Exit Inventory\n");
    Console.Write("Choose an option: ");

    if (!int.TryParse(Console.ReadLine(), out int subChoice))
    {
        Console.WriteLine("Invalid input.");
        invalidInventoryChoice = true;
        return;
    }

    switch (subChoice)
    {

        case 1:
            inventory.RestockMedicine(); // Triggers the add/edit function
            break;
        case 2:


            inventory.DisplayTotalInventory(); // Triggers the view function
            break;
        case 3:
            invalidInventoryChoice = false;
            break;
        default:
            Console.WriteLine("You have selected an invalid option");
            break;
    }
}


