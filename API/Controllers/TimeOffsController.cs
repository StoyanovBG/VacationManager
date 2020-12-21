using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using API.Models;
using API.Models.Filters;
using API.Models.ViewModels.TimeOffs;
using API.Models.ViewModels.TimeOffs.PaidTimeOffs;
using API.Models.ViewModels.TimeOffs.SickTimeOff;
using API.Models.ViewModels.TimeOffs.UnpaidTimeOffs;
using API.Models.ViewModels.Users;
using API.Services;
using Data;
using Data.Entities;
using Data.Entities.TimeOffs;

namespace API.Controllers
{
    public class TimeOffsController : Controller
    {
        [HttpGet]
        public ActionResult Index(TimeOffsIndexVM model)
        {
            User loggedUser = AuthenticationManager.LoggedUser;

            if (loggedUser == null)
                return RedirectToAction("Index", "Home");

            model.PaidTimeOffsPager = model.PaidTimeOffsPager ?? new PagerVM();
            model.UnpaidTimeOffsPager = model.UnpaidTimeOffsPager ?? new PagerVM();
            model.SickTimeOffsPager = model.SickTimeOffsPager ?? new PagerVM();

            model.PaidTimeOffsPager.Page = model.PaidTimeOffsPager.Page <= 0 ? 1 : model.PaidTimeOffsPager.Page;
            model.UnpaidTimeOffsPager.Page = model.UnpaidTimeOffsPager.Page <= 0 ? 1 : model.UnpaidTimeOffsPager.Page;
            model.SickTimeOffsPager.Page = model.SickTimeOffsPager.Page <= 0 ? 1 : model.SickTimeOffsPager.Page;

            model.PaidTimeOffsPager.ItemsPerPage =
                model.PaidTimeOffsPager.ItemsPerPage <= 0 ? 10 : model.PaidTimeOffsPager.ItemsPerPage;
            model.UnpaidTimeOffsPager.ItemsPerPage = model.UnpaidTimeOffsPager.ItemsPerPage <= 0
                ? 10
                : model.UnpaidTimeOffsPager.ItemsPerPage;
            model.SickTimeOffsPager.ItemsPerPage =
                model.SickTimeOffsPager.ItemsPerPage <= 0 ? 10 : model.SickTimeOffsPager.ItemsPerPage;

            model.PaidTimeOffsFilter = model.PaidTimeOffsFilter ?? new TimeOffsFilterVM();
            model.UnpaidTimeOffsFilter = model.UnpaidTimeOffsFilter ?? new TimeOffsFilterVM();
            model.SickTimeOffsFilter = model.SickTimeOffsFilter ?? new TimeOffsFilterVM();

            bool isPaidDateNotSet = model.PaidTimeOffsFilter.CreatedAfter == null;
            bool isUnpaidDateNotSet = model.UnpaidTimeOffsFilter.CreatedAfter == null;
            bool isSickDateNotSet = model.SickTimeOffsFilter.CreatedAfter == null;

            OvmDbContext context = new OvmDbContext();

            IQueryable<PaidTimeOff> paidQuery = context.PaidTimeOffs.Where(p =>
                p.RequestorId == loggedUser.Id &&
                (isPaidDateNotSet || p.CreatedOn >= model.PaidTimeOffsFilter.CreatedAfter));

            IQueryable<UnpaidTimeOff> unpaidQuery = context.UnpaidTimeOffs.Where(p =>
                p.RequestorId == loggedUser.Id &&
                (isUnpaidDateNotSet || p.CreatedOn >= model.UnpaidTimeOffsFilter.CreatedAfter));

            IQueryable<SickTimeOff> sickQuery = context.SickTimeOffs.Where(p =>
                p.RequestorId == loggedUser.Id &&
                (isSickDateNotSet || p.CreatedOn >= model.SickTimeOffsFilter.CreatedAfter));

            model.PaidTimeOffsPager.PagesCount =
                (int)Math.Ceiling(paidQuery.Count() / (double)model.PaidTimeOffsPager.ItemsPerPage);
            model.UnpaidTimeOffsPager.PagesCount =
                (int)Math.Ceiling(unpaidQuery.Count() / (double)model.UnpaidTimeOffsPager.ItemsPerPage);
            model.SickTimeOffsPager.PagesCount =
                (int)Math.Ceiling(sickQuery.Count() / (double)model.SickTimeOffsPager.ItemsPerPage);

            paidQuery = paidQuery.OrderByDescending(to => to.CreatedOn)
                .Skip((model.PaidTimeOffsPager.Page - 1) * model.PaidTimeOffsPager.ItemsPerPage)
                .Take(model.PaidTimeOffsPager.ItemsPerPage);
            unpaidQuery = unpaidQuery.OrderByDescending(to => to.CreatedOn)
                .Skip((model.UnpaidTimeOffsPager.Page - 1) * model.UnpaidTimeOffsPager.ItemsPerPage)
                .Take(model.UnpaidTimeOffsPager.ItemsPerPage);
            sickQuery = sickQuery.OrderByDescending(to => to.CreatedOn)
                .Skip((model.SickTimeOffsPager.Page - 1) * model.SickTimeOffsPager.ItemsPerPage)
                .Take(model.SickTimeOffsPager.ItemsPerPage);

            model.PaidTimeOffs = paidQuery.Select(pto => new PaidTimeOffsVM
            {
                Id = pto.Id,
                CreatedOn = pto.CreatedOn,
                IsApproved = pto.IsApproved,
                Requestor = new UsersPair
                {
                    Id = pto.Requestor.Id,
                    Username = pto.Requestor.Username
                },
                From = pto.From,
                IsHalfDay = pto.IsHalfDay,
                To = pto.To
            }).ToList();

            model.UnpaidTimeOffs = unpaidQuery.Select(pto => new UnpaidTimeOffsVM
            {
                Id = pto.Id,
                CreatedOn = pto.CreatedOn,
                IsApproved = pto.IsApproved,
                Requestor = new UsersPair
                {
                    Id = pto.Requestor.Id,
                    Username = pto.Requestor.Username
                },
                From = pto.From,
                IsHalfDay = pto.IsHalfDay,
                To = pto.To
            }).ToList();

            model.SickTimeOffs = sickQuery.Select(pto => new SickTimeOffsVM
            {
                Id = pto.Id,
                CreatedOn = pto.CreatedOn,
                IsApproved = pto.IsApproved,
                Requestor = new UsersPair
                {
                    Id = pto.Requestor.Id,
                    Username = pto.Requestor.Username
                },
                From = pto.From,
                To = pto.To
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult EditPaid(int? id)
        {
            PaidTimeOffsEditVM model = new PaidTimeOffsEditVM();
            ;
            OvmDbContext context = new OvmDbContext();

            if (id != null)
            {
                PaidTimeOff pto = context.PaidTimeOffs.Find(id.Value);
                model.To = pto.To;
                model.From = pto.From;
                model.IsHalfDay = pto.IsHalfDay;
                model.Id = pto.Id;
            }

            context.Dispose();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditPaid(PaidTimeOffsEditVM model)
        {
            OvmDbContext context = new OvmDbContext();

            PaidTimeOff pto = new PaidTimeOff
            {
                Id = model.Id,
                CreatedOn = DateTime.Now,
                RequestorId = AuthenticationManager.LoggedUser.Id,
                From = model.From,
                To = model.To,
                IsHalfDay = model.IsHalfDay
            };

            context.PaidTimeOffs.AddOrUpdate(pt => pt.Id, pto);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeletePaid(int id)
        {
            OvmDbContext context = new OvmDbContext();
            PaidTimeOff pto = context.PaidTimeOffs.Find(id);
            context.PaidTimeOffs.Remove(pto);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditSick(int? id)
        {
            SickTimeOffsEditVM model = new SickTimeOffsEditVM();
            OvmDbContext context = new OvmDbContext();

            if (id != null)
            {
                SickTimeOff pto = context.SickTimeOffs.Find(id.Value);
                model.To = pto.To;
                model.From = pto.From;
                model.Id = pto.Id;
                model.AttachmentPath = pto.AttachmentPath;
            }

            context.Dispose();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditSick(SickTimeOffsEditVM model)
        {
            OvmDbContext context = new OvmDbContext();

            SickTimeOff sto = new SickTimeOff
            {
                Id = model.Id,
                CreatedOn = DateTime.Now,
                RequestorId = AuthenticationManager.LoggedUser.Id,
                From = model.From,
                To = model.To,
                AttachmentPath = model.Attachment == null
                    ? model.AttachmentPath
                    : StorageService.SaveFile(model.Attachment)
            };

            context.SickTimeOffs.AddOrUpdate(pt => pt.Id, sto);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteSick(int id)
        {
            OvmDbContext context = new OvmDbContext();
            SickTimeOff sto = context.SickTimeOffs.Find(id);
            StorageService.DeleteFile(sto.AttachmentPath);
            context.SickTimeOffs.Remove(sto);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DetailsSick(int id)
        {
            OvmDbContext context = new OvmDbContext();
            SickTimeOff sto = context.SickTimeOffs.Find(id);

            SickTimeOffsDetailsVM model = new SickTimeOffsDetailsVM
            {
                Id = sto.Id,
                From = sto.From,
                To = sto.To,
                IsApproved = sto.IsApproved,
                LastChangedOn = sto.CreatedOn,
            };

            context.Dispose();

            return View(model);
        }

        [HttpGet]
        public ActionResult DownloadAttachment(int id)
        {
            OvmDbContext context = new OvmDbContext();
            SickTimeOff sto = context.SickTimeOffs.Find(id);
            string attachmentPath = sto.AttachmentPath;

            return File(attachmentPath, System.Net.Mime.MediaTypeNames.Application.Octet,
                Path.GetFileName(attachmentPath));
        }

        [HttpGet]
        public ActionResult EditUnpaid(int? id)
        {
            UnpaidTimeOffsEditVM model = new UnpaidTimeOffsEditVM();
            OvmDbContext context = new OvmDbContext();

            if (id != null)
            {
                UnpaidTimeOff pto = context.UnpaidTimeOffs.Find(id.Value);
                model.To = pto.To;
                model.From = pto.From;
                model.IsHalfDay = pto.IsHalfDay;
                model.Id = pto.Id;
            }

            context.Dispose();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditUnpaid(UnpaidTimeOffsEditVM model)
        {
            OvmDbContext context = new OvmDbContext();

            UnpaidTimeOff uto = new UnpaidTimeOff
            {
                Id = model.Id,
                CreatedOn = DateTime.Now,
                RequestorId = AuthenticationManager.LoggedUser.Id,
                From = model.From,
                To = model.To,
                IsHalfDay = model.IsHalfDay
            };

            context.UnpaidTimeOffs.AddOrUpdate(pt => pt.Id, uto);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteUnpaid(int id)
        {
            OvmDbContext context = new OvmDbContext();
            UnpaidTimeOff pto = context.UnpaidTimeOffs.Find(id);
            context.UnpaidTimeOffs.Remove(pto);
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult AllRequests(TimeOffsApproveVM model)
        {
            User loggedUser = AuthenticationManager.LoggedUser;

            if (loggedUser == null || (loggedUser.Role.Name != "CEO" && loggedUser.Role.Name != "Team Lead"))
                return RedirectToAction("Index", "Home");

            model.PaidTimeOffsPager = model.PaidTimeOffsPager ?? new PagerVM();
            model.UnpaidTimeOffsPager = model.UnpaidTimeOffsPager ?? new PagerVM();
            model.SickTimeOffsPager = model.SickTimeOffsPager ?? new PagerVM();

            model.PaidTimeOffsPager.Page = model.PaidTimeOffsPager.Page <= 0 ? 1 : model.PaidTimeOffsPager.Page;
            model.UnpaidTimeOffsPager.Page = model.UnpaidTimeOffsPager.Page <= 0 ? 1 : model.UnpaidTimeOffsPager.Page;
            model.SickTimeOffsPager.Page = model.SickTimeOffsPager.Page <= 0 ? 1 : model.SickTimeOffsPager.Page;

            model.PaidTimeOffsPager.ItemsPerPage =
                model.PaidTimeOffsPager.ItemsPerPage <= 0 ? 10 : model.PaidTimeOffsPager.ItemsPerPage;
            model.UnpaidTimeOffsPager.ItemsPerPage = model.UnpaidTimeOffsPager.ItemsPerPage <= 0
                ? 10
                : model.UnpaidTimeOffsPager.ItemsPerPage;
            model.SickTimeOffsPager.ItemsPerPage =
                model.SickTimeOffsPager.ItemsPerPage <= 0 ? 10 : model.SickTimeOffsPager.ItemsPerPage;

            model.PaidTimeOffsFilter = model.PaidTimeOffsFilter ?? new TimeOffsFilterVM();
            model.UnpaidTimeOffsFilter = model.UnpaidTimeOffsFilter ?? new TimeOffsFilterVM();
            model.SickTimeOffsFilter = model.SickTimeOffsFilter ?? new TimeOffsFilterVM();

            bool isPaidDateNotSet = model.PaidTimeOffsFilter.CreatedAfter == null;
            bool isUnpaidDateNotSet = model.UnpaidTimeOffsFilter.CreatedAfter == null;
            bool isSickDateNotSet = model.SickTimeOffsFilter.CreatedAfter == null;

            OvmDbContext context = new OvmDbContext();
            IQueryable<PaidTimeOff> paidQuery = null;
            IQueryable<UnpaidTimeOff> unpaidQuery = null;
            IQueryable<SickTimeOff> sickQuery = null;

            if (loggedUser.Role.Name == "CEO")
            {
                paidQuery = context.PaidTimeOffs.Where(p => (isPaidDateNotSet || p.CreatedOn >= model.PaidTimeOffsFilter.CreatedAfter));

                unpaidQuery = context.UnpaidTimeOffs.Where(p => (isUnpaidDateNotSet || p.CreatedOn >= model.UnpaidTimeOffsFilter.CreatedAfter));

                sickQuery = context.SickTimeOffs.Where(p => (isSickDateNotSet || p.CreatedOn >= model.SickTimeOffsFilter.CreatedAfter));
            }

            if (loggedUser.Role.Name == "Team Lead")
            {
                paidQuery = context.PaidTimeOffs.Where(p => p.Requestor.Team.TeamLeadId == loggedUser.Id &&
                                                            (isPaidDateNotSet || p.CreatedOn >= model.PaidTimeOffsFilter.CreatedAfter));

                unpaidQuery = context.UnpaidTimeOffs.Where(p => p.Requestor.Team.TeamLeadId == loggedUser.Id &&
                                                            (isUnpaidDateNotSet || p.CreatedOn >= model.UnpaidTimeOffsFilter.CreatedAfter));

                sickQuery = context.SickTimeOffs.Where(p => p.Requestor.Team.TeamLeadId == loggedUser.Id &&
                                                                (isSickDateNotSet || p.CreatedOn >= model.SickTimeOffsFilter.CreatedAfter));
            }

            model.PaidTimeOffsPager.PagesCount =
                (int)Math.Ceiling(paidQuery.Count() / (double)model.PaidTimeOffsPager.ItemsPerPage);
            model.UnpaidTimeOffsPager.PagesCount =
                (int)Math.Ceiling(unpaidQuery.Count() / (double)model.UnpaidTimeOffsPager.ItemsPerPage);
            model.SickTimeOffsPager.PagesCount =
                (int)Math.Ceiling(sickQuery.Count() / (double)model.SickTimeOffsPager.ItemsPerPage);

            paidQuery = paidQuery.OrderByDescending(to => to.CreatedOn)
                .Skip((model.PaidTimeOffsPager.Page - 1) * model.PaidTimeOffsPager.ItemsPerPage)
                .Take(model.PaidTimeOffsPager.ItemsPerPage);
            unpaidQuery = unpaidQuery.OrderByDescending(to => to.CreatedOn)
                .Skip((model.UnpaidTimeOffsPager.Page - 1) * model.UnpaidTimeOffsPager.ItemsPerPage)
                .Take(model.UnpaidTimeOffsPager.ItemsPerPage);
            sickQuery = sickQuery.OrderByDescending(to => to.CreatedOn)
                .Skip((model.SickTimeOffsPager.Page - 1) * model.SickTimeOffsPager.ItemsPerPage)
                .Take(model.SickTimeOffsPager.ItemsPerPage);

            model.PaidTimeOffs = paidQuery.Select(pto => new PaidTimeOffsVM
            {
                Id = pto.Id,
                CreatedOn = pto.CreatedOn,
                IsApproved = pto.IsApproved,
                Requestor = new UsersPair
                {
                    Id = pto.Requestor.Id,
                    Username = pto.Requestor.Username
                },
                From = pto.From,
                IsHalfDay = pto.IsHalfDay,
                To = pto.To
            }).ToList();

            model.UnpaidTimeOffs = unpaidQuery.Select(pto => new UnpaidTimeOffsVM
            {
                Id = pto.Id,
                CreatedOn = pto.CreatedOn,
                IsApproved = pto.IsApproved,
                Requestor = new UsersPair
                {
                    Id = pto.Requestor.Id,
                    Username = pto.Requestor.Username
                },
                From = pto.From,
                IsHalfDay = pto.IsHalfDay,
                To = pto.To
            }).ToList();

            model.SickTimeOffs = sickQuery.Select(pto => new SickTimeOffsVM
            {
                Id = pto.Id,
                CreatedOn = pto.CreatedOn,
                IsApproved = pto.IsApproved,
                Requestor = new UsersPair
                {
                    Id = pto.Requestor.Id,
                    Username = pto.Requestor.Username
                },
                From = pto.From,
                To = pto.To
            }).ToList();


            return View(model);
        }

        [HttpGet]
        public ActionResult ApprovePaid(int id)
        {
            OvmDbContext context = new OvmDbContext();
            PaidTimeOff pto = context.PaidTimeOffs.Find(id);
            pto.IsApproved = true;
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("AllRequests");
        }

        [HttpGet]
        public ActionResult ApproveUnpaid(int id)
        {
            OvmDbContext context = new OvmDbContext();
            UnpaidTimeOff uto = context.UnpaidTimeOffs.Find(id);
            uto.IsApproved = true;
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("AllRequests");
        }

        public ActionResult ApproveSick(int id)
        {
            OvmDbContext context = new OvmDbContext();
            SickTimeOff sto = context.SickTimeOffs.Find(id);
            sto.IsApproved = true;
            context.SaveChanges();
            context.Dispose();

            return RedirectToAction("AllRequests");
        }
    }
}