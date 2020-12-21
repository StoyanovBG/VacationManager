using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using API.Models;
using API.Models.Filters;
using API.Models.ViewModels.Roles;
using API.Models.ViewModels.Users;
using Data;
using Data.Entities;

namespace API.Controllers
{
    public class RolesController : Controller
    {
        [HttpGet]
        public ActionResult Index(RolesIndexVM model)
        {
            if (AuthenticationManager.LoggedUser == null)
                return RedirectToAction("Login", "Home");

            model.Pager = model.Pager ?? new PagerVM();
            model.Pager.Page = model.Pager.Page <= 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;

            model.FilterVm = model.FilterVm ?? new RolesFilterVM();

            bool emptyName = string.IsNullOrWhiteSpace(model.FilterVm.Name);

            OvmDbContext context = new OvmDbContext();
            IQueryable<Role> query = context.Roles.Where(r => emptyName || r.Name.Contains(model.FilterVm.Name));

            model.Pager.PagesCount = (int)Math.Ceiling(query.Count() / (double)model.Pager.ItemsPerPage);

            query = query.OrderBy(r => r.Id).Skip((model.Pager.Page - 1) * model.Pager.ItemsPerPage).Take(model.Pager.ItemsPerPage);

            model.Items = query.Select(r => new RolesVM
            {
                Id = r.Id,
                Name = r.Name,
                AssignedUsersCount = r.Users.Count
            }).ToList();

            context.Dispose();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            RolesEditVM item;
            OvmDbContext context = new OvmDbContext();

            if (id == null)
            {
                item = new RolesEditVM();
            }
            else
            {
                Role role = context.Roles.Find(id.Value);

                item = new RolesEditVM
                {
                    Id = id.Value,
                    Name = role.Name
                };
            }

            context.Dispose();

            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(RolesEditVM model)
        {
            OvmDbContext context = new OvmDbContext();

            Role role = new Role
            {
                Id = model.Id,
                Name = model.Name
            };

            context.Roles.AddOrUpdate(u => u.Id, role);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Role role = context.Roles.Find(id);
            Role unassignedRole = context.Roles.FirstOrDefault(r => r.Name == "Unassigned");

            foreach (User assignedUser in role.Users)
            {
                assignedUser.Role = unassignedRole ?? new Role("Unassigned");
            }

            context.Roles.Remove(role);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            OvmDbContext context = new OvmDbContext();
            Role role = context.Roles.Find(id);

            RolesDetailsVM model = new RolesDetailsVM
            {
                Name = role.Name,
                Users = role.Users.Select(u => new UsersVM
                {
                    Id = u.Id,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                }).ToList()
            };

            return View(model);
        }
    }
}