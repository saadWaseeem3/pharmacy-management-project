using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Pharmacy.Models;

namespace Pharmacy.Data;

public class DatabaseManager
{
    private readonly string _dbFilePath;
    private readonly string _connectionString;

    public DatabaseManager(string dbFilePath = "pharmacy.db")
    {
        _dbFilePath = dbFilePath;
        _connectionString = $"Data Source={_dbFilePath}";
    }

    public void InitializeDatabase()
    {
        CreateTables();
    }

    public SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using (var pragmaCommand = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
        {
            pragmaCommand.ExecuteNonQuery();
        }

        return connection;
    }

    private bool CreateTables()
    {

        string createTableSql = @"
                CREATE TABLE IF NOT EXISTS Medicines (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MedicineCode TEXT UNIQUE NOT NULL,
                    Name TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Price REAL NOT NULL
                    
                );

                CREATE TABLE IF NOT EXISTS Sales (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceNumber TEXT NOT NULL UNIQUE,
                    SalesmanId INTEGER NOT NULL,
                    SaleDate TEXT NOT NULL,
                    GrandTotal REAL NOT NULL DEFAULT 0.0,
                    FOREIGN KEY (SalesmanId) REFERENCES Salesmen(Id)
                );
                CREATE TABLE IF NOT EXISTS SaleDetails (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SaleId INTEGER NOT NULL,
                    MedicineName TEXT NOT NULL,
                    BatchId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    UnitPrice REAL NOT NULL,
                    SubTotal REAL NOT NULL,
                    FOREIGN KEY (SaleId) REFERENCES Sales(Id),
                    FOREIGN KEY (BatchId) REFERENCES Batches(Id)
                );

                CREATE TABLE IF NOT EXISTS Salesmen(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SalesmanCode TEXT NOT NULL UNIQUE,
                    Name TEXT NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL CHECK(Role IN ('Admin', 'Salesman')),
                    IsActive INTEGER NOT NULL DEFAULT 1
                    
                );

                CREATE TABLE IF NOT EXISTS Batches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MedicineId INTEGER NOT NULL,
                    BatchNumber TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    ExpiryDate TEXT NOT NULL,

                    FOREIGN KEY (MedicineId) REFERENCES Medicines(Id) ON DELETE RESTRICT
                    );";

