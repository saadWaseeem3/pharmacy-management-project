using System;
using Pharmacy.Data;
using Pharmacy.Sales;
using Pharmacy.Inventory;
using Pharmacy.Models;

Console.WriteLine("--Starting Pharmacy Applictaion--");

InventoryManager inventory = new InventoryManager();

bool appRunning = true;

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
            Console.WriteLine("You Are in the Sales Section which is currently under development!");
            Console.WriteLine("This section will contain 2 sub sections as per the current plan");
            Console.WriteLine("1. Sales Entry\n2. Sales Report\n");
            break;
        case 2:
            Console.WriteLine("\n[Inventory Management]\n1. Inventory Restock\n2. Total Inventory Report");
            Console.Write("Choose an option: ");
            string subChoice = Console.ReadLine();

            if (subChoice == "1")
            {
                inventory.RestockMedicine(); // Triggers the add/edit function
            }
            else if (subChoice == "2")
            {
                inventory.DisplayTotalInventory(); // Triggers the view function
            }
            break;
            
        case 3:
           
            appRunning = false;
            break;
        
        default:
            Console.WriteLine("You have selected an invalid option");
            break;
    }

}while (appRunning);




