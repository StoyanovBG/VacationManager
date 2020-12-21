using System.Collections.Generic;
using API.Models.ViewModels.Base;
using API.Models.ViewModels.Teams;

namespace API.Models.ViewModels.Projects
{
    public class ProjectsDetailsVM : BaseDetailsVM
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<TeamsVM> Teams { get; set; }
    }
}