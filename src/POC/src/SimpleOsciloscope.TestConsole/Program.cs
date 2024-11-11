using SimpleOsciloscope.UI;
using SimpleOsciloscope.UI.FrequencyDetection;
using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.Remoting.Channels;
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;

namespace SimpleOsciloscope.TestConsole
{
   

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CopyTest.Test();
            Console.ReadKey();


        }




    }
}
