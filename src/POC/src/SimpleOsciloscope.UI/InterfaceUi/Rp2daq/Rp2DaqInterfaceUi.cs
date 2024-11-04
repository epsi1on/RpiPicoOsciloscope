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
            var buf = new RpiPicoDaqInterface(userSettings, calibrationData);

            return buf;
            
            throw new NotImplementedException();
        }

        public override BaseDaqConfigGUIControl GenerateUiInterface(BaseDeviceUserSettingsData config)
        {
            var buf = new Rp2DaqInterfaceControl();

            buf.Init();
            buf.SetDefaultUserSettings(config);

            return buf;
        }

        protected override BaseDeviceCalibrationData GetDefaulCalibration()
        {
            var buf = new Rp2daqCalibrationData();

            buf.AlphaA1 = buf.AlphaA2 = 1.0 / 4096;
            buf.AlphaB1 = buf.AlphaB2 = 1.0 / 4096;
            buf.AlphaC1 = buf.AlphaC2 = 1.0 / 4096;

            buf.BetaA1 = buf.BetaB1 = buf.BetaC1 = 0;
            buf.BetaA2 = buf.BetaB2 = buf.BetaC2 = 0;

            return buf;
        }

        protected override BaseDeviceUserSettingsData GetDefaultUserSettings()
        {
            var set = new Rp2daqUserSettings();

            set.SampleRate = 500_000;
            set.ChannelId =  RpiPicoDaqInterface.Rp2040AdcChannels.Gpio27;
            set.BitWidth = 12;

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
