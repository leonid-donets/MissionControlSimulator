using MongoDB.Driver;
using MissionControlSimulator.src.model;

namespace MissionControlSimulator.src.Services
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

        public List<Aircraft> Get() => _aircrafts.Find(a => true).ToList();

        public Aircraft Get(string id) => _aircrafts.Find(a => a.Id == id).FirstOrDefault();

public Aircraft Create(Aircraft aircraft)
{
    _aircrafts.InsertOne(aircraft); // _aircrafts = MongoDB collection
    return aircraft;
}

        public void Update(string id, Aircraft aircraftIn) =>
            _aircrafts.ReplaceOne(a => a.Id == id, aircraftIn);

public bool Remove(string id)
{
    var aircraft = _aircrafts.Find(a => a.Id == id);
    if (aircraft == null)
        return false;

    _aircrafts.DeleteOne(a => a.Id == id); // אם MongoDB
    return true;
}
    }
}
