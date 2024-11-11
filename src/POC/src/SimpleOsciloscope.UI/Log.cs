using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public static class Log
    {
        public static void Info(string format,params object[] args)
        {
            var msg = string.Format(format, args);

            Console.Write("info: ");
            Console.WriteLine(msg);
        }
    }
}