        try
        {

            using (var connection = GetConnection())
            using (var command = new SqliteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }





            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"[Init Error] Permission denied accessing path '{_dbFilePath}': {ex.Message}");
            return false;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"[Init Error] SQLite failure during startup (ErrorCode {ex.SqliteErrorCode}): {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Init Error] Unexpected error during database initialization: {ex.Message}");
            return false;
        }
    }

    // ==========================================
    // INVENTORY OPERATIONS
    // ==========================================

    public void AddMedicine(Medicine medicine)
    {
        string insertSql = @"
                INSERT INTO Medicines (MedicineCode, Name, Category, Price)
                VALUES (@MedicineCode, @Name, @Category, @Price);";

        try
        {

            using (var connection = GetConnection())
            using (var command = new SqliteCommand(insertSql, connection))
            {
                command.Parameters.AddWithValue("@MedicineCode", medicine.MedicineCode);
                command.Parameters.AddWithValue("@Name", medicine.Name);
                command.Parameters.AddWithValue("@Category", medicine.Category);
                command.Parameters.AddWithValue("@Price", medicine.Price);


                command.ExecuteNonQuery();
            }

        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"[Database Error]: Failed to add medicine. Details: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[System Error]: An unexpected error occurred: {ex.Message}");
        }
    }

    public void AddBatches(Batch batch)
    {

        string insertSql = @"
                INSERT INTO Batches (MedicineId, BatchNumber, Quantity, ExpiryDate)
                VALUES (@MedicineId, @BatchNumber, @Quantity, @ExpiryDate);";

        try
        {

            using (var connection = GetConnection())
            using (var command = new SqliteCommand(insertSql, connection))
            {
                command.Parameters.AddWithValue("@MedicineId", batch.MedicineId);
                command.Parameters.AddWithValue("@BatchNumber", batch.BatchNumber);
                command.Parameters.AddWithValue("@Quantity", batch.Quantity);
                command.Parameters.AddWithValue("@ExpiryDate", batch.ExpiryDate.ToString("yyyy-MM-dd"));


                command.ExecuteNonQuery();
            }

        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"[Database Error]: Failed to add batch. Details: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[System Error]: An unexpected error occurred: {ex.Message}");
        }
    }

    public List<MedicineWithBatch> GetAllMedicines()
    {
        List<MedicineWithBatch> medicineList = new List<MedicineWithBatch>();

        string selectSql = @"
        SELECT 
        m.Id, m.MedicineCode, m.Name, m.Category, m.Price,
        COALESCE(SUM(b.Quantity), 0) AS TOTAL_QUANTITY
        FROM Medicines m
        LEFT JOIN Batches b ON m.Id = b.MedicineId
        GROUP BY m.Id, m.MedicineCode, m.Name, m.Category, m.Price;";
        try
        {
            using (var connection = GetConnection())
            using (var command = new SqliteCommand(selectSql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    medicineList.Add(new MedicineWithBatch
                    {
                        MedicineId = reader.GetInt32(0),
                        MedicineCode = reader.GetString(1),
                        Name = reader.GetString(2),
                        Category = reader.GetString(3),
                        Price = reader.GetDouble(4),
                        TotalQuantity = reader.GetInt32(5)
                    });
                }
            }

        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"[Database Error]: Failed to retrieve medicines: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[System Error]: Failed to load data: {ex.Message}");
        }

        return medicineList;
    }

    public List<MedicineWithBatch> SearchMedicinesByName(string searchTerm)
    {
        List<MedicineWithBatch> results = new List<MedicineWithBatch>();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return results;

        string querySql = @"
            SELECT 
            m.Id, m.MedicineCode, m.Name, m.Category, m.Price,
            COALESCE(SUM(b.Quantity), 0) AS TOTAL_QUANTITY
            FROM Medicines m
            LEFT JOIN BATCHES b ON m.Id = b.MedicineId
            WHERE m.Name LIKE @Search OR m.Category LIKE @Search
            GROUP BY m.Id, m.MedicineCode, m.Name, m.Category, m.Price;";

        try
        {

            using (var connection = GetConnection())
            using (var command = new SqliteCommand(querySql, connection))
            {
                command.Parameters.AddWithValue("@Search", $"%{searchTerm}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new MedicineWithBatch
                        {
                            MedicineId = reader.GetInt32(0),
                            MedicineCode = reader.GetString(1),
                            Name = reader.GetString(2),
                            Category = reader.GetString(3),
                            Price = reader.GetDouble(4),
                            TotalQuantity = reader.GetInt32(5)
                        });
                    }
                }
            }

        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"[DATABASE ERROR] Search failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SYSTEM ERROR] Unexpected failure during search: {ex.Message}");
        }

        return results;
    }

    public MedicineWithBatch? SearchMedicineByCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        string querySql = @"
            SELECT
            m.Id, MedicineCode, m.Name, m.Category, m.Price,
            COALESCE(SUM(b.Quantity), 0) AS TOTAL_QUANTITY
            FROM Medicines m
            LEFT JOIN BATCHES b ON m.Id = b.MedicineId
            WHERE m.MedicineCode = @code
            GROUP BY m.Id, m.MedicineCode, m.Name, m.Category, m.Price;";

        try
        {
            using (var connection = GetConnection())
            using (var command = new SqliteCommand(querySql, connection))
            {
                command.Parameters.AddWithValue("@code", code.ToUpper());

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MedicineWithBatch
                        {
                            MedicineId = reader.GetInt32(0),
                            MedicineCode = reader.GetString(1),
                            Name = reader.GetString(2),
                            Category = reader.GetString(3),
                            Price = reader.GetDouble(4),
                            TotalQuantity = reader.GetInt32(5)

                        };
                    }
                }
            }

        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"[DATABASE ERROR] Code lookup failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SYSTEM ERROR] Unexpected failure during lookup: {ex.Message}");
        }

        return null;
    }

    // ==========================================
    // SALES & CHECKOUT OPERATIONS
    // ==========================================

    /// <summary>
    /// Executes an atomic batch transaction for cart checkout:
    /// Deducts inventory stock and logs sales history together.
    /// If any line item fails, the entire transaction rolls back.
    /// </summary>
    public bool ExecuteSale(Sale sale)
    {
        if (sale == null || sale.Items == null || sale.Items.Count == 0)
            return false;


        using (var connection = GetConnection())
        using (var transaction = connection.BeginTransaction())
        {
            try
            {

                //Insert transaction header into Sales table
                string insertSaleSql = @"
                        INSERT INTO Sales (InvoiceNumber, SalesmanId, SaleDate, GrandTotal)
                        VALUES (@InvoiceNumber, @SalesmanId, @SaleDate, @GrandTotal);
                        SELECT last_insert_rowid();";


                int generatedSaleId;
                using (var saleCmd = new SqliteCommand(insertSaleSql, connection, transaction))
                {
                    saleCmd.Parameters.AddWithValue("@InvoiceNumber", sale.InvoiceNumber);
                    saleCmd.Parameters.AddWithValue("@SalesmanId", sale.SalesmanId);
                    saleCmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                    saleCmd.Parameters.AddWithValue("@GrandTotal", sale.GrandTotal);

                    //Execute query and extract auto-generated Sales.Id
                    generatedSaleId = Convert.ToInt32(saleCmd.ExecuteScalar());
                }

                string updateStockSql = @"
                        UPDATE Batches 
                        SET Quantity = Quantity - @Quantity 
                        WHERE Id = @BatchId AND Quantity >= @Quantity;";

                string insertDetailSql = @"
                        INSERT INTO SaleDetails (SaleId, BatchId, MedicineName, Quantity, UnitPrice, SubTotal)
                        VALUES (@SaleId, @BatchId, @MedicineName, @Quantity, @UnitPrice, @SubTotal);";

                // 2. Process each line item (SaleDetail) in the transaction
                foreach (var item in sale.Items)
                {
                    using (var updateCmd = new SqliteCommand(updateStockSql, connection, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@BatchId", item.BatchId);
                        updateCmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        if(rowsAffected == 0)
                        {
                            throw new InvalidOperationException($"Stock deduction failed for {item.MedicineName} (Batch ID: {item.BatchId}). Insufficient Inventory.");
                        }



                    }

                    using (var detailCmd = new SqliteCommand(insertDetailSql, connection, transaction))
                    {
                        
                        detailCmd.Parameters.AddWithValue("@SaleId", generatedSaleId);
                        detailCmd.Parameters.AddWithValue("@BatchId", item.BatchId);
                        detailCmd.Parameters.AddWithValue("@MedicineName", item.MedicineName);
                        detailCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        detailCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                        detailCmd.Parameters.AddWithValue("@TotalPrice", item.SubTotal);
                        

                        detailCmd.ExecuteNonQuery();
                    }

                }
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"[TRANSACTION ERROR] Checkout failed and was rolled back: {ex.Message}");
                return false;
            }
        }
    }


    public List<SaleRecord> GetSaleHistory()
    {
        var history = new List<SaleRecord>();
        string querySql = @"
            SELECT
            s.Id AS SaleId,
            s.InvoiceNumber, 
            sm.SalesmanCode,
            sd.BatchId,
            b.BatchNumber,
            m.MedicineCode,
            sd.MedicineName, 
            sd.Quantity, 
            sd.UnitPrice, 
            sd.SubTotal, 
            s.SaleDate
            FROM Sales s
            INNER JOIN Salesman sm ON s.SalesmanId = sm.Id
            INNER JOIN SaleDetails sd ON s.Id = sd.SaleId
            INNER JOIN BATCHES b ON sd.BatchId = b.Id
            INNER JOIN Medicines m ON b.MedicineId = m.Id
            ORDER BY s.Id DESC;";

        try
        {
            using var connection = GetConnection();
            using var command = new SqliteCommand(querySql, connection);
            using var reader = command.ExecuteReader();

            int saleIdOrdinal = reader.GetOrdinal("SaleId");
            int invoiceNumberOrdinal = reader.GetOrdinal("InvoiceNumber");
            int salesmanCodeOrdinal = reader.GetOrdinal("SalesmanCode");
            int batchIdOrdinal = reader.GetOrdinal("BatchId");
            int batchNumberOrdinal = reader.GetOrdinal("BatchNumber");
            int medicineCodeOrdinal = reader.GetOrdinal("MedicineCode");
            int medicineNameOrdinal = reader.GetOrdinal("MedicineName");
            int quantityOrdinal = reader.GetOrdinal("Quantity");
            int unitPriceOrdinal = reader.GetOrdinal("UnitPrice");
            int subTotalOrdinal = reader.GetOrdinal("SubTotal");
            int saleDateOrdinal = reader.GetOrdinal("SaleDate");

            while (reader.Read())
            {
                history.Add(new SaleRecord
                {
                    SaleId = reader.GetInt32(saleIdOrdinal),
                    InvoiceNumber = reader.GetString(invoiceNumberOrdinal),
                    SalesmanCode = reader.GetString(salesmanCodeOrdinal),
                    BatchId = reader.GetInt32(batchIdOrdinal),
                    BatchNumber = reader.GetString(batchNumberOrdinal),
                    MedicineCode = reader.GetString(medicineCodeOrdinal),
                    MedicineName = reader.GetString(medicineNameOrdinal),
                    Quantity = reader.GetInt32(quantityOrdinal),
                    UnitPrice = reader.GetDouble(unitPriceOrdinal),
                    SubTotal = reader.GetDouble(subTotalOrdinal),
                    SaleDate = reader.GetString(saleDateOrdinal)
                });
            }


        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"[DATABASE ERROR] Failed to fetch sales history: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GENERAL ERROR] An unexpected error occurred: {ex.Message}");
        }

        return history;
    }
}