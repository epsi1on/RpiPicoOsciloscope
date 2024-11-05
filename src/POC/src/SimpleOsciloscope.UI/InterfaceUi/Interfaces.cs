using SimpleOsciloscope.UI.InterfaceUi.FakeDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi
{
    public static class Interfaces
    {
        public static InterfaceUi.BaseDeviceInterface[] GetInterfaces()
        {
            var buf = new InterfaceUi.BaseDeviceInterface[] { new Rp2DaqInterfaceUi(), new FakeInterfaceUi() };
            return buf;
        }
    }
}
