using System.Collections.Generic;
using System.ComponentModel;
using API.Models.ViewModels.Teams;

namespace API.Models.ViewModels.Projects
{
    public class ProjectsAssignVM
    {
        public int ProjectId { get; set; }

        [DisplayName("Team")]
        public int TeamId { get; set; }

        public ICollection<TeamsPair> Teams { get; set; }
    }
}