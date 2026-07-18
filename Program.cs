using System;


bool appRunning = true;

do
{
    
    Console.WriteLine($"1. Sales\n2. Inventory\n3. Exit");
    Console.WriteLine("Enter a choice: ");
    int choice = Convert.ToInt32(Console.ReadLine());

    switch (choice)
    {
        case 1:
            Console.WriteLine("You Are in the Sales Section which is currently under development!");
            Console.WriteLine("This section will contain 2 sub sections as per the current plan");
            Console.WriteLine("1. Sales Entry\n2. Sales Report\n");
            break;
        case 2:
            Console.WriteLine("You Are in the Inventory Section which is under developlment!");
            Console.WriteLine("This section will contain 3 sub sections as per the current plan");
            Console.WriteLine("1. Inventory restock\n2. Inventory Refill Check And orders\n3. Total Inventory Report\n");
            break;
        case 3:
           
            appRunning = false;
            break;
        
        default:
            Console.WriteLine("You have selected an invalid option");
            break;
    }

}while (appRunning);


