using API.Models.ViewModels.Base;
using System.ComponentModel;
using API.Models.ViewModels.Projects;

namespace API.Models.ViewModels.Teams
{
    public class TeamsVM : BaseVM
    {
        public string Name { get; set; }

        public ProjectsPair ProjectPair { get; set; }

        [DisplayName("Team lead name")]
        public string TeamLeadName { get; set; }

        [DisplayName("Team size")]
        public int TeamSize { get; set; }
    }
}