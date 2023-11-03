using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;             //手動加入參考
using System.Management.Automation.Runspaces;   //手動加入參考
using System.Threading;
using System.Reflection;

namespace TestManager
{
    public class Program
    {
        public static string testPath = "C:\\TestManager\\";
        public static string downloadPath = "C:\\TestManager\\Download\\";
        public static string log_file = "TestManager.log";
        // **** 創建log file ****
        static void CreateDirectoryAndFile()
        {
            string filePath = testPath+log_file;
            try
            {
                // 檢查目錄是否存在，如果不存在則建立
                if (!Directory.Exists(testPath))
                {
                    Directory.CreateDirectory(testPath);
                }                
                // 檢查檔案是否存在，如果不存在則建立，檔案存在內容就清空
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                else
                {
                    // 清空內容
                    // File.WriteAllText(filePath, string.Empty);
                }
            }
            catch (Exception ex)
            {
                process_log("Error!!! " + ex.Message);
            }
        }

        // **** Test manager log file ****
        public static void process_log(string content)
        {
            // 指定要建立或寫入的檔案的路徑
            string filePath = testPath+log_file;

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
                process_log("Error!!! " + ex.Message);
            }
        }      
        
        // ***** get jobs from DB *****
        static string[] Get_Job()
        {
            string[] job_list = {};
            CreateDirectoryAndFile();
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();
        try
        {
            // pipeline.Commands.AddScript(testPath+"RunAs.ps1");
            pipeline.Commands.AddScript(testPath+"Get_Job.ps1");
            var result = pipeline.Invoke();
            foreach (var psObject in result)
            {
                if(psObject != null)
                {
                    // 新增一個 job 到 Job_List
                    Array.Resize(ref job_list, job_list.Length + 1);
                    job_list[job_list.Length - 1] = psObject.ToString();
                }
                else
                {
                    runspace.Close();
                    return null;
                }
            }
        }    
        catch (Exception ex)
        {
            process_log(ex.Message);
            runspace.Close();
            return null;
        }

            runspace.Close();
            return job_list;
        }

        // ***** download a program from FTP *****
        static void FTP_Download(string[] job_list)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

        try
        {
            // pipeline.Commands.AddScript(testPath+"RunAs.ps1");
            string remoteFile = "@(\"";
            foreach (string job in job_list)
            {
                remoteFile += job;
                remoteFile += "\",\"";
            }
            remoteFile += '!';
            string a = remoteFile.Replace(",\"!",")");
            // process_log(a);
            pipeline.Commands.AddScript("$remoteFile = "+a);
            pipeline.Commands.AddScript(testPath+"Download.ps1");
            var result = pipeline.Invoke();
        }    
        catch (Exception ex)
        {
            process_log(ex.Message);
            runspace.Close();
            return;
        }

            runspace.Close();
            return;
        }

        // ***** update job_status to DB *****
        static bool Update_Job_Status()
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            try
            {
                // pipeline.Commands.AddScript(testPath+"RunAs.ps1");
                pipeline.Commands.AddScript(testPath+"Update_Job_Status.ps1");
                var result = pipeline.Invoke();
                if(result[0].ToString() == "Unconnected_")
                {
                    runspace.Close();
                    return false;
                }

            }    
            catch (Exception ex)
            {
                process_log(ex.Message);
                runspace.Close();
                return false;
            }

            runspace.Close();
            return true;
        }

        // ***** Executing a job *****
        static void Executing(string job)
        {
            try
            {
             Runspace runspace = RunspaceFactory.CreateRunspace();
             runspace.Open();
             Pipeline pipeline = runspace.CreatePipeline();
            //  pipeline.Commands.AddScript(testPath+"RunAs.ps1");
             if (!File.Exists(job))
             {
                 process_log("Error!!! File not exist");
                 runspace.Close();
                 return;
             }
            //  process_log("執行 "+job);
             pipeline.Commands.AddScript(job);
             pipeline.Invoke();
             runspace.Close();
            }
            catch (Exception ex)
            {
                process_log("Error!!! " + ex.Message);
            }
             return;
        }

        // ***** Executing a dll *****
        static bool Execute_dll(string dllPath)
        {
            string callingDomainName = AppDomain.CurrentDomain.FriendlyName;//Thread.GetDomain().FriendlyName;
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            AppDomain ad = AppDomain.CreateDomain("TestManager DLL");
            ProxyObject obj = (ProxyObject)ad.CreateInstanceFromAndUnwrap(basePath+callingDomainName, "TestManager.ProxyObject");
            try
            {
                obj.LoadAssembly("c:\\TestManager\\TestManager\\Common\\bin\\Debug\\Common.dll");
            }
            catch (System.IO.FileNotFoundException)
            {
                process_log("!!! 找不到 Common.dll");
                return false;
            }
            Object[] p = new object[]{dllPath};
            obj.Invoke("RunTestItem",p);
            // process_log("             Invoke .Setup()");
            // obj.Invoke("Setup", p);
            // process_log("             Invoke .Run()");
            // obj.Invoke("Run", p);
            // process_log("             Invoke .UpdateResults()");
            // obj.Invoke("UpdateResults", p);
            // process_log("             Invoke .TearDown()");
            // obj.Invoke("TearDown", p);
            // process_log("             Unload "+dllPath);
            AppDomain.Unload(ad);
            obj = null;
            return true;
        }


        // ============== MAIN ==============

        static void Main(string[] args)
        {
            string[] Job_List;
            int job_index;
            DateTime startTime, endTime;
            TimeSpan timeSpan;
            do {            
                // step 1. Listening job status from DB, if (job_staus=new_) then get the Job_List
                Job_List = Get_Job();
                if(Job_List == null)
                {
                    // process_log("No job on DB");
                    // Thread.Sleep(1000);
                    continue;
                } 
                else if (Job_List[0] == "Unconnected_")
                {
                    process_log("!!! Wait 3 seconds for the network to connect");
                    process_log("!!! Refer to Get_Job_process.log");
                    Thread.Sleep(3000);
                    continue;
                }    
                process_log("<<Step 1>> Got "+Job_List.Length.ToString()+" items in total");

                // step 2. Download Job_List的PowerShell程式
                process_log("<<Step 2>> Download Job_List items");
                FTP_Download(Job_List);

                // step 3. 依序執行Job_List的PowerShell程式
                process_log("<<Step 3>> 依序執行Job_List的[*.ps1, *.dll]程式");
                job_index = 0;
                startTime = DateTime.Now;
                foreach (string job in Job_List)
                {
                    job_index++;
                    if( job.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase) )
                    {
                        process_log("  Item["+job_index+"] -> 執行"+downloadPath+job);
                        Executing(downloadPath+job);
                    }
                    else if(job.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        process_log("  Item["+job_index+"] -> 執行 DLL");
                        try
                        {
                            Execute_dll(downloadPath+job);
                        }
                        catch (Exception ex)
                        {
                            process_log("Run test Error!!! " + ex.Message);
                        }
                    }
                    else
                    {
                        process_log("  Item["+job_index+"] -> 不執行"+job);
                    }
                }

                // step 4. Test item 測試結束，把job status改為completed_
                process_log("<<Step 4>> 測試結束，把job status改為completed_");
                while(!Update_Job_Status())
                {
                    process_log("!!! Wait 3 seconds for the network to connect");
                    process_log("!!! Refer to Update_Job_Status_process.log");
                    Thread.Sleep(3000);
                }

                // step 5. Job_List的PowerShell程式都完成，繼續Listening job status
                process_log("<<Step 5>> Job_List 的測項完成，Keep listening job status");
                endTime = DateTime.Now;
                timeSpan = endTime - startTime;
                // 输出时间间隔
                process_log("執行花費時間: " + timeSpan.Minutes + "分鐘 " + timeSpan.Seconds + "秒");
                process_log("=================Completed================");
                // Thread.Sleep(1000);
            } while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape));
            
            // Close window
            process_log("**** Exit ****");            
            Environment.Exit(0);            
        }
    }

    class ProxyObject : MarshalByRefObject
    {
        Assembly assembly = null;
        public void LoadAssembly(string myDllPath)
        {
            assembly = Assembly.LoadFile(myDllPath);
        }
        public bool Invoke(string methodName, params Object[] args)
        {
            if (assembly == null)
                return false;
            var cName=assembly.GetTypes().First(m=>!m.IsAbstract && m.IsClass);
            string fullClassName = cName.FullName;
            Type tp = assembly.GetType(fullClassName);
            if (tp == null)
                return false;
            MethodInfo method = tp.GetMethod(methodName);
            if (method == null)
                return false;
            Object obj = Activator.CreateInstance(tp);
            method.Invoke(obj, args);
            return true;
        }
    }    
}
