using System;
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
        private const string DllName = "TestItem1";

        public int Setup()
        {
            // common.Setup
            Testflow.Setup(DllName);
            return 11;
        }

        public int Run()
        {



        string exeFilePath = "c:\\TestManager\\CommonLibs\\pwrtest.exe";

        // Create a ProcessStartInfo object with the file path
        ProcessStartInfo startInfo = new ProcessStartInfo(exeFilePath);

        // Optionally, you can set working directory, arguments, and other properties
        startInfo.WorkingDirectory = "c:\\TestManager\\CommonLibs\\";
        startInfo.Arguments = "/info";

        // Start the process
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        // Optionally, you can wait for the process to exit
        process.WaitForExit();


            // common.Setup
            Testflow.Run("yyy");
            return 12;
        }

        public int UpdateResults()
        {
            Testflow.UpdateResults(DllName, true);
            return 13;
        }

        public int TearDown()
        {
            Testflow.TearDown(DllName);
            return 14;
        }
    }
}
