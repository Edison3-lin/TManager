using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Management;

namespace SQL
{
    internal class Program
    {

        public static string GetSystemUUID()
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");
            ManagementObjectCollection moc = mc.GetInstances();

            string uuid = string.Empty;

            foreach (ManagementObject mo in moc)
            {
                uuid = mo["UUID"].ToString();
                break; // Assuming there's only one UUID
            }

            return uuid;
        }

        //連接資料庫
        public void Conn(SqlConnection conn, string ConnectionString)
        {
            //取得或設定用於開啟 SQL Server 資料庫的字串
            conn.ConnectionString = ConnectionString;
            try
            {
                //開啟數據庫
                conn.Open();
                //列印資料庫連線狀態
                Console.WriteLine(conn.State);
 
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Fail");
                Console.WriteLine(ex.Message);
            }
 
        }

        //insert
        public void Insert(SqlConnection conn, string sql_insert)
        {
           
            // String sql_insert = "insert into Table_1(uid,name) values(@UID,@NAME)";
 
            SqlCommand cmd_insert = new SqlCommand(sql_insert, conn);
            SqlParameter para1 = new SqlParameter("@UID", "106");
            cmd_insert.Parameters.Add(para1);
            SqlParameter para2 = new SqlParameter("@NAME", "Bit106");
            cmd_insert.Parameters.Add(para2);
 
            //对连接执行 Transact-SQL 语句并返回受影响的行数
            int res_1 = cmd_insert.ExecuteNonQuery();
            Console.WriteLine(res_1);
        }
 
        //update
        public void update(SqlConnection conn, string sql_update)
        { 
            // string sql_update = "update Table_1 set name=@NAME where id=@ID;";
            SqlCommand cmd_update = new SqlCommand(sql_update, conn);
            cmd_update.Parameters.AddWithValue("@ID", "3");
            cmd_update.Parameters.AddWithValue("@NAME", "Bit100");
            int res_2 = cmd_update.ExecuteNonQuery();
            Console.WriteLine(res_2);
        }
 
        //delete
        public void delete(SqlConnection conn, string sql_delete)
        {
            // string sql_delete = "DELETE FROM Table_1 WHERE name=@NAME;";
            SqlCommand cmd_delete = new SqlCommand(sql_delete, conn);
            cmd_delete.Parameters.AddWithValue("@NAME", "Bit106");
            int res_3 = cmd_delete.ExecuteNonQuery();
            Console.WriteLine(res_3);
        }
 
        //select 
        public void select(SqlConnection conn, string sql)
        {
            //定义查询语句
            // String sql = "SELECT [DP_ID],[DP_UUID],[DP_Product_Name],[DP_Status] FROM [SIT_TEST].[DBO].[DUT_Profile]";
            SqlCommand sqlComm = new SqlCommand(sql, conn);
            //接收查询到的sql数据
            SqlDataReader reader = sqlComm.ExecuteReader();
            //读取数据
            while (reader.Read())
            {
                //打印
                string DP_ID = (reader["DP_ID"].ToString());
                string DP_UUID = (reader["DP_UUID"].ToString());
                string DP_Product_Name = (reader["DP_Product_Name"].ToString());
                string DP_Status = (reader["DP_Status"].ToString());
                Console.WriteLine(DP_ID+" "+DP_UUID+" "+DP_Status);
            }
            reader.Close();
        }

        static void Main(string[] args)
        {


            string uuid = GetSystemUUID();
            Console.WriteLine("System UUID: " + uuid);

            //建立一个连接数据库的对象
            SqlConnection conn = new SqlConnection();
            Program p = new Program();
            string ConnectionString = "Server=DESKTOP-VD92848;DataBase=SIT_TEST;User Id=edison;Password=123;";
            p.Conn(conn, ConnectionString);
            
            //查
            String sql = "SELECT [DP_ID],[DP_UUID],[DP_Product_Name],[DP_Status] FROM [SIT_TEST].[DBO].[DUT_Profile]";
            p.select(conn, sql);

            //增
            //p.Insert(conn);

            //改
            //p.update(conn);

            //删
            //p.delete(conn);
 
            //调用存储过程
            //p.procedure(conn);
 
            //批量写入
            // p.insertBulk(conn);

            conn.Close();
            conn.Dispose();
 
            // Console.ReadLine();

        }
    }
}
