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
        public double AlphaA1, AlphaA2;
        public double AlphaB1, AlphaB2;
        public double AlphaC1, AlphaC2;

        public double BetaA1, BetaA2;
        public double BetaB1, BetaB2;
        public double BetaC1, BetaC2;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AlphaA1", AlphaA1);
            info.AddValue("AlphaA2", AlphaA2);

            info.AddValue("AlphaB1", AlphaB1);
            info.AddValue("AlphaB2", AlphaB2);

            info.AddValue("AlphaC1", AlphaC1);
            info.AddValue("AlphaC2", AlphaC2);

            info.AddValue("BetaA1", BetaA1);
            info.AddValue("BetaA2", BetaA2);

            info.AddValue("BetaB1", BetaB1);
            info.AddValue("BetaB2", BetaB2);

            info.AddValue("BetaC1", BetaC1);
            info.AddValue("BetaC2", BetaC2);
        }

        public Rp2daqCalibrationData()
        {

        }

        public Rp2daqCalibrationData(SerializationInfo info, StreamingContext context)
        {
            AlphaA1 = info.GetDouble("AlphaA1");
            AlphaA2 = info.GetDouble("AlphaA2");

            AlphaB1 = info.GetDouble("AlphaB1");
            AlphaB2 = info.GetDouble("AlphaB2");

            AlphaC1 = info.GetDouble("AlphaC1");
            AlphaC2 = info.GetDouble("AlphaC2");

            BetaA1 = info.GetDouble("BetaA1");
            BetaA2 = info.GetDouble("BetaA2");

            BetaB1 = info.GetDouble("BetaB1");
            BetaB2 = info.GetDouble("BetaB2");

            BetaC1 = info.GetDouble("BetaC1");
            BetaC2 = info.GetDouble("BetaC2");
        }
    }
}
