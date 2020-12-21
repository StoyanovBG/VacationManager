using API.Models.Filters;
using API.Models.ViewModels.Base;
using System.Collections.Generic;

namespace API.Models.ViewModels.Teams
{
    public class TeamsIndexVM : BaseIndexVM
    {
        public List<TeamsVM> Items { get; set; }

        public TeamsFilterVM FilterVm { get; set; }
    }
}