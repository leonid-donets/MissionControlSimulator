using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MissionControlSimulator.src.model

{
    public class Mission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // MongoDB עובד עם ObjectId כ-string / id only string in mongodb

        [Required(ErrorMessage = "Mission name is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Mission name must be between 5 and 50 characters")]
        public string Name { get; set; }    // שם המשימה name mission
        [Required(ErrorMessage = "Mission status is required")]
        public string status { get; set; }  // סטטוס המשימה status mission


        public int AircraftId { get; set; }   // הקצאה למטוס
        public Aircraft Aircraft { get; set; } // מאפשר גישה לכל שדות המטוס (אם רוצים)
    }
}