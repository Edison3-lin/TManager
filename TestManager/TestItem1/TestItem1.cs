﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Diagnostics;

namespace TestItem1
{
    public class TestItem1
    {

        public int Setup()
        {
            // common.Setup
            Console.WriteLine("Test1 Setup........");            
            Testflow.Setup("TestItem1");

            return 11;
        }

        public int Run()
        {



        string exeFilePath = "c:\\TestManager\\Download\\pwrtest.exe";

        // Create a ProcessStartInfo object with the file path
        ProcessStartInfo startInfo = new ProcessStartInfo(exeFilePath);

        // Optionally, you can set working directory, arguments, and other properties
        startInfo.WorkingDirectory = "c:\\TestManager\\Download\\";
        startInfo.Arguments = "/sleep /c:1 /s:3 /d:30 /p:40";

        // Start the process
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        // Optionally, you can wait for the process to exit
        process.WaitForExit();


            // common.Setup
            Testflow.Run("TestItem1");
            Console.WriteLine("Test1 Run........");            
            return 12;
        }

        public int UpdateResults()
        {
            Testflow.UpdateResults("TestItem1", true);
            Console.WriteLine("Test1 UpdateResults........");            
            return 13;
        }

        public int TearDown()
        {
            Testflow.TearDown("TestItem1");
            Console.WriteLine("Test1 TearDown........");            
            return 14;
        }
    }
}
