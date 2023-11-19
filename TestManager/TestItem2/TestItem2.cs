using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Management.Automation;             //手動加入參考
using System.Management.Automation.Runspaces;   //手動加入參考

namespace TestItem2
{
    public class TestItem2
    {
        private const string DllName = "TestItem2";

        public int Setup()
        {
            // common.Setup
            Testflow.Setup(DllName);

            return 21;
        }

        public int Run()
        {
            try
            {
             Runspace runspace = RunspaceFactory.CreateRunspace();
             runspace.Open();
             Pipeline pipeline = runspace.CreatePipeline();
            //  pipeline.Commands.AddScript("c:\\TestManager\\CommonLibs\\RunAs.ps1");             
             pipeline.Commands.AddScript("c:\\TestManager\\CommonLibs\\Abt1.ps1");
             pipeline.Invoke();
             runspace.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error!!! " + ex.Message);
            }

            // common.Setup
            Testflow.Run("TestItem2");
            return 22;
        }

        public int UpdateResults()
        {
            Testflow.UpdateResults(DllName, true);
            return 23;
        }

        public int TearDown()
        {
            Testflow.TearDown(DllName);
            return 24;
        }

    }
}
