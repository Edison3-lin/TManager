using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Data.SqlClient;
using System.Data;

namespace Common
{
    public class Runnner
    {
        //private static string test_dll_folder;

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
    
            // Console.WriteLine(uuid);

            return uuid;
        }

        //連接資料庫
        public string Conn(string ConnectionString)
        {
            //建立一个连接数据库的对象
            SqlConnection conn = new SqlConnection();
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

            String sql = "SELECT [DP_ID],[DP_UUID],[DP_Product_Name],[DP_Status] FROM [SIT_TEST].[DBO].[DUT_Profile]";
            SqlCommand sqlComm = new SqlCommand(sql, conn);
            //接收查詢到的sql數據
            SqlDataReader reader = sqlComm.ExecuteReader();

            // string DP_ID, DP_UUID, DP_Product_Name, DP_Status;
            //讀取數據
            string UUID = GetSystemUUID();
            while (reader.Read())
            {
                string DP_ID = (reader["DP_ID"].ToString());
                string DP_UUID = (reader["DP_UUID"].ToString());
                string DP_Product_Name = (reader["DP_Product_Name"].ToString());
                string DP_Status = (reader["DP_Status"].ToString());
                if((DP_Status == "new_") && (UUID == DP_UUID))
                {
                    // Console.WriteLine(DP_ID+" "+DP_UUID+" "+DP_Status);
                    reader.Close();
                    conn.Close();
                    conn.Dispose();
                     return DP_Product_Name;                    
                    // break;
                }    
            }
            conn.Close();
            conn.Dispose();
            reader.Close();
            return null;
        }

        //select 
        public void select(SqlConnection conn, string sql)
        {
            //定義查詢語句
            // String sql = "SELECT [DP_ID],[DP_UUID],[DP_Product_Name],[DP_Status] FROM [SIT_TEST].[DBO].[DUT_Profile]";
            SqlCommand sqlComm = new SqlCommand(sql, conn);
            //接收查詢到的sql數據
            SqlDataReader reader = sqlComm.ExecuteReader();
            //讀取數據
            while (reader.Read())
            {
                string DP_ID = (reader["DP_ID"].ToString());
                string DP_UUID = (reader["DP_UUID"].ToString());
                string DP_Product_Name = (reader["DP_Product_Name"].ToString());
                string DP_Status = (reader["DP_Status"].ToString());
                Console.WriteLine(DP_ID+" "+DP_UUID+" "+DP_Status);
            }
            reader.Close();
        }



        public void SetTestDllFolder(string folder)
        {
            // Runnner.test_dll_folder = folder;
        }

        /*
         * DllName: the unique file name (without '.dll') of the test dll. For example: MyTestItem_1
         * 
         */
        public static bool RunTestItem(string dllPath)
        {

            Testflow.General.WriteLog("RunTestItem", dllPath);
            try
            {
                Assembly myDll = Assembly.LoadFile(dllPath);
                var myTest=myDll.GetTypes().First(m=>!m.IsAbstract && m.IsClass);
                object myObj = myDll.CreateInstance(myTest.FullName);
                object myResult = myTest.GetMethod("Setup").Invoke(myObj, new object[]{});            
                myResult = myTest.GetMethod("Run").Invoke(myObj, new object[]{});            
                myResult = myTest.GetMethod("UpdateResults").Invoke(myObj, new object[]{});            
                myResult = myTest.GetMethod("TearDown").Invoke(myObj, new object[]{});            
            }
            catch (Exception ex)
            {
                // Testflow.General.WriteLog("RunTestItem", "Common Error!!! " + ex.Message);
            }            
            return true;
        }
    }
    public class Testflow
    {
        public static int Setup(string DllName)
        {
            General.WriteLog(DllName, "Testflow::Setup");
            return 90;
        }
        public static int Run(string DllName)
        {
            General.WriteLog(DllName, "Testflow::Run");
            return 90;
        }

        public static int UpdateResults(string DllName, bool passFail)
        {
            General.WriteLog(DllName, "Testflow::UpdateResults");
            return 90;
        }

        public static int TearDown(string DllName)
        {
            General.WriteLog(DllName, "Testflow::TearDown");
            return 90;
        }

        public class General
        {
            public static int WriteLog(string DllName, string content)
            {
                string log_path = "C:\\TestManager\\ResultUpload\\" + DllName+".log";
                // 檢查目錄是否存在，如果不存在則建立
                if (!Directory.Exists("C:\\TestManager\\ResultUpload\\"))
                {
                    Directory.CreateDirectory("C:\\TestManager\\ResultUpload\\");
                }                

                try
                {
                    // 使用 StreamWriter 打開檔案並appand內容
                    using (StreamWriter writer = new StreamWriter(log_path, true))
                    {
                        writer.Write("["+DateTime.Now.ToString()+"] "+content+'\n');
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("WriteLog Error!!! " + ex.Message);
                }
            
                return 0;
            }
        }
    }
}
