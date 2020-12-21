using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.Projects
{
    public class ProjectsEditVM : BaseEditVM
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}