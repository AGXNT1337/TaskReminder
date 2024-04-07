using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskReminder.Models
{
    public class Tasks
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        [DisplayName("Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Required]
        [DisplayName("Task Date")]
        public DateTime TaskDate { get; set; }
        [DisplayName("Notification")]
        public int NotificationBeforeTask { get; set; } = 30;
        public bool UserNotified { get; set; } = false;
    }
}
