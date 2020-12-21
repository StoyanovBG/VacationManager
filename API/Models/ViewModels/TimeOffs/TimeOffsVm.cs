using System;
using API.Models.ViewModels.Base;
using API.Models.ViewModels.Users;

namespace API.Models.ViewModels.TimeOffs
{
    public class TimeOffsVM : BaseVM
    {
        public DateTime CreatedOn { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public UsersPair Requestor { get; set; }

        public bool IsApproved { get; set; }

        public bool IsHalfDay { get; set; }
    }
}