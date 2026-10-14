
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ADO_Lab3
{
    class Program
    {
        static string cs =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();            
                Console.WriteLine(" - _ - _ - Список поставщиков данных _ - _ - _");
                Console.WriteLine(" ЗАПРОС");
                Console.WriteLine(" Сортировка");
                Console.WriteLine(" Успешная");
                Console.WriteLine(" Откат");
                Console.WriteLine("Выход");
                Console.Write("\n> ");

                string choice = Console.ReadLine();
                Console.WriteLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            ShowProviders();
                            break;
                        case "2":
                            RunQueryViaFactory();
                            break;
                        case "3":
                            DemoDataViewManager();
                            break;
                        case "4":
                            SuccessfulTransaction();
                            break;
                        case "5":
                            FailedTransaction();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Нет такого пункта.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ОШИБКА: " + ex.Message);
                }

                Console.WriteLine("\nНажмите Enter...");
                Console.ReadLine();
            }
        }
        static void ShowProviders()
        {
            DataTable providers = DbProviderFactories.GetFactoryClasses();

            Console.WriteLine("=== Зарегистрированные поставщики ===\n");
            Console.WriteLine("{0,-30} | {1}", "InvariantName", "Description");
            Console.WriteLine(new string('-', 85));

            foreach (DataRow row in providers.Rows)
            {
                Console.WriteLine(
                    "{0,-30} | {1}",
                    row["InvariantName"],
                    row["Description"]);
            }

            Console.WriteLine("\nВсего найдено: " + providers.Rows.Count);
        }
        static void RunQueryViaFactory()
        {
            Console.WriteLine("Выберите поставщика:");
            Console.WriteLine("System.Data.SqlClient");
            Console.WriteLine("System.Data.OleDb");
            Console.Write("> ");

            string providerName = (Console.ReadLine() ?? "").Trim();

            DbProviderFactory factory;

            try
            {
                factory = DbProviderFactories.GetFactory(providerName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Поставщик не найден: " + ex.Message);
                return;
            }

            string connStr = GetConnectionStringByProvider(providerName);

            if (string.IsNullOrEmpty(connStr))
            {
                Console.WriteLine(
                    "Не найдена");
                return;
            }

            using (DbConnection conn = factory.CreateConnection())
            {
                if (conn == null)
                {
                    Console.WriteLine("Не получилось");
                    return;
                }

                conn.ConnectionString = connStr;

                using (DbCommand cmd = factory.CreateCommand())
                {
                    if (cmd == null)
                    {
                        Console.WriteLine("Не удалось создать команду.");
                        return;
                    }

                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Books";

                    using (DbDataAdapter adapter = factory.CreateDataAdapter())
                    {
                        if (adapter == null)
                        {
                            Console.WriteLine(
                                "Не создалось");
                            return;
                        }
                        adapter.SelectCommand = cmd;

                        DataTable dt = new DataTable();

                        try
                        {
                            conn.Open();
                            adapter.Fill(dt);

                            Console.WriteLine(
                                "\n=== Результат SELECT * FROM Books ===\n");

                            PrintDataTable(dt);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ошибка: " + ex.Message);
                        }
                    }
                }
            }
        }

        static string GetConnectionStringByProvider(string providerName)
        {
            foreach (ConnectionStringSettings s
                in ConfigurationManager.ConnectionStrings)
            {
                if (s.ProviderName == providerName)
                    return s.ConnectionString;
            }

            return null;
        }
        static void DemoDataViewManager()
        {
            DataSet set = new DataSet();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(
                    "SELECT * FROM Authors", conn);

                adapter.Fill(set, "Authors");
            }

            Console.WriteLine("=== Исходные данные (без фильтра) ===\n");
            PrintDataTable(set.Tables["Authors"]);

            DataViewManager dvm = new DataViewManager(set);

            dvm.DataViewSettings["Authors"].RowFilter = "Id < 100";
            dvm.DataViewSettings["Authors"].Sort = "LastName ASC";

            DataView dv = dvm.CreateDataView(set.Tables["Authors"]);

            Console.WriteLine(
                "\n=== После фильтра Id < 100 и сортировки LastName ASC ===\n");

            PrintDataView(dv);

            Console.WriteLine(
                "\n=== Исходные данные после фильтрации ===\n");

            PrintDataTable(set.Tables["Authors"]);
        }
        static void SuccessfulTransaction()
        {
            DropTmp3IfExists();

            bool committed = false;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlTransaction tran = null;

                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();

                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        comm.Transaction = tran;

                        comm.CommandText =
                            "CREATE TABLE dbo.tmp3 (" +
                            "id INT IDENTITY PRIMARY KEY, " +
                            "f1 VARCHAR(20), f2 INT)";

                        comm.ExecuteNonQuery();
                        Console.WriteLine("Создана таблица tmp3.");

                        comm.CommandText =
                            "INSERT INTO dbo.tmp3(f1, f2) " +
                            "VALUES('Hello', 100)";

                        comm.ExecuteNonQuery();
                        Console.WriteLine("Вставлена строка 1.");

                        comm.CommandText =
                            "INSERT INTO dbo.tmp3(f1, f2) " +
                            "VALUES('World', 200)";

                        comm.ExecuteNonQuery();
                        Console.WriteLine("Вставлена строка 2.");
                    }

                    tran.Commit();
                    committed = true;

                    Console.WriteLine("\nТРАНЗАКЦИЯ ЗАФИКСИРОВАНА.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);

                    if (tran != null)
                    {
                        try
                        {
                            tran.Rollback();
                            Console.WriteLine("ТРАНЗАКЦИЯ ОТКАЧЕНА.");
                        }
                        catch (Exception rollbackEx)
                        {
                            Console.WriteLine(
                                "Ошибка отката: " + rollbackEx.Message);
                        }
                    }
                }
                finally
                {
                    if (tran != null)
                        tran.Dispose();
                }
            }

            if (committed)
            {
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    using (SqlCommand check = new SqlCommand(
                        "SELECT COUNT(*) FROM dbo.tmp3", conn))
                    {
                        int count = Convert.ToInt32(check.ExecuteScalar());

                        Console.WriteLine(
                            "\nПроверка: строк в tmp3 = " + count +
                            " (ожидается 2).");
                    }

                    using (SqlCommand cleanup = new SqlCommand(
                        "DROP TABLE dbo.tmp3", conn))
                    {
                        cleanup.ExecuteNonQuery();
                    }

                    Console.WriteLine("Таблица tmp3 удалена.");
                }
            }
        }
        static void FailedTransaction()
        {
            DropTmp3IfExists();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlTransaction tran = null;

                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();

                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        comm.Transaction = tran;

                        comm.CommandText =
                            "CREATE TABLE dbo.tmp3 (" +
                            "id INT IDENTITY PRIMARY KEY, " +
                            "f1 VARCHAR(20), f2 INT)";

                        comm.ExecuteNonQuery();
                        Console.WriteLine("Создана таблица tmp3.");

                        comm.CommandText =
                            "INSERT INTO dbo.tmp3(f1, f2) " +
                            "VALUES('Hello', 100)";

                        comm.ExecuteNonQuery();
                        Console.WriteLine("Вставлена строка 1.");
                        comm.CommandText =
                            "INSERT INTO dbo.tmp4(f1, f2) " +
                            "VALUES('World', 200)";

                        comm.ExecuteNonQuery();

                        Console.WriteLine(
                            "Эта строка не должна выполниться.");
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nОШИБКА: " + ex.Message);

                    if (tran != null)
                    {
                        try
                        {
                            tran.Rollback();

                            Console.WriteLine(
                                "ТРАНЗАКЦИЯ ОТКАЧЕНА — все шаги отменены.");
                        }
                        catch (Exception rollbackEx)
                        {
                            Console.WriteLine(
                                "Ошибка отката: " + rollbackEx.Message);
                        }
                    }
                }
                finally
                {
                    if (tran != null)
                        tran.Dispose();
                }
            }

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand check = new SqlCommand(
                    "SELECT COUNT(*) FROM sys.tables " +
                    "WHERE name = N'tmp3' " +
                    "AND schema_id = SCHEMA_ID(N'dbo')", conn))
                {
                    int count = Convert.ToInt32(check.ExecuteScalar());

                    Console.WriteLine(
                        "\nПроверка: таблиц tmp3 в БД = " + count +
                        " (ожидается 0).");
                }
            }
        }
        static void DropTmp3IfExists()
        {
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(
                    "IF OBJECT_ID('dbo.tmp3', 'U') IS NOT NULL " +
                    "DROP TABLE dbo.tmp3;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        static void PrintDataTable(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                Console.WriteLine("(нет данных)");
                return;
            }

            foreach (DataColumn col in dt.Columns)
                Console.Write(col.ColumnName.PadRight(15) + "| ");

            Console.WriteLine();
            Console.WriteLine(new string('-', 17 * dt.Columns.Count));

            foreach (DataRow row in dt.Rows)
            {
                foreach (object value in row.ItemArray)
                {
                    string text = value == null || value == DBNull.Value
                        ? ""
                        : value.ToString();

                    Console.Write(text.PadRight(15) + "| ");
                }

                Console.WriteLine();
            }
        }
        static void PrintDataView(DataView dv)
        {
            foreach (DataColumn col in dv.Table.Columns)
                Console.Write(col.ColumnName.PadRight(15) + "| ");

            Console.WriteLine();
            Console.WriteLine(new string('-', 17 * dv.Table.Columns.Count));

            foreach (DataRowView row in dv)
            {
                foreach (object value in row.Row.ItemArray)
                {
                    string text = value == null || value == DBNull.Value
                        ? ""
                        : value.ToString();

                    Console.Write(text.PadRight(15) + "| ");
                }

                Console.WriteLine();
            }
        }
    }
}
