using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.HardwareInterface
{
    public class CalibrationUtil
    {
        public double V1;//adc readout (0-4096) averaged where adc pin directly connected to adc_vref and no resistor there
        public double V2;//adc readout (0-4096) averaged where adc pin directly connected to adc_gnd and no resistor there

        public double V3;//adc readout where a single Rin is between ADC pin and adc_vref
        public double V4;//adc readout where a single Rin is between ADC pin and adc_gnd

        public double Rin;//the input resistor

        public double V5;//adc readout where a single Rin is between ADC pin and adc_vref
        public double V6;//adc readout where a single Rin is between ADC pin and adc_gnd

        public double Ru, Rd;//voltage divider resistors


        public void CalibrateAdcInternals()
        {
            //only v1,v2,v3,v4 is used
        }

    }
}
