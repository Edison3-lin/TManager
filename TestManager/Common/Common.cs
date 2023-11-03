using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestManager;

namespace Common
{
    public class Runnner
    {
        private static string test_dll_folder;

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

            Program.process_log("             "+dllPath);
            try
            {
                Assembly myDll = Assembly.LoadFile(dllPath);
                var myTest=myDll.GetTypes().First(m=>!m.IsAbstract && m.IsClass);
                object myObj = myDll.CreateInstance(myTest.FullName);
                object myResult = myTest.GetMethod("Setup").Invoke(myObj, new object[]{});            
                Program.process_log(myResult.ToString());
                myResult = myTest.GetMethod("Run").Invoke(myObj, new object[]{});            
                Program.process_log(myResult.ToString());
                myResult = myTest.GetMethod("UpdateResults").Invoke(myObj, new object[]{});            
                Program.process_log(myResult.ToString());
                myResult = myTest.GetMethod("TearDown").Invoke(myObj, new object[]{});            
                Program.process_log(myResult.ToString());
            }
            catch (Exception ex)
            {
                Program.process_log("Common Error!!! " + ex.Message);
            }            
            return true;
        }
    }
    public class Testflow
    {
        public static int Setup(string DllName)
        {
            General.WriteLog(DllName, "Testflow::Setup");
            Program.process_log(DllName);
            return 90;
        }
        public static int Run(string DllName)
        {

            General.WriteLog(DllName, "Run.log");
            Program.process_log(DllName+" Run.log");
            return 91;
        }

        public static int UpdateResults(string DllName, bool passFail)
        {
            General.WriteLog(DllName, "Testflow::UpdateResults");
            Program.process_log(DllName);
            return 92;
        }

        public static int TearDown(string DllName)
        {
            General.WriteLog(DllName, "Testflow::TearDown");
            Program.process_log(DllName);
            return 93;
        }

        public class General
        {
            public static int WriteLog(string DllName, string content)
            {
                // TODO: create different log file for the same dll (with timestamp)
                // string appendText = DllName + " " + line + Environment.NewLine;
                // File.AppendAllText(DllName + ".log", appendText);

                // 指定要建立或寫入的檔案的路徑
                string filePath = DllName;

                try
                {
                    // 使用 StreamWriter 打開檔案並appand內容
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.Write("["+DateTime.Now.ToString()+"] "+content+'\n');
                    }

                }
                catch (Exception ex)
                {
                    Program.process_log("Error!!! " + ex.Message);
                }
              

                return 0;
            }
        }
    }
}
