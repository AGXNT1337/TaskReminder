using TaskReminder.Data;
using TaskReminder.Models;

namespace TaskReminder.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public void CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }
        public List<Notification> GetNotificationsForUser(string userId)
        {
            return _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .OrderByDescending(x => x.Timestamp)
                .ToList();
        }
        public void MarkNotificationAsRead(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
        }
        public void MarkPlayedSoundAsComplete(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.PlayedSound = true;
                _context.SaveChanges();
            }
        }
    }
}
