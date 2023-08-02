using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleOsciloscope.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //start DAQ thread

            var ifs = new DaqInterface();
            ifs.TargetRepository = UiState.Instance.CurrentRepo;
            var thr = new Thread(ifs.StartSync);
            thr.Start();

        }
    }
}
