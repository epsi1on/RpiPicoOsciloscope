using FftSharp;
using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi.Rp2daq
{
    [Serializable]
    public class Rp2daqCalibrationData: BaseDeviceCalibrationData
    {
        public double Alpha1, Alpha2, Alpha3;
        public double Beta1, Beta2, Beta3;


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Alpha1", Alpha1);
            info.AddValue("Alpha2", Alpha2);
            info.AddValue("Alpha3", Alpha3);

            info.AddValue("Beta1", Beta1);
            info.AddValue("Beta2", Beta2);
            info.AddValue("Beta3", Beta3);
        }

        public Rp2daqCalibrationData()
        {

        }

        public Rp2daqCalibrationData(SerializationInfo info, StreamingContext context)
        {
            Alpha1 = info.GetDouble("Alpha1");
            Alpha2 = info.GetDouble("Alpha2");
            Alpha3 = info.GetDouble("Alpha3");

            Beta1 = info.GetDouble("Beta1");
            Beta2 = info.GetDouble("Beta2");
            Beta3 = info.GetDouble("Beta3");
        }
    }
}
