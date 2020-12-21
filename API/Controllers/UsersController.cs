using API.Models;
using API.Models.Filters;
using API.Models.ViewModels.Users;
using Data;
using Data.Entities;
using System;
using System.Linq;
using System.Web.Mvc;
using API.Models.ViewModels.Roles;
using API.Models.ViewModels.Teams;
using Data.Repository;
using AuthenticationManager = API.Models.AuthenticationManager;

namespace API.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserRepository _userRepository;

        public UsersController()
        {
            this._userRepository = new UserRepository(new OvmDbContext());
        }

        [HttpGet]
        public ActionResult Index(UsersIndexVM model)
        {
            if (AuthenticationManager.LoggedUser == null)
                return RedirectToAction("Login", "Home");

            model.Pager = model.Pager ?? new PagerVM();
            model.Pager.Page = model.Pager.Page <= 0 ? 1 : model.Pager.Page;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;

            model.Filter = model.Filter ?? new UsersFilterVM();

            bool emptyUsername = string.IsNullOrWhiteSpace(model.Filter.Username);
            bool emptyFirstName = string.IsNullOrWhiteSpace(model.Filter.FirstName);
            bool emptyLastName = string.IsNullOrWhiteSpace(model.Filter.LastName);
            bool emptyRoleName = string.IsNullOrWhiteSpace(model.Filter.RoleName);

            IQueryable<User> query = this._userRepository.GetAll(u =>
                (emptyUsername || u.Username.Contains(model.Filter.Username)) &&
                (emptyFirstName || u.FirstName.Contains(model.Filter.FirstName)) &&
                (emptyLastName || u.LastName.Contains(model.Filter.LastName)) &&
                (emptyRoleName || u.Role.Name.Contains(model.Filter.RoleName)));

            model.Pager.PagesCount = (int)Math.Ceiling(query.Count() / (double)model.Pager.ItemsPerPage);

            query = query.OrderBy(u => u.Id).Skip((model.Pager.Page - 1) * model.Pager.ItemsPerPage).Take(model.Pager.ItemsPerPage);

            model.Items = query.Select(u => new UsersVM
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                RoleName = u.Role.Name
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            UsersEditVM item;

            if (id == null)
            {
                item = new UsersEditVM();
            }
            else
            {
                User user = this._userRepository.GetOne(id.Value);

                item = new UsersEditVM
                {
                    Id = id.Value,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Password = user.Password,
                    RoleId = user.RoleId,
                };
            }

            OvmDbContext context = new OvmDbContext();
            item.Roles = context.Roles.Select(r => new RolesPair
            {
                Name = r.Name,
                Id = r.Id
            }).ToList();

            context.Dispose();

            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(UsersEditVM model)
        {
            User user = new User
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Password = model.Password,
                RoleId = model.RoleId
            };

            if (user.Id > 0)
                this._userRepository.Update(user);
            else this._userRepository.Add(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            User user = this._userRepository.GetOne(id);
            this._userRepository.Remove(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            User user = this._userRepository.GetOne(id);

            UsersDetailsVM model = new UsersDetailsVM
            {
                Id = id,
                RoleName = user.Role.Name,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LedTeams = user.LedTeams.Count,
                TeamName = user.Team == null ? "The user isn't assigned to any team" : user.Team.Name,
                Username = user.Username
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Assign(int id)
        {
            OvmDbContext context = new OvmDbContext();

            UsersAssignVM model = new UsersAssignVM
            {
                Id = id,
                Teams = context.Teams.Select(t => new TeamsPair { Name = t.Name, Id = t.Id }).ToList()
            };

            context.Dispose();

            return View(model);
        }

        [HttpPost]
        public ActionResult Assign(UsersAssignVM model)
        {
            User user = this._userRepository.GetOne(model.Id);
            user.TeamId = model.TeamId;
            this._userRepository.Update(user);

            return RedirectToAction("Details", "Users", new { Id = user.Id });
        }
    }
}