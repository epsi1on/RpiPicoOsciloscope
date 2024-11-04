using SimpleOsciloscope.UI.HardwareInterface;
using SimpleOsciloscope.UI.InterfaceUi.Rp2daq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleOsciloscope.UI.InterfaceUi
{
    public abstract class BaseDeviceInterface
    {
        /// <summary>
        /// shows some UI to user and ask for do something for calibration (like connecting probe to 1khz signal)
        /// and finaly if user did not cancel the procedure, send out calibration data as byte array (like xml etc) for save on disk
        /// and send to next instance
        /// </summary>
        /// <param name="config">the calibration result</param>
        /// <returns></returns>
        public abstract bool TryCalibrate(out BaseDeviceCalibrationData config);

        /// <summary>
        /// calibrationData: output of TryCalibrate
        /// </summary>
        /// <returns></returns>
        public abstract IDaqInterface GenerateDaqInterface(BaseDeviceCalibrationData calibrationData, BaseDeviceUserSettingsData userSettings);

        /// <summary>
        /// config: output of BaseDaqConfigControl.GetConfig()
        /// </summary>
        /// <returns></returns>
        public abstract BaseDaqConfigControl GenerateUiInterface(BaseDeviceUserSettingsData config);

        //for saving settings
        public abstract string GetUid();
        ///for showing in UI
        public abstract string GetName();
        ///for showing in UI
        public abstract string GetDescription();

        //default settings for show to user, like default sample rate etc
        //only called once inmwhole application lifecycle, at very first run where there is not any user made settings
        protected abstract BaseDeviceUserSettingsData GetDefaultUserSettings();

        //default settings for calibration, like coeft for converting ADC integer values to volt
        //only called once inmwhole application lifecycle, at very first run where there is not any user made settings
        protected abstract BaseDeviceCalibrationData GetDefaulCalibration();

        public BaseDeviceUserSettingsData LoadUserSettings()
        {
            var key = GetUid() + "_userSettings";

            var str = SettingsUtil.Load(key);

            BaseDeviceUserSettingsData buf;//

            if (str == null)
            {
                buf = GetDefaultUserSettings();
            }
            else
            {
                buf = SerializationUtil.DeSerialize(str) as BaseDeviceUserSettingsData;
            }
                

            return buf;
        }

        public BaseDeviceCalibrationData LoadCalibrationSettings()
        {
            var key = GetUid() + "_calibration";

            var str = SettingsUtil.Load(key);

            BaseDeviceCalibrationData buf;

            if (str == null)
                buf = GetDefaulCalibration();
            else
                buf = SerializationUtil.DeSerialize(str) as BaseDeviceCalibrationData;

            return buf;
        }

    }
}
