using System;
using System.Data.Entity.Migrations;
using System.Linq;
using API.Models;
using System.Web.Mvc;
using API.Models.Filters;
using API.Models.ViewModels.Projects;
using API.Models.ViewModels.Teams;
using Data;
using Data.Entities;

namespace API.Controllers
{
    public class ProjectsController : Controller
    {
        [HttpGet]
        public ActionResult Index(ProjectsIndexVM model)
        {
            if (AuthenticationManager.LoggedUser == null)
                return RedirectToAction("Login", "Home");

            model.Pager = model.Pager ?? new PagerVM();
            model.Pager.Page = model.Pager.Page <= 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;

            model.FilterVm = model.FilterVm ?? new ProjectsFilterVM();

            bool emptyName = string.IsNullOrWhiteSpace(model.FilterVm.Name);
            bool emptyDescription = string.IsNullOrWhiteSpace(model.FilterVm.Description);

            OvmDbContext context = new OvmDbContext();
            IQueryable<Project> query = context.Projects.Where(p =>
                (emptyName || p.Name.Contains(model.FilterVm.Name)) &&
                (emptyDescription || p.Description.Contains(model.FilterVm.Description)));

            model.Pager.PagesCount = (int)Math.Ceiling(query.Count() / (double)model.Pager.ItemsPerPage);

            query = query.OrderBy(p => p.Id).Skip((model.Pager.Page - 1) * model.Pager.ItemsPerPage).Take(model.Pager.ItemsPerPage);

            model.Items = query.Select(p => new ProjectsVM
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                TeamsCount = p.Teams.Count
            }).ToList();

            context.Dispose();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            ProjectsEditVM model;

            if (id == null)
            {
                model = new ProjectsEditVM();
            }
            else
            {
                OvmDbContext context = new OvmDbContext();
                Project project = context.Projects.Find(id.Value);

                model = new ProjectsEditVM
                {
                    Id = id.Value,
                    Name = project.Name,
                    Description = project.Description
                };

                context.Dispose();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProjectsEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OvmDbContext context = new OvmDbContext();
            Project project = new Project
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };

            context.Projects.AddOrUpdate(u => u.Id, project);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Project project = context.Projects.Find(id);
            project.Teams.Clear();
            context.Projects.Remove(project);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Project project = context.Projects.Find(id);

            ProjectsDetailsVM model = new ProjectsDetailsVM
            {
                Id = id,
                Name = project.Name,
                Description = project.Description,
                Teams = project.Teams.Select(t => new TeamsVM
                {
                    Id = t.Id,
                    Name = t.Name,
                    TeamLeadName = t.TeamLead?.Username,
                    TeamSize = t.Developers.Count
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult AssignTeam(int id)
        {
            OvmDbContext context = new OvmDbContext();
            ProjectsAssignVM model = new ProjectsAssignVM
            {
                TeamId = id,
                Teams = context.Teams.Where(t => t.Project == null)
                    .Select(t => new TeamsPair { Id = t.Id, Name = t.Name }).ToList(),
                ProjectId = id
            };

            context.Dispose();

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignTeam(ProjectsAssignVM model)
        {
            OvmDbContext context = new OvmDbContext();
            Team team = context.Teams.Find(model.TeamId);
            team.ProjectId = model.ProjectId;
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Details", "Projects", new { Id = model.ProjectId });
        }
    }
}