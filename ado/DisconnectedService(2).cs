using System.Data;
using System.Data.Common;

namespace ElectronicsShopApp;

public class DisconnectedService
{
    public void DemonstrateDisconnectedMode()
    {
        string selectSql = "SELECT Id, Name, CategoryId, Price, Stock FROM Products";

        using DbConnection connection = DbHelper.CreateConnection();
        DbDataAdapter adapter = DbProviderFactories
            .GetFactory("Microsoft.Data.SqlClient")
            .CreateDataAdapter();

        adapter.SelectCommand = DbHelper.CreateCommand(selectSql, connection);

        // 1. Загружаем таблицу в DataTable (в память)
        DataTable table = new DataTable();
        adapter.Fill(table);
        Console.WriteLine($"Загружено строк: {table.Rows.Count}");

        // 2. Меняем цены на 10% для категории 1 (смартфоны)
        foreach (DataRow row in table.Rows)
        {
            if ((int)row["CategoryId"] == 1)
            {
                decimal oldPrice = (decimal)row["Price"];
                row["Price"] = oldPrice * 1.10m;
            }
        }

        // 3. Добавляем новую строку
        DataRow newRow = table.NewRow();
        newRow["Name"] = "Xiaomi 14";
        newRow["CategoryId"] = 1;
        newRow["Price"] = 59999.00m;
        newRow["Stock"] = 25;
        table.Rows.Add(newRow);

        // 4. Настраиваем InsertCommand и UpdateCommand ВРУЧНУЮ
        SetupInsertCommand(adapter);
        SetupUpdateCommand(adapter);

        // 5. Сохраняем изменения в БД
        try
        {
            connection.Open();
            int rows = adapter.Update(table);
            Console.WriteLine($"Синхронизировано строк: {rows}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка синхронизации: {ex.Message}");
        }
    }

    private void SetupInsertCommand(DbDataAdapter adapter)
    {
        string sql = @"
            INSERT INTO Products (Name, CategoryId, Price, Stock)
            VALUES (@Name, @CategoryId, @Price, @Stock);
            SELECT SCOPE_IDENTITY();"; // возвращает Id только что вставленной строки

        DbCommand cmd = DbHelper.CreateCommand(sql, adapter.SelectCommand.Connection);

        AddParam(cmd, "@Name", "Name");
        AddParam(cmd, "@CategoryId", "CategoryId");
        AddParam(cmd, "@Price", "Price");
        AddParam(cmd, "@Stock", "Stock");

        adapter.InsertCommand = cmd;
    }

    private void SetupUpdateCommand(DbDataAdapter adapter)
    {
        string sql = @"
            UPDATE Products 
            SET Name = @Name, CategoryId = @CategoryId, Price = @Price, Stock = @Stock
            WHERE Id = @Id";

        DbCommand cmd = DbHelper.CreateCommand(sql, adapter.SelectCommand.Connection);

        AddParam(cmd, "@Name", "Name");
        AddParam(cmd, "@CategoryId", "CategoryId");
        AddParam(cmd, "@Price", "Price");
        AddParam(cmd, "@Stock", "Stock");

        // Для WHERE нужен оригинальный Id (SourceVersion = Original)
        DbParameter idParam = cmd.CreateParameter();
        idParam.ParameterName = "@Id";
        idParam.SourceColumn = "Id";
        idParam.SourceVersion = DataRowVersion.Original;
        cmd.Parameters.Add(idParam);

        adapter.UpdateCommand = cmd;
    }

    private void AddParam(DbCommand cmd, string paramName, string sourceColumn)
    {
        DbParameter p = cmd.CreateParameter();
        p.ParameterName = paramName;
        p.SourceColumn = sourceColumn; // DataAdapter сам возьмёт значение из строки
        p.SourceVersion = DataRowVersion.Current;
        cmd.Parameters.Add(p);
    }
}