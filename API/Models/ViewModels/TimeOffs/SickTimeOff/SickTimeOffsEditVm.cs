using System.Web;

namespace API.Models.ViewModels.TimeOffs.SickTimeOff
{
    public class SickTimeOffsEditVM : TimeOffsEditVM
    {
        public string AttachmentPath { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }
}