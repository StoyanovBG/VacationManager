using System.Collections.Generic;
using System.ComponentModel;
using API.Models.ViewModels.Base;
using API.Models.ViewModels.Projects;

namespace API.Models.ViewModels.Teams
{
    public class TeamsEditVM : BaseEditVM
    {
        public TeamsEditVM()
        {
            this.Projects = new List<ProjectsPair>();
        }

        public string Name { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }
        public ICollection<ProjectsPair> Projects { get; set; }
    }
}