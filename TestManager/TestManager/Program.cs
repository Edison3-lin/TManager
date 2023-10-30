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
    internal class Program
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
        static void process_log(string content)
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
            pipeline.Commands.AddScript(testPath+"RunAs.ps1");
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
            pipeline.Commands.AddScript(testPath+"RunAs.ps1");
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
                pipeline.Commands.AddScript(testPath+"RunAs.ps1");
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
             pipeline.Commands.AddScript(testPath+"RunAs.ps1");
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
        static void Execute_dll(string myJob)
        {
            try
            {
                Assembly myDll = Assembly.LoadFile(myJob);
                var myTest=myDll.GetTypes().First(m=>!m.IsAbstract && m.IsClass);
                object myObj = myDll.CreateInstance(myTest.FullName);
                object myResult = myTest.GetMethod("Setup").Invoke(myObj, new object[]{});            
                process_log(myResult.ToString());
                myResult = myTest.GetMethod("Run").Invoke(myObj, new object[]{});            
                process_log(myResult.ToString());
                myResult = myTest.GetMethod("UpdateResults").Invoke(myObj, new object[]{});            
                process_log(myResult.ToString());
                myResult = myTest.GetMethod("TearDown").Invoke(myObj, new object[]{});            
                process_log(myResult.ToString());
            }
            catch (Exception ex)
            {
                process_log("Error!!! " + ex.Message);
            // catch (System.IO.FileNotFoundException)
            }
            return;
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
                process_log("<<Step 3>> 依序執行Job_List的PowerShell程式");
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
                        process_log("  Item["+job_index+"] -> 執行"+downloadPath+job);
                        Execute_dll(downloadPath+job);
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
}
