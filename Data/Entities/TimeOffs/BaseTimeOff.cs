using System;

namespace Data.Entities.TimeOffs
{
    public class BaseTimeOff : BaseEntity
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual bool IsHalfDay { get; set; }

        public bool IsApproved { get; set; }

        public int RequestorId { get; set; }
        public virtual User Requestor { get; set; }
    }
}
