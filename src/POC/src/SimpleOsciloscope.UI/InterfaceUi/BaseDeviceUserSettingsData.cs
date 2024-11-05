using System;
using System.Runtime.Serialization;

namespace SimpleOsciloscope.UI.InterfaceUi
{
    [Serializable]
    public abstract class BaseDeviceUserSettingsData: ISerializable
    {
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        public abstract int GetAdcSampleRate();
    }
}
