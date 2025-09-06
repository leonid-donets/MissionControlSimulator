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
        public string? Id { get; set; }  // מזהה ייחודי במשימות

        [Required(ErrorMessage = "Mission name is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Mission name must be between 5 and 50 characters")]
        public string Name { get; set; } = string.Empty;  // שם המשימה

        [Required(ErrorMessage = "Mission status is required")]
        [RegularExpression("Pending|Active|Completed|Cancelled", ErrorMessage = "Invalid status. Allowed values: Pending, Active, Completed, Cancelled")]
        public string Status { get; set; } = "Pending";   // סטטוס ברירת מחדל = Pending

        public string? AircraftId { get; set; } // מזהה המטוס שאליו משויכת המשימה
        public Aircraft? Aircraft { get; set; } // אופציונלי: מאפשר להביא פרטים מלאים על המטוס

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // תאריך יצירה
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // תאריך עדכון

        public bool IsDeleted { get; set; } = false;   // soft delete
        public DateTime? DeletedAt { get; set; }       // מתי נמחקה
    }
}
