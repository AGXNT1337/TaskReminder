using TaskReminder.Data;
using TaskReminder.Models;
using TaskReminder.Services;

namespace TaskReminder.BackgroundTasks
{
    public class TaskNotifier
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public TaskNotifier(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }
        public void CheckForOverdueTasks()
        {
            DateTime now = DateTime.Now;
            var overdueTasks = _context.Tasks
                .Where(x => x.TaskDate < now.AddMinutes(x.NotificationBeforeTask) && !x.UserNotified)
                .ToList();

            foreach (var task in overdueTasks)
            {
                task.UserNotified = true;
                var notification = new Notification
                {
                    UserId = task.UserId,
                    Message = $"Reminder for '{task.Title}' at {task.TaskDate}",
                    Timestamp = task.TaskDate.AddMinutes(-task.NotificationBeforeTask),
                    IsRead = false
                };
                _notificationService.CreateNotification(notification);
            }
        }

    }
}
