using System;
using System.Data.SqlClient;
 
namespace DatabaseExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=YourServer;Initial Catalog=YourDatabase;User ID=YourUsername;Password=YourPassword";
 
            // 连接到数据库
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
 
                // 创建表
                string createTableQuery = "CREATE TABLE Persons (Id INT PRIMARY KEY, Name VARCHAR(50), Age INT)";
                using (SqlCommand createTableCommand = new SqlCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                    Console.WriteLine("Table created successfully.");
                }
 
                // 插入数据
                string insertQuery = "INSERT INTO Persons (Id, Name, Age) VALUES (1, 'John', 30)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Data inserted successfully.");
                }
 
                // 查询数据
                string selectQuery = "SELECT * FROM Persons";
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            int age = reader.GetInt32(2);
                            Console.WriteLine("Id: {0}, Name: {1}, Age: {2}", id, name, age);
                        }
                    }
                }
 
                // 关闭连接
                connection.Close();
            }
 
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}