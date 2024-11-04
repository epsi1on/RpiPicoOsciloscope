using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi
{
    [Serializable]
    public abstract class BaseDeviceCalibrationData: ISerializable
    {
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    }

    [Serializable]
    public abstract class BaseDeviceUserSettingsData: ISerializable
    {
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}
