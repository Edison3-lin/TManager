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

        public int Setup()
        {
            // common.Setup
            Console.WriteLine("Item2 Setup........");            
            // Testflow.Setup(DllName);

            return 21;
        }

        public int Run()
        {
            try
            {
             Runspace runspace = RunspaceFactory.CreateRunspace();
             runspace.Open();
             Pipeline pipeline = runspace.CreatePipeline();
            //  pipeline.Commands.AddScript("c:\\TestManager\\Common\\RunAs.ps1");             
             pipeline.Commands.AddScript("c:\\TestManager\\Common\\S4.ps1");
             pipeline.Invoke();
             runspace.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error!!! " + ex.Message);
            }

            // common.Setup
            // Testflow.Run(DllName);
            Console.WriteLine("Item2 Run........");            
            return 22;
        }

        public int UpdateResults()
        {
            // Testflow.UpdateResults(DllName, true);
            Console.WriteLine("Item2 UpdateResults........");            
            return 23;
        }

        public int TearDown()
        {
            // Testflow.TearDown(DllName);
            Console.WriteLine("Item2 TearDown........");            
            return 24;
        }

    }
}
