using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        public static void ShowError(Exception exception)
        {
            var thr = new Thread(ShowErrorSync);
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start(exception);

        }

        public static void ShowErrorSync(object e)
        {
            
            var exception = e as Exception;
            /*
            TaskDialogOptions config = new TaskDialogOptions();

            config.Owner = Application.Current.Windows[0];

            config.Title = "Fatal!";
            config.MainInstruction = "Something Went Wrong!!!";

            config.Content = exception.Message;
            config.ExpandedInfo = exception.ToString();
            config.VerificationText = "Don't show me this message again";
            config.CustomButtons = new string[] { "&OK" };
            config.MainIcon = VistaTaskDialogIcon.Error;
            config.FooterText = "Application will close now...";
            config.FooterIcon = VistaTaskDialogIcon.Warning;

            TaskDialogResult res = TaskDialog.Show(config);

            */
            MessageBox.Show(exception.Message);


            //Thread.Sleep(3000);
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //MessageBox.Show(e.ExceptionObject.ToString());
            //return;
            ShowError((Exception)e.ExceptionObject);


        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show(e.Exception.ToString());
            //e.Handled = true;
            ShowError(e.Exception);
        }
    }
}
