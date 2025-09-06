using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MissionControlSimulator.src.model
{
    public class Aircraft
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Aircraft name is required")]
        [StringLength(50, ErrorMessage = "Aircraft name cannot be longer than 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Aircraft type is required")]
        [RegularExpression("Idle|InMission|Refueling", ErrorMessage = "Invalid status. Allowed values are: Idle, InMission, Refueling")]
        public string Status { get; set; }

        public bool IsDeleted { get; set; } = false;  // מחיקה רכה
        public DateTime? DeletedAt { get; set; }      // תאריך המחיקה

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
