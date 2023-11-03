using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TestItem_all
{
    public class TestItem_all
    {
        private const string DllName = "TestItem_all";
        private const string dllDir = "c:\\TestManager\\Download\\";

        public int Setup()
        {
            // common.Setup
            //Testflow.Setup(DllName);

            return 81;
        }

        public int Run()
        {
            try
            {
                // Common.Program.process_log("Run TestItem1.dll....");
                Common.Runnner.RunTestItem(dllDir+"TestItem1.dll");
                // Common.Program.process_log("Run TestItem2.dll....");
                Common.Runnner.RunTestItem(dllDir+"TestItem2.dll");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test all " + ex.Message);
            }
            return 82;
        }

        public int UpdateResults()
        {
            //Testflow.UpdateResults(DllName, true);

            // Return the test results from 'Run'
            return 83;
        }

        public int TearDown()
        {
            //Testflow.TearDown(DllName);
            return 84;
        }
    }
}
