namespace SimpleOsciloscope.UI.HardwareInterface
{
    public interface IDaqInterface
    {
        void StartSync();

        DataRepository TargetRepository { get; set; }

        int AdcResolutionBits { get; }//resulution in bits (8 or 10 or 12)

        double AdcMaxVoltage { get; }// for example for a 8 bit Resolution, (ResolutionBits = 8), max value is 256. 256 equal to AdcMaxVoltage.

        long AdcSampleRate { get; }


        void StopAdc();


        void DisConnect();
    }
}
