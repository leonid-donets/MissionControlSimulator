using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MissionControlSimulator.src.model
{
    public class Mission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Mission name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        // סטטוס מנותק מרשימת ערכים על ידי RegularExpression
        [Required]
        [RegularExpression("Pending|Active|Completed|Cancelled", ErrorMessage = "Invalid status. Allowed: Pending, Active, Completed, Cancelled")]
        public string Status { get; set; } = "Pending";

        // קישור ל-Aircraft (ObjectId כ-string)
        public string? AircraftId { get; set; }
        // אופציונלי: לא צריך לשלוח את ה-Aircraft בתוך JSON ב-POST, זה יכול לשמש ל-DTOים בלבד
        public Aircraft? Aircraft { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
