using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Pharmacy.Models;

namespace Pharmacy.Data;
public class DatabaseManager
{
    private readonly string _connectionString = "Data Source=pharmacy.db";

    public DatabaseManager()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            string createTableSql = @"
                CREATE TABLE IF NOT EXISTS Medicines (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MedicineCode TEXT UNIQUE NOT NULL,
                Name TEXT NOT NULL,
                Category TEXT NOT NULL,
                Price REAL NOT NULL,
                Quantity INTEGER NOT NULL
            );";

            using (var command = new SqliteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }
            
        }
    }

    public void AddMedicine(Medicine medicine)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            string insertSql = @"
                INSERT INTO Medicines (MedicineCode, Name, Category, Price, Quantity)
                VALUES (@MedicineCode, @Name, @Category, @Price, @Quantity);";

            using (var command = new SqliteCommand(insertSql, connection))
            {
                command.Parameters.AddWithValue("@MedicineCode", medicine.MedicineCode);
                command.Parameters.AddWithValue("@Name", medicine.Name);
                command.Parameters.AddWithValue("@Category", medicine.Category);
                command.Parameters.AddWithValue("@Price", medicine.Price);
                command.Parameters.AddWithValue("@Quantity", medicine.Quantity);

                command.ExecuteNonQuery();
            }
        }
    }
    public List<Medicine> GetAllMedicines()
    {
        List<Medicine> medicineList = new List<Medicine>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            string selectSql = "SELECT Id,MedicineCode, Name, Category, Price, Quantity FROM Medicines";

            using (var command = new SqliteCommand(selectSql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Medicine med = new Medicine
                        {
                            Id = reader.GetInt32(0),
                            MedicineCode = reader.GetString(1),
                            Name = reader.GetString(2),
                            Category = reader.GetString(3),
                            Price = reader.GetDouble(4),
                            Quantity = reader.GetInt32(5)

                        };
                        medicineList.Add(med);
                    }  
                }
            }

        }

        return medicineList;
    }

    public List<Medicine> SearchMedicinesByName(string searchTerm)
    {
        List<Medicine> results = new List<Medicine>();
        if(string.IsNullOrWhiteSpace(searchTerm))
        return results;

        string querySql = @"
            SELECT Id, MedicineCode, Name, Category, Price, Quantity
            FROM Medicines
            WHERE Name LIKE @Search OR Category LIKE @Search;";
        
        try
        {
            using(var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using(var command = new SqliteCommand(querySql, connection))
                {
                    command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new Medicine
                            {
                                Id = reader.GetInt32(0),
                                MedicineCode = reader.GetString(1),
                                Name = reader.GetString(2),
                                Category = reader.GetString(3),
                                Price = reader.GetDouble(4),
                                Quantity = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
        }
        catch(SqliteException ex)
        {
            Console.WriteLine($"[DATABASE ERROR] Search failed: {ex.Message}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"[SYSTEM ERROR] Unexpected failure during search: {ex.Message}");
        }
        return results;
                        
    }      

    public Medicine SearchMedicineByCode(string code)
    {
        Medicine results = new Medicine();
        if(string.IsNullOrWhiteSpace(code))
        return results;

        string querySql = @"
            SELECT Id, MedicineCode, Name, Category, Price, Quantity
            FROM Medicines
            WHERE UPPER(MedicineCode) = @code;";
        
        try
        {
            using(var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using(var command = new SqliteCommand(querySql, connection))
                {
                    command.Parameters.AddWithValue("@code", code.ToUpper());

                    using(var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Medicine
                            {
                                Id = reader.GetInt32(0),
                                MedicineCode = reader.GetString(1),
                                Name = reader.GetString(2),
                                Category = reader.GetString(3),
                                Price = reader.GetDouble(4),
                                Quantity = reader.GetInt32(5)
                            };
                        }
                    }
                }
            }
        }
        catch(SqliteException ex)
        {
            Console.WriteLine($"[DATABASE ERROR] Code lookup failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SYSTEM ERROR] Unexpected failure during lookup: {ex.Message}");
        }

        return null;
    }        


        


}
    












