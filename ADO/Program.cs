using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Utilities;
namespace ADO;

class Program
{
    // ReSharper disable once UnusedParameter.Local
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("connection.json")
            .Build();
        
        string connectionString = config.GetConnectionString("DefaultConnection")!;

        using (var connection = new SqlConnection(connectionString))
        {
            
            connection.Open();
            while (true)
            {
                var tablesQuery = $@"
                        SELECT TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_TYPE = 'BASE TABLE'";
                var tables = new HybridList<string>();

                using (var tableCommand = new SqlCommand(tablesQuery, connection))
                using (var reader = tableCommand.ExecuteReader())
                {
                   Console.WriteLine("Таблицы в базе данных:");
                   int i = 0;
                   while (reader.Read())
                   {
                      string tableName = reader.GetString(0);
                      tables.Add(tableName, tableName);
                      Console.WriteLine($"{i} - {tableName}");
                      i++;
                   }
                } // reader закроется автоматически
                
                bool temp = true;
                string? tb = null;

                while (temp){
                    Console.WriteLine("Выберете таблицу: ");
                    string? choice = Console.ReadLine();
                    try
                    {
                        tb = tables[Convert.ToInt32(choice)];
                    }
                    catch (FormatException)
                    {
                        tb = tables[choice];
                    }
                    
                    if (tb != string.Empty) { temp = false; }
                }

                temp = true;
                while (temp){
                    Console.Clear();
                    List<string> nme;
                    HybridList<string> tdn;
                    string tmp;
                    int c;
                    Console.WriteLine("1 - Показать структуру тб");
                    Console.WriteLine("2 - Вывести все данные с таблицы");
                    Console.WriteLine("3 - Вставить строку");
                    Console.WriteLine("4 - Обновить строку по Id");
                    Console.WriteLine("5 - Удалить строку по Id");
                    Console.WriteLine("6 - Выбрать другую тб");
                    Console.WriteLine("Любое другое - Выйти");
                    Console.WriteLine("Выберете действие: ");
                    string ch = Console.ReadLine();
                    Console.Clear();
                    switch (ch)
                    {
                        case "1":
                            Console.WriteLine($"Таблица {tb}");

                            tablesQuery = $@"
                            SELECT COLUMN_NAME, DATA_TYPE 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = @TableName";

                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {
                                columnCommand.Parameters.AddWithValue("@TableName", tb);

                                using (var columnReader = columnCommand.ExecuteReader())
                                {
                                    while (columnReader.Read())
                                    {

                                        string columnName = columnReader.GetString(0);
                                        string columnType = columnReader.GetString(1);
                                        Console.WriteLine($"{columnName} - {columnType}");
                                    }
                                }
                            }
                            break;
                        case "2":
                            tablesQuery = $@"
                                        SELECT *
                                        FROM dbo.{tb}";
                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {

                                using (var columnReader = columnCommand.ExecuteReader())
                                {
                                    for (int i = 0; i < columnReader.FieldCount; i++)
                                    {
                                        Console.Write($"{columnReader.GetName(i)}\t");
                                    }

                                    Console.WriteLine();

                                    while (columnReader.Read())
                                    {
                                        for (int i = 0; i < columnReader.FieldCount; i++)
                                        {
                                            Console.Write($"{columnReader.GetValue(i)}\t");
                                        }

                                        Console.WriteLine();
                                    }
                                }
                            }

                            break;
                        case "3":
                            tablesQuery = $@"
                            SELECT COLUMN_NAME, DATA_TYPE,
                                   COLUMNPROPERTY(OBJECT_ID('dbo.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity'),
                                   COLUMNPROPERTY(OBJECT_ID('dbo.' + TABLE_NAME), COLUMN_NAME, 'IsComputed')
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = @TableName";

                            nme = new List<string>();
                            tdn = new HybridList<string>();

                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {
                                columnCommand.Parameters.AddWithValue("@TableName", tb);

                                using (var columnReader = columnCommand.ExecuteReader())
                                {
                                    while (columnReader.Read())
                                    {
                                        string columnName = columnReader.GetString(0);
                                        string columnType = columnReader.GetString(1);
                                        if (columnReader.GetInt32(2) != 1 && columnReader.GetInt32(3) != 1)
                                        {
                                            Console.Write($"Введите {columnName}({columnType}): ");
                                            tmp = Console.ReadLine() ?? throw new Exception();
                                            nme.Add(columnName);
                                            tdn.Add(columnName, tmp);
                                        }
                                    }
                                }
                            }

                            tablesQuery = $@"
                                        INSERT INTO dbo.{tb} ({string.Join(", ", nme)})
                                            VALUES ({string.Join(", ", nme.Select(s => $"@{s}"))})";
                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {
                                foreach (var col in tdn)
                                {
                                    columnCommand.Parameters.AddWithValue($"@{tdn.GetKey(col)}", col);
                                }

                                Console.WriteLine(columnCommand.ExecuteNonQuery() == 1 ? "Операция прошла успешно" : "Операция не прошла успешно" );
                            }

                            break;
                        case "4":
                            tablesQuery = $@"
                                        SELECT *
                                        FROM dbo.{tb}";
                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {

                                using (var columnReader = columnCommand.ExecuteReader())
                                {
                                    for (int i = 0; i < columnReader.FieldCount; i++)
                                    {
                                        Console.Write($"{columnReader.GetName(i)}\t");
                                    }

                                    Console.WriteLine();

                                    while (columnReader.Read())
                                    {
                                        for (int i = 0; i < columnReader.FieldCount; i++)
                                        {
                                            Console.Write($"{columnReader.GetValue(i)}\t");
                                        }

                                        Console.WriteLine();
                                    }
                                }
                            }

                            Console.Write("Введите нужное ID поля: ");
                            c = Convert.ToInt32(Console.ReadLine());

                            tablesQuery = $@"
                            SELECT COLUMN_NAME, DATA_TYPE,
                                   COLUMNPROPERTY(OBJECT_ID('dbo.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity'),
                                   COLUMNPROPERTY(OBJECT_ID('dbo.' + TABLE_NAME), COLUMN_NAME, 'IsComputed')
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = @TableName";

                            nme = new List<string>();
                            tdn = new HybridList<string>();

                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {
                                columnCommand.Parameters.AddWithValue("@TableName", tb);

                                using (var columnReader = columnCommand.ExecuteReader())
                                {
                                    while (columnReader.Read())
                                    {
                                        string columnName = columnReader.GetString(0);
                                        string columnType = columnReader.GetString(1);
                                        if (columnReader.GetInt32(2) != 1 && columnReader.GetInt32(3) != 1)
                                        {
                                            Console.Write($"Введите {columnName}({columnType}): ");
                                            tmp = Console.ReadLine() ?? throw new Exception();
                                            nme.Add(columnName);
                                            tdn.Add(columnName, tmp);
                                        }
                                    }
                                }
                            }

                            tablesQuery = $@"
                                        UPDATE dbo.{tb}
                                        SET {string.Join(", ", nme.Select(s => $"{s} = @{s}"))}
                                        WHERE id = {c}";
                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {
                                foreach (var col in tdn)
                                {
                                    columnCommand.Parameters.AddWithValue($"@{tdn.GetKey(col)}", col);
                                }

                                Console.WriteLine(columnCommand.ExecuteNonQuery() == 1 ? "Операция прошла успешно" : "Операция не прошла успешно" );
                            }

                            break;
                        case "5":
                            tablesQuery = $@"
                                        SELECT *
                                        FROM dbo.{tb}";
                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {

                                using (var columnReader = columnCommand.ExecuteReader())
                                {
                                    for (int i = 0; i < columnReader.FieldCount; i++)
                                    {
                                        Console.Write($"{columnReader.GetName(i)}\t");
                                    }

                                    Console.WriteLine();

                                    while (columnReader.Read())
                                    {
                                        for (int i = 0; i < columnReader.FieldCount; i++)
                                        {
                                            Console.Write($"{columnReader.GetValue(i)}\t");
                                        }

                                        Console.WriteLine();
                                    }
                                }
                            }

                            Console.Write("Введите нужное ID поля: ");
                            c = Convert.ToInt32(Console.ReadLine());

                            tablesQuery = $@"
                                        DELETE
                                        FROM dbo.{tb}
                                        WHERE id = @id";
                            using (var columnCommand = new SqlCommand(tablesQuery, connection))
                            {
                                columnCommand.Parameters.AddWithValue($"@id", c);
                                Console.WriteLine(columnCommand.ExecuteNonQuery() == 1 ? "Операция прошла успешно" : "Операция не прошла успешно" );
                            }

                            break;
                        case "6":
                            temp = false;
                            break;
                        default:
                            return;
                    }
                    Console.WriteLine("Нажмите что бы продолжить: ");
                    Console.ReadKey();
                }
            }
        }
    }
}