using System;
using System.ComponentModel;
using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.TimeOffs
{
    public class TimeOffsEditVM : BaseEditVM
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        [DisplayName("Half day")]
        public bool IsHalfDay { get; set; }
    }
}