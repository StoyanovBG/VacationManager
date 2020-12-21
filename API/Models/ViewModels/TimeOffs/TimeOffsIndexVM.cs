using System.Collections.Generic;
using API.Models.Filters;
using API.Models.ViewModels.TimeOffs.PaidTimeOffs;
using API.Models.ViewModels.TimeOffs.SickTimeOff;
using API.Models.ViewModels.TimeOffs.UnpaidTimeOffs;

namespace API.Models.ViewModels.TimeOffs
{
    public class TimeOffsIndexVM
    {
        public TimeOffsFilterVM PaidTimeOffsFilter { get; set; }

        public TimeOffsFilterVM UnpaidTimeOffsFilter { get; set; }

        public TimeOffsFilterVM SickTimeOffsFilter { get; set; }

        public PagerVM PaidTimeOffsPager { get; set; }

        public PagerVM UnpaidTimeOffsPager { get; set; }

        public PagerVM SickTimeOffsPager { get; set; }

        public List<PaidTimeOffsVM> PaidTimeOffs { get; set; }

        public List<UnpaidTimeOffsVM> UnpaidTimeOffs { get; set; }

        public List<SickTimeOffsVM> SickTimeOffs { get; set; }
    }
}