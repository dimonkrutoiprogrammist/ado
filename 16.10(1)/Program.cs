using System;
using System.Configuration;
using System.Data; 
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            //Task1();
            //Task2();
            //Task3();
            //Task4();
            //Task5();
            //Task6();
            //Task7();
        }
        static void PrintTable(DataTable table, string title = "")
        {
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine($"\n{title}");
            }
            foreach (DataColumn col in table.Columns)
                Console.Write(col.ColumnName.PadRight(15));
            Console.WriteLine();
            Console.WriteLine(new string('-', table.Columns.Count * 15));
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    object val = row[col];
                    string s = val == DBNull.Value ? "NULL" : val.ToString();
                    Console.Write(s.PadRight(15));
                }
                Console.WriteLine();
            }
        }

        static void Task1()
        {
            DataTable table = new DataTable("Authors");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            DataRow row1 = table.NewRow();
            row1["Id"] = 1;
            row1["FirstName"] = "Isaac";
            row1["LastName"] = "Asimov";
            table.Rows.Add(row1);
            DataRow row2 = table.NewRow();
            row2["Id"] = 2;
            row2["FirstName"] = "Ray";
            row2["LastName"] = "Bradbury";
            table.Rows.Add(row2);
            DataRow row3 = table.NewRow();
            row3["Id"] = 3;
            row3["FirstName"] = "Clifford";
            row3["LastName"] = "Simak";
            table.Rows.Add(row3);
            DataRow row4 = table.NewRow();
            row4["Id"] = 4;
            row4["FirstName"] = "Roger";
            row4["LastName"] = "Zelazny";
            table.Rows.Add(row4); 
            PrintTable(table, "Authors");
            if (table.Rows.Count > 0)
            {
                DataRow firstRow = table.Rows[0];
                Console.WriteLine($"Чтение через индекс row[0]: {firstRow[0]}");
                Console.WriteLine($"Чтение через имя колонки row[\"FirstName\"]: {firstRow["FirstName"]}");
            }
        }
        static void Task2()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            { 
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Authors", conn);

                DataSet ds = new DataSet();
                da.Fill(ds, "Authors");
                PrintTable(ds.Tables["Authors"], "Authors из БД");
                da.SelectCommand.CommandText = "SELECT * FROM Books";
                da.Fill(ds, "Books");
                PrintTable(ds.Tables["Books"], "Books из БД");
            }
        }
        static void Task3()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", conn);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds, "Books");
                DataTable booksTable = ds.Tables["Books"];
                if (booksTable.Rows.Count == 0)
                {
                    Console.WriteLine("\nНет 1данных в таблице Books");
                    return;
                }
                PrintTable(booksTable, "Books до локального изменения");
                booksTable.Rows[0]["Price"] = 999; 
                PrintTable(booksTable, "Books после локального изменения");
                da.Update(ds, "Books");
                Console.WriteLine("\nТексты автоматически сгенерированных команд");
                Console.WriteLine("INSERT: " + cmdBuilder.GetInsertCommand().CommandText);
                Console.WriteLine("UPDATE: " + cmdBuilder.GetUpdateCommand().CommandText);
                Console.WriteLine("DELETE: " + cmdBuilder.GetDeleteCommand().CommandText);


                Console.WriteLine("\nПроблемный эксперимент");
    
                SqlDataAdapter daBad = new SqlDataAdapter("SELECT Title, Price, Pages FROM Books", conn);
     
                SqlCommandBuilder cmdBuilderBad = new SqlCommandBuilder(daBad);

                DataSet dsBad = new DataSet();
         
                daBad.Fill(dsBad, "BooksBad");

  
                if (dsBad.Tables["BooksBad"].Rows.Count > 0)
                {
                  
                    dsBad.Tables["BooksBad"].Rows[0]["Price"] = 1200;
                    try
                    {
                   
                        daBad.Update(dsBad, "BooksBad");
                    }
                    catch (InvalidOperationException ex)
                    {
               
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

   
        static void Task4()
        {
         
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
              
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", conn);
             
                DataSet ds = new DataSet();
                da.Fill(ds, "Books");

                DataTable booksTable = ds.Tables["Books"];

           
                if (booksTable.Rows.Count < 2)
                {
                    Console.WriteLine("\n[Task 4] В базе данных должно быть не менее 2 книг для выполнения теста.");
                    return;
                }

      
                SqlCommand updateCmd = new SqlCommand("UPDATE Books SET Price = @pPrice WHERE Id = @pId", conn);
        
                updateCmd.Parameters.Add(new SqlParameter("@pPrice", SqlDbType.Int));
          
                updateCmd.Parameters["@pPrice"].SourceVersion = DataRowVersion.Current;
       
                updateCmd.Parameters["@pPrice"].SourceColumn = "Price";

         
                updateCmd.Parameters.Add(new SqlParameter("@pId", SqlDbType.Int));
          
                updateCmd.Parameters["@pId"].SourceVersion = DataRowVersion.Original;
             
                updateCmd.Parameters["@pId"].SourceColumn = "Id";

         
                da.UpdateCommand = updateCmd;

       
                booksTable.Rows[0]["Price"] = 450;
                booksTable.Rows[1]["Price"] = 620;

                Console.WriteLine("\nЛокальные изменения внесены");

         
                da.Update(ds, "Books");
            }
        }

     
        static void Task5()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", conn);
                SqlCommandBuilder cb = new SqlCommandBuilder(da); 
                DataSet ds = new DataSet();
                da.Fill(ds, "Books");

                DataTable books = ds.Tables["Books"];
                if (books.Rows.Count < 2)
                {
                    Console.WriteLine("\nНедостаточно строк в Books");
                    return;
                }

       
                books.Rows[0].Delete();

             
                books.Rows[1]["Price"] = 777;

             
                DataRow newRow = books.NewRow();
                newRow["AuthorId"] = 1;
                newRow["Title"] = "New Book";
                newRow["Price"] = 500;
                newRow["Pages"] = 100;
                books.Rows.Add(newRow);

         
                Console.WriteLine("\nТекущие состояния строк ");
                foreach (DataRow row in books.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    Console.WriteLine($"{row["Title"],-20} -> {row.RowState}");
                }

     
                da.Update(books.Select(null, null, DataViewRowState.Deleted));
                da.Update(books.Select(null, null, DataViewRowState.ModifiedCurrent));
                da.Update(books.Select(null, null, DataViewRowState.Added));

      
                DataSet dsVerify = new DataSet();
                da.Fill(dsVerify, "Books");
                PrintTable(dsVerify.Tables["Books"], "Books после синхронизации");
            }
        }

        static void Task6()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
      
                string sql = "SELECT * FROM Authors; SELECT * FROM Books;";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);

                DataSet dsDefault = new DataSet();
                da.Fill(dsDefault);
                
                Console.WriteLine("\nИмена таблиц по умолчанию");
                foreach (DataTable t in dsDefault.Tables)
                {
                    Console.WriteLine($"Имя таблицы в DataSet: {t.TableName}");
                }

                SqlDataAdapter daWithMaps = new SqlDataAdapter(sql, conn);


                DataTableMapping dtm = daWithMaps.TableMappings.Add("Table", "Authors");

                dtm.ColumnMappings.Add("Id", "Num");
                dtm.ColumnMappings.Add("FirstName", "Name");
                dtm.ColumnMappings.Add("LastName", "Surname");
                daWithMaps.TableMappings.Add("Table1", "Books");
                DataSet dsMapped = new DataSet();
                daWithMaps.Fill(dsMapped);

            }
        }
        static void Task7()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Picture FROM Pictures WHERE Id = @id", conn);
                da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = 1;

                DataSet ds = new DataSet();
                da.Fill(ds);
                byte[] bytes = (byte[])ds.Tables[0].Rows[0]["Picture"];
                System.IO.File.WriteAllBytes("out.jpg", bytes);
            }


        }
    }
}
