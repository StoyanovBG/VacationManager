using System;
using System.ServiceModel.PeerResolvers;
using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.TimeOffs
{
    public class TimeOffsDetailsVM : BaseDetailsVM
    {
        public DateTime LastChangedOn { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public bool IsApproved { get; set; }

        public bool IsHalfDay { get; set; }
    }
}