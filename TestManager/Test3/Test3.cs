using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;             //手動加入參考
using System.Management.Automation.Runspaces;   //手動加入參考
using Common;

namespace Test3
{
    public class Test3
    {
        private const string DllName = "Test3";

        public int Setup()
        {
            // common.Setup
            Testflow.Setup(DllName);

            return 31;
        }

        public int Run()
        {
            try
            {
             Runspace runspace = RunspaceFactory.CreateRunspace();
             runspace.Open();
             Pipeline pipeline = runspace.CreatePipeline();
            //  pipeline.Commands.AddScript("c:\\TestManager\\CommonLibs\\RunAs.ps1");             
             pipeline.Commands.AddScript("c:\\TestManager\\CommonLibs\\Info.ps1");
             pipeline.Invoke();
             runspace.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error!!! " + ex.Message);
            }

            // common.Setup
            Testflow.Run("xxx");
            return 32;
        }

        public int UpdateResults()
        {
            Testflow.UpdateResults(DllName, true);
            return 33;
        }

        public int TearDown()
        {
            Testflow.TearDown(DllName);
            return 34;
        }

    }
}
