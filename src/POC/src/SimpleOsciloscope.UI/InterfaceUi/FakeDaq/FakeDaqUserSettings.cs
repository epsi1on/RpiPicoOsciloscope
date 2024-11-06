using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi.FakeDaq
{
    [Serializable]
    public class FakeDaqUserSettings : BaseDeviceUserSettingsData
    {
        public int SampleRate { get; set; }

        public double Frequency { get; set; }

        public double Offset { get; set; }

        public double Amplitude { get; set; }

        public double Noise { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(SampleRate), SampleRate);
            info.AddValue(nameof(Frequency), Frequency);
            info.AddValue(nameof(Offset), Offset);
            info.AddValue(nameof(Amplitude), Amplitude);
            info.AddValue(nameof(Noise), Noise);
        }

        public override int GetAdcSampleRate()
        {
            return SampleRate;
        }

        public FakeDaqUserSettings()
        {

        }
        public FakeDaqUserSettings(SerializationInfo info, StreamingContext context)
        {
            SampleRate = info.GetInt32("SampleRate");
            Frequency = info.GetDouble("Frequency");
            Offset = info.GetDouble("Offset");
            Amplitude = info.GetDouble("Amplitude");
            Noise = info.GetDouble("Noise");
        }


    }
}
