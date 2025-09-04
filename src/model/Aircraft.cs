using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MissionControlSimulator.src.model
{
    public class Aircraft
    {
[BsonId]
[BsonRepresentation(BsonType.ObjectId)]
public string? Id { get; set; } // MongoDB יוצר אוטומטית אם null

[Required(ErrorMessage = "Aircraft name is required")]
[StringLength(50, ErrorMessage = "Aircraft name cannot be longer than 50 characters")]
public string Name { get; set; }


[Required(ErrorMessage = "Aircraft type is required")]
[RegularExpression("Idle|InMission|Refueling", ErrorMessage = "Invalid status. Allowed values are: Idle, InMission, Refueling")]
public string status { get; set; }

        public DateTime CreatedAt { get; set; }   // תאריך יצירה start mission

        public DateTime UpdatedAt { get; set; }   // תאריך עדכון אחרון last update mission


    }
}