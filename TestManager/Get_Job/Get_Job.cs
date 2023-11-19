using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Get_Job
{
    class Get_Job
    {
        public static string[] getMyJob()
        {
            string connectionString = "Data Source=DESKTOP-VD92848;Initial Catalog=SIT_TEST;User ID=edison;Password=123";

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
            return new string[]{ "Alice", "Bob", "Charlie" }; 
        }


    }
}