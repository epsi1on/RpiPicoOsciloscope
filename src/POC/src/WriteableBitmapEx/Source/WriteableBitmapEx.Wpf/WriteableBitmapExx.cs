using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace System.Windows.Media.Imaging
{

    //native calls to wpf add dirty rect
    public static class WriteableBitmapExx
    {

        static WriteableBitmapExx()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var mscorlib = assemblies.FirstOrDefault(ii => ii.FullName.Contains("PresentationCore"));

            var all = mscorlib.GetTypes();
            var rr = all.OrderBy(ii => ii.Name).ToArray();

            var c = mscorlib.GetType("MS.Internal.MILSwDoubleBufferedBitmap");
            var m = AddDirtyRect = c.GetMethod("AddDirtyRect", BindingFlags.NonPublic | BindingFlags.Static);

            var c2 = _pDoubleBufferedBitmap= typeof(WriteableBitmap).GetField("_pDoubleBufferedBitmap", BindingFlags.NonPublic |
                         BindingFlags.Instance);

            var sub = SubscribeToCommittingBatch= typeof(WriteableBitmap).GetMethod("SubscribeToCommittingBatch", BindingFlags.NonPublic | BindingFlags.Instance);
        }


        static FieldInfo _pDoubleBufferedBitmap;
        static MethodInfo SubscribeToCommittingBatch;
        static MethodInfo AddDirtyRect;


        public static void AddDirtyRectThreadSafe(this WriteableBitmap bmp, Int32Rect rect)
        {
            var buff = _pDoubleBufferedBitmap.GetValue(bmp);

            AddDirtyRect.Invoke(null, new object[] { buff, rect });
            SubscribeToCommittingBatch.Invoke(bmp, new object[0]);
        }
    }
}
