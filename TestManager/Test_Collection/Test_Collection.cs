using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Test_Collection
{
    public class Test_Collection
    {
        private const string DllName = "Test_Collection";
        private const string LibsPath = "c:\\TestManager\\CommonLibs\\";

        public int Setup()
        {
            // common.Setup
            Testflow.Setup(DllName);

            return 81;
        }

        public int Run()
        {
            // Testflow.Run(DllName);

            try
            {
                Common.Runnner.RunTestItem(LibsPath+"TestItem1.dll");
                Common.Runnner.RunTestItem(LibsPath+"Test3.dll");
                Common.Runnner.RunTestItem(LibsPath+"TestItem2.dll");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Test all " + ex.Message);
            }
            return 82;
        }

        public int UpdateResults()
        {
            Testflow.UpdateResults(DllName, true);

            // Return the test results from 'Run'
            return 83;
        }

        public int TearDown()
        {
            Testflow.TearDown(DllName);
            return 84;
        }
    }
}
