using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskReminder.Models;
using TaskReminder.Services;

namespace TaskReminder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NotificationService _notificationService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, NotificationService notificationService, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            string? UserId = _userManager.GetUserId(HttpContext.User);
            var notifications = _notificationService.GetNotificationsForUser(UserId);
            ViewData["Notifications"] = notifications;
            return View();            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
