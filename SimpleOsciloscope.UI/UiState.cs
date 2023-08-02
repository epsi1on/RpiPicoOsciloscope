using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public class UiState
    {
        public DataRepository CurrentRepo;

        public static UiState Instance = new UiState();

        private UiState()
        {
            CurrentRepo = new DataRepository();
            CurrentRepo.Channels[0] = new FixedLengthList<short>(DataRepository.RepoLength);
        }

    }
}
