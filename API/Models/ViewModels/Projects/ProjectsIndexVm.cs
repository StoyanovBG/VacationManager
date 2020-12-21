using API.Models.Filters;
using API.Models.ViewModels.Base;
using System.Collections.Generic;

namespace API.Models.ViewModels.Projects
{
    public class ProjectsIndexVM : BaseIndexVM
    {
        public List<ProjectsVM> Items { get; set; }

        public ProjectsFilterVM FilterVm { get; set; }
    }
}