using MongoDB.Driver;
using MissionControlSimulator.src.model;
using System;
using System.Collections.Generic;

namespace MissionControlSimulator.src.Service
{
    public class AircraftService
    {
        private readonly IMongoCollection<Aircraft> _aircrafts;

        public AircraftService()
        {
            var client = new MongoClient("mongodb://localhost:27017"); // כתובת MongoDB
            var database = client.GetDatabase("MissionControlDB");      // שם מסד הנתונים
            _aircrafts = database.GetCollection<Aircraft>("Aircrafts"); // שם הקולקשן
        }

        // החזרת כל כלי הטיס שאינם מחוקים
        public List<Aircraft> GetAll() =>
            _aircrafts.Find(a => !a.IsDeleted).ToList();

        // החזרת כלי טיס לפי מזהה
        public Aircraft? GetById(string id) =>
            _aircrafts.Find(a => a.Id == id && !a.IsDeleted).FirstOrDefault();

        // יצירת כלי טיס חדש
        public Aircraft Create(Aircraft aircraft)
        {
            aircraft.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            aircraft.CreatedAt = DateTime.UtcNow;
            aircraft.UpdatedAt = DateTime.UtcNow;
            aircraft.IsDeleted = false;
            aircraft.DeletedAt = null;

            _aircrafts.InsertOne(aircraft);
            return aircraft;
        }

        // עדכון כלי טיס קיים
        public bool Update(string id, Aircraft aircraftIn)
        {
            aircraftIn.UpdatedAt = DateTime.UtcNow;
            var result = _aircrafts.ReplaceOne(a => a.Id == id, aircraftIn);
            return result.ModifiedCount > 0;
        }

        // מחיקה רכה (Soft Delete)
        public bool SoftDelete(string id)
        {
            var aircraft = _aircrafts.Find(a => a.Id == id).FirstOrDefault();
            if (aircraft == null) return false;

            aircraft.IsDeleted = true;
            aircraft.DeletedAt = DateTime.UtcNow;
            aircraft.UpdatedAt = DateTime.UtcNow;

            var result = _aircrafts.ReplaceOne(a => a.Id == id, aircraft);
            return result.ModifiedCount > 0;
        }

        // שחזור כלי טיס שנמחק רך
        public bool Restore(string id)
        {
            var aircraft = _aircrafts.Find(a => a.Id == id).FirstOrDefault();
            if (aircraft == null || !aircraft.IsDeleted) return false;

            aircraft.IsDeleted = false;
            aircraft.DeletedAt = null;
            aircraft.UpdatedAt = DateTime.UtcNow;

            var result = _aircrafts.ReplaceOne(a => a.Id == id, aircraft);
            return result.ModifiedCount > 0;
        }

        // החזרת כל כלי הטיס שנמחקו
        public List<Aircraft> GetDeleted() =>
            _aircrafts.Find(a => a.IsDeleted).ToList();
    }
}
