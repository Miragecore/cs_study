using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestForJson
{

    using Json_Util;
    class Program
    {
        static void Main(string[] args)
        {
            AutoPwrCtrlSetting apcs = new AutoPwrCtrlSetting();

            apcs.Load();

            System.Console.WriteLine(apcs.strTargetIp);

            apcs.Save();
        }
    }
}
