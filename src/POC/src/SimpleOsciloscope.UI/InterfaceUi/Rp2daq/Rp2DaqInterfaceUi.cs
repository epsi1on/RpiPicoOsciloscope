using SimpleOsciloscope.UI.HardwareInterface;
using SimpleOsciloscope.UI.InterfaceUi.Rp2daq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi
{
    public class Rp2DaqInterfaceUi : BaseDeviceInterface
    {
        public override IDaqInterface GenerateDaqInterface(BaseDeviceCalibrationData calibrationData, BaseDeviceUserSettingsData userSettings)
        {
            throw new NotImplementedException();
        }

        public override BaseDaqConfigControl GenerateUiInterface(BaseDeviceUserSettingsData config)
        {
            var buf = new Rp2DaqInterfaceControl();

            buf.Init();
            buf.SetDefaultUserSettings(config);

            return buf;
        }

        protected override BaseDeviceCalibrationData GetDefaulCalibration()
        {
            var buf = new Rp2daqCalibrationData();

            buf.Alpha1 = buf.Alpha2 = buf.Alpha3 = 1.0 / 4096;
            buf.Beta1 = buf.Beta2 = buf.Beta3 = 0;

            return buf;
        }

        protected override BaseDeviceUserSettingsData GetDefaultUserSettings()
        {
            var set = new Rp2daqUserSettings();

            set.SampleRate = 500_000;
            set.ChannelId = 1;

            return set;
        }

        public override string GetDescription()
        {
            return "rp2daq on RPi Pico";
        }

        public override string GetName()
        {
            return "rp2daq";
        }

        public override bool TryCalibrate(out BaseDeviceCalibrationData config)
        {
            throw new NotImplementedException();
        }

        public override string GetUid()
        {
            return "rp2daq_5548";//5548 just a random
        }
    }
}
