using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySite.Controllers
{
    [Authorize(Roles ="admin")]
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

//------Отображение страницы со всеми ролями----------------------------------------------------------------------------
        public ActionResult Index()
        {
            return View(_roleManager.Roles.ToList());
        }

        // GET: RolesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

//------Отображение страницы с созданием роли---------------------------------------------------------------------------
        public ActionResult Create()
        {
            return View();
        }

//------Обработка создания новой роли-----------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

//------Отображение страницы для изменения роли-------------------------------------------------------------------------
        public ActionResult Edit(int id)
        {
            return View();
        }

//------Обработка изменения роли----------------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

//------Отображение страницы с удалением роли---------------------------------------------------------------------------
        public ActionResult Delete(int id)
        {
            return View();
        }

//------Обработка и полное удаление роли--------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
