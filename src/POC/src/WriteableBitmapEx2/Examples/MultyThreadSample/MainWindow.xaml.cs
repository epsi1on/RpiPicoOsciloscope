using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultyThreadSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            img.Source = bmp = BitmapFactory.New(500, 500);

            bmp.Clear();

            Context = bmp.GetBitmapContext();


            new Thread(Draw).Start();


        }

        WriteableBitmap bmp;
        BitmapContext Context;


        private static void AddDirtyRect(WriteableBitmap bmp)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var mscorlib = assemblies.FirstOrDefault(ii => ii.FullName.Contains("PresentationCore"));

            var all = mscorlib.GetTypes();
            var rr = all.OrderBy(ii => ii.Name).ToArray();

            var c = mscorlib.GetType("MS.Internal.MILSwDoubleBufferedBitmap");
            var c2 = typeof(WriteableBitmap).GetField("_pDoubleBufferedBitmap", BindingFlags.NonPublic |
                         BindingFlags.Instance);

            var sub = typeof(WriteableBitmap).GetMethod("SubscribeToCommittingBatch", BindingFlags.NonPublic | BindingFlags.Instance);
            var psc = typeof(WriteableBitmap).GetMethod("WritePostscript");

            //();
            var c2val = c2.GetValue(bmp);

            var m = c.GetMethod("AddDirtyRect", BindingFlags.NonPublic | BindingFlags.Static);

            var rect = new Int32Rect(0, 0, 500, 500);

            m.Invoke(null, new object[] { c2val, rect });

            sub.Invoke(bmp,new object[0]);
            //bmp.AddDirtyRect
        }


        public void Draw()
        {
            var cnt = 0;

            var sc = 1;

            var doNotSleep = true;

            var colors = new Color[] { Colors.DarkGray, Colors.LightGray };
            var colCnt = 0;
            
            while (true)
            {
                cnt += sc;

                if (cnt == 500)
                {
                    cnt = 0;
                    colCnt++;
                }

                var col = colors[colCnt % 2];

                

                Context.DrawLineAa(cnt, 50, 500 - cnt, 450, col);

                bmp.AddDirtyRectThreadSafe(new Int32Rect(0, 0, 250, 500));

                if (!doNotSleep)
                    Thread.Sleep(sc);
            }
        }
    }
}
