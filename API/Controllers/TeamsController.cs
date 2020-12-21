using API.Models;
using API.Models.Filters;
using API.Models.ViewModels.Teams;
using API.Models.ViewModels.Users;
using Data;
using Data.Entities;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using API.Models.ViewModels.Projects;

namespace API.Controllers
{
    public class TeamsController : Controller
    {
        [HttpGet]
        public ActionResult Index(TeamsIndexVM model)
        {
            if (AuthenticationManager.LoggedUser == null)
                return RedirectToAction("Login", "Home");

            model.Pager = model.Pager ?? new PagerVM();
            model.Pager.Page = model.Pager.Page <= 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;

            model.FilterVm = model.FilterVm ?? new TeamsFilterVM();

            bool emptyProjectName = string.IsNullOrWhiteSpace(model.FilterVm.ProjectName);
            bool emptyTeamName = string.IsNullOrWhiteSpace(model.FilterVm.TeamName);

            OvmDbContext context = new OvmDbContext();

            IQueryable<Team> query = context.Teams.Where(t =>
                (emptyProjectName || t.Project.Name.Contains(model.FilterVm.ProjectName)) &&
                (emptyTeamName || t.Name.Contains(model.FilterVm.TeamName)));

            model.Pager.PagesCount = (int)Math.Ceiling(query.Count() / (double)(model.Pager.ItemsPerPage));

            query = query.OrderBy(t => t.Id).Skip((model.Pager.Page - 1) * model.Pager.ItemsPerPage).Take(model.Pager.ItemsPerPage);
            model.Items = query.Select(t => new TeamsVM
            {
                Id = t.Id,
                Name = t.Name,
                ProjectPair = t.Project != null ? new ProjectsPair
                {
                    Id = t.ProjectId.Value,
                    Name = t.Project.Name
                } : null,
                TeamLeadName = t.TeamLead.Username ?? string.Empty,
                TeamSize = t.Developers.Count
            }).ToList();

            context.Dispose();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            TeamsEditVM item;
            OvmDbContext context = new OvmDbContext();

            if (id == null)
            {
                item = new TeamsEditVM();
            }
            else
            {

                Team team = context.Teams.Find(id.Value);

                item = new TeamsEditVM
                {
                    Id = id.Value,
                    Name = team.Name,
                    ProjectId = team.ProjectId ?? 0
                };
            }

            item.Projects = context.Projects.Select(p => new ProjectsPair
            {
                Name = p.Name,
                Id = p.Id
            }).ToList();

            context.Dispose();

            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(TeamsEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OvmDbContext context = new OvmDbContext();

            Team team = new Team();
            team.Id = model.Id;
            team.Name = model.Name;

            if (model.ProjectId != 0)
                team.ProjectId = model.ProjectId;

            context.Teams.AddOrUpdate(t => t.Id, team);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Team team = context.Teams.Find(id);
            team.Developers.Clear();
            context.Teams.Remove(team);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult AssignTeamLead(int id)
        {
            OvmDbContext context = new OvmDbContext();
            TeamsAssignVM model = new TeamsAssignVM
            {
                Id = id,
                Users = context.Users.Where(u => u.Role.Name == "Team Lead" && u.LedTeams.FirstOrDefault(t => t.Id == id) == null)
                    .Select(u => new UsersPair { Id = u.Id, Username = u.Username })
                    .ToList()
            };

            context.Dispose();

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignTeamLead(TeamsAssignVM model)
        {
            OvmDbContext context = new OvmDbContext();
            Team team = context.Teams.Find(model.Id);
            team.TeamLeadId = model.UserId;

            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Details", "Teams", new { Id = model.Id });
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Team team = context.Teams.Find(id);

            TeamsDetailsVM model = new TeamsDetailsVM
            {
                Id = id,
                Name = team.Name,
                Project = team.Project == null
                    ? null
                    : new ProjectsPair
                    {
                        Name = team.Project.Name,
                        Id = team.Project.Id
                    },
                Developers = team.Developers.Select(u => new UsersVM
                {
                    Id = u.Id,
                    Username = u.Username,
                    RoleName = u.Role.Name,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                }).ToList(),
                TeamLead = team.TeamLead != null
                    ? new UsersPair
                    {
                        Id = team.TeamLead.Id,
                        Username = team.TeamLead.Username
                    }
                    : null
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult RemoveDeveloper(int teamId, int id)
        {
            OvmDbContext context = new OvmDbContext();
            User user = context.Users.Find(id);
            user.TeamId = null;
            user.Team = null;
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Details", "Teams", new { Id = teamId });
        }

        [HttpGet]
        public ActionResult AssignDeveloper(int id)
        {
            OvmDbContext context = new OvmDbContext();
            TeamsAssignVM model = new TeamsAssignVM
            {
                Users = context.Users.Where(u => u.Role.Name == "Developer" && u.Team == null).Select(u =>
                    new UsersPair() { Id = u.Id, Username = u.Username }).ToList()
            };

            context.Dispose();

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignDeveloper(TeamsAssignVM model)
        {
            OvmDbContext context = new OvmDbContext();
            User user = context.Users.Find(model.UserId);
            user.TeamId = model.Id;

            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Details", "Teams", new { Id = model.Id });
        }

        [HttpGet]
        public ActionResult RemoveTeamLead(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Team team = context.Teams.Find(id);
            team.TeamLead = null;
            team.TeamLeadId = null;
            context.SaveChanges();
            context.Dispose(); ;

            return RedirectToAction("Details", "Teams", new { Id = id });
        }

        [HttpGet]
        public ActionResult RemoveProject(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Team team = context.Teams.Find(id);
            team.Project = null;
            team.ProjectId = null;
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Details", "Teams", new { Id = id });
        }
    }
}