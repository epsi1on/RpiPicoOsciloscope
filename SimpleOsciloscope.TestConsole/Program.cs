using SimpleOsciloscope.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleOsciloscope.TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ifs = new DaqInterface();

            ifs.TargetRepository = UiState.Instance.CurrentRepo;

            ifs.StartSync();

        }
    }
}
