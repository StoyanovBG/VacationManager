using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.Projects
{
    public class ProjectsVM : BaseVM
    {
        public int TeamsCount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}