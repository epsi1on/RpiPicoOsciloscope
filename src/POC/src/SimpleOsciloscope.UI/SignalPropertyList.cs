using FftSharp;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SimpleOsciloscope.UI
{
    public class SignalPropertyList
    {
        public double alpha;
        public double beta;

        public short Max { get; set; }
        public short Min { get; set; }
        public double Avg { get; set; }
        public short Domain{ get; set; }

        public short MinPercentile1 { get; set; }
        public short MaxPercentile1 { get; set; }

        public short Percentile1Domain { get; set; }
        public short MinPercentile5 { get; set; }
        public short MaxPercentile5 { get; set; }
        public short Percentile5Domain { get; set; }

        public double Frequency { get; set; }
        public double PhaseRadian { get; set; }

        public double PwmDutyCycle { get; set; }

        public bool Error { get; set; }

        
    }
}
