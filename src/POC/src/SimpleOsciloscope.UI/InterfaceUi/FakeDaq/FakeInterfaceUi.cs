using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi.FakeDaq
{
    public class FakeInterfaceUi : BaseDeviceInterface
    {
        public override IDaqInterface GenerateDaqInterface(BaseDeviceCalibrationData calibrationData, BaseDeviceUserSettingsData userSettings)
        {
            var buf = new FakeDaqInterface();

            buf.UserSettings = (FakeDaqUserSettings)userSettings;
            buf.CalibrationData = (FakeDaqCalibrationData)calibrationData;

            return buf;
        }

        public override BaseDaqConfigGUIControl GenerateUiInterface(BaseDeviceUserSettingsData config)
        {
            var buf = new FakeDaqControl();

            buf.SetDefaultUserSettings(config);

            return buf;
        }

        public override string GetDescription()
        {
            return "Builtin Fake ADC mock";
        }

        public override string GetName()
        {
            return "Fake ADC";
        }

        public override string GetUid()
        {
            return "fake_builtin_1564";
        }

        public override bool TryCalibrate(out BaseDeviceCalibrationData config)
        {
            throw new NotImplementedException();
        }

        protected override BaseDeviceCalibrationData GetDefaulCalibration()
        {
            return null;
        }

        protected override BaseDeviceUserSettingsData GetDefaultUserSettings()
        {
            var set = new FakeDaqUserSettings();

            set.SampleRate = 1000_000;
            set.Amplitude = 2;
            set.Offset = 1;
            set.Frequency = 10_000;

            return set;
        }
    }
}
