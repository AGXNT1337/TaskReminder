using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskReminder.Data;
using TaskReminder.Models;
using TaskReminder.BackgroundTasks;
using TaskReminder.Services;
namespace TaskReminder.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TaskNotifier _taskNotifier;
        private readonly NotificationService _notificationService;


        public TasksController(ApplicationDbContext context, UserManager<IdentityUser> userManager, TaskNotifier taskNotifier, NotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _taskNotifier = taskNotifier;
            _notificationService = notificationService;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            string? UserId = _userManager.GetUserId(HttpContext.User);
            if (UserId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            else
            {
                var notifications = _notificationService.GetNotificationsForUser(UserId);
                ViewData["Notifications"] = notifications;
                var tasksList = await _context.Tasks.Where(x => x.UserId == UserId).ToListAsync();
                return View(tasksList);
            }


        }

        // GET: Tasks/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? UserId = _userManager.GetUserId(HttpContext.User);
            var tasks = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);

            if (tasks == null || tasks.UserId != UserId)
            {
                return NotFound();
            }
            else
            {
                return View(tasks);
            }
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            string? UserId = _userManager.GetUserId(HttpContext.User);
            if (UserId != null)
            {
                return View();
            }
            else
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,TaskDate,NotificationBeforeTask")] Tasks tasks)
        {
            string? UserId = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid && UserId != null)
            {
                tasks.UserId = UserId;
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tasks);
        }

        // GET: Tasks/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? UserId = _userManager.GetUserId(HttpContext.User);
            var tasks = await _context.Tasks.FindAsync(id);

            if (tasks == null || tasks.UserId != UserId)
            {
                return NotFound();
            }
            else
            {
                return View(tasks);
            }
        }

        // POST: Tasks/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,TaskDate,NotificationBeforeTask")] Tasks tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? UserId = _userManager.GetUserId(HttpContext.User);
                    tasks.UserId = UserId;
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tasks);
        }

        // GET: Tasks/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? UserId = _userManager.GetUserId(HttpContext.User);
            var tasks = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id);

            if (tasks == null || tasks.UserId != UserId)
            {
                return NotFound();
            }
            else
            {
                return View(tasks);
            }
        }

        // POST: Tasks/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? UserId = _userManager.GetUserId(HttpContext.User);
            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null || tasks.UserId != UserId)
            {
                return NotFound();
            }
            else
            {
                _context.Tasks.Remove(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }


        public IActionResult GetNotifications()
        {
            string? UserId = _userManager.GetUserId(HttpContext.User);
            if (UserId == null)
            {
                return (NotFound());
            }
            else
            {
                _taskNotifier.CheckForOverdueTasks();
                var notifications = _notificationService.GetNotificationsForUser(UserId);
                return Json(notifications);
            }
        }

        public IActionResult MarkNotificationAsRead(int id)
        {
            _notificationService.MarkNotificationAsRead(id);
            return Ok();
        }

        public IActionResult NotificationSoundPlayed(int id)
        {
            _notificationService.MarkPlayedSoundAsComplete(id);
            return Ok();
        }

    }
}
