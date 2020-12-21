using API.Models;
using API.Models.ViewModels;
using System.Web.Mvc;

namespace API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (this.ModelState.IsValid)
            {
                AuthenticationManager.Authenticate(model.Username, model.Password);

                if (AuthenticationManager.LoggedUser == null)
                    ModelState.AddModelError("authenticationFailed", "Wrong username or password!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            AuthenticationManager.Logout();

            return RedirectToAction("Index", "Home");
        }
    }
}