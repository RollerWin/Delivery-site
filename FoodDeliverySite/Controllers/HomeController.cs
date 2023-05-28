using FoodDeliverySite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace FoodDeliverySite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

//------Отображение самой первой заглавной страницы---------------------------------------------------------------------
        public IActionResult Index()
        {
            return View();
        }

//------Отображение страницы о нас--------------------------------------------------------------------------------------
        public IActionResult About(string? id)
        {

            if(id != null)
            {
                Response.Cookies.Append("name", id.ToString());
            }

            return View();
        }

//------Отображение страницы о нас--------------------------------------------------------------------------------------
        public IActionResult EasterEgg(string? id)
        {
            if (Request.Cookies.ContainsKey("name"))
            {
                ViewData["name"] = Request.Cookies["name"]?.ToString();
            }
            
            if (HttpContext.Session.Keys.Contains("VisitorsCount"))
            {
                int count = Convert.ToInt32(HttpContext.Session.GetInt32("VisitorsCount"));
                count++;
                HttpContext.Session.SetInt32("VisitorsCount", count);

            }
            else
            {
                HttpContext.Session.SetInt32("VisitorsCount", 1);
            }
            return View();
        }

//------Отображение страницы о нас--------------------------------------------------------------------------------------
        public IActionResult AddSession(string? id)
        {
            if (Request.Cookies.ContainsKey("name"))
            {
                ViewData["name"] = Request.Cookies["name"]?.ToString();
            }
            return View();
        }

        //------Какая-то страница с выводом ошибки------------------------------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}