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
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NAME TEXT NOT NULL,
                CATEGORY TEXT NOT NULL,
                PRICE REAL NOT NULL,
                QUANTITY INTEGER NOT NULL
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
                INSERT INTO Medicines (NAME, CATEGORY, PRICE, QUANTITY)
                VALUES (@NAME, @CATEGORY, @PRICE, @QUANTITY);";

            using (var command = new SqliteCommand(insertSql, connection))
            {
                command.Parameters.AddWithValue("@NAME", medicine.Name);
                command.Parameters.AddWithValue("@CATEGORY", medicine.Category);
                command.Parameters.AddWithValue("@PRICE", medicine.Price);
                command.Parameters.AddWithValue("@QUANTITY", medicine.Quantity);

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

            string selectSql = "SELECT ID, NAME, CATEGORY, PRICE, QUANTITY FROM Medicines";

            using (var command = new SqliteCommand(selectSql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Medicine med = new Medicine
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Category = reader.GetString(2),
                            Price = reader.GetDouble(3),
                            Quantity = reader.GetInt32(4)

                        };
                        medicineList.Add(med);
                    }  
                }
            }

        }

        return medicineList;
    } 




}
    












