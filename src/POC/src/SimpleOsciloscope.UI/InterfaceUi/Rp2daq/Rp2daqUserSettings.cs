using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi
{
    [Serializable]
    public class Rp2daqUserSettings: BaseDeviceUserSettingsData
    {
        public int SampleRate;
        public string ComPortName;
        public int ChannelId;

        public Rp2daqUserSettings()
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ComPortName", ComPortName);
            info.AddValue("SampleRate", SampleRate);
            info.AddValue("ChannelId", ChannelId);
        }

        public Rp2daqUserSettings(SerializationInfo info, StreamingContext context)
        {
            ComPortName = info.GetString("ComPortName");
            SampleRate = info.GetInt32("SampleRate");
            ChannelId = info.GetInt32("ChannelId");
        }
    }
}
