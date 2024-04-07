using System.ComponentModel.DataAnnotations;

namespace TaskReminder.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; } = false;
        public bool PlayedSound { get; set; } = false;
    }
}
