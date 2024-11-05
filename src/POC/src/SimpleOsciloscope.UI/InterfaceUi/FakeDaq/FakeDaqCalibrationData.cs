using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi.FakeDaq
{
    [Serializable]
    public class FakeDaqCalibrationData : BaseDeviceCalibrationData
    {
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }
    }
}
