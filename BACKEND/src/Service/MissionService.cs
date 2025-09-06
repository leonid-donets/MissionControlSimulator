using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MissionControlSimulator.src.model;
using MongoDB.Driver;

namespace MissionControlSimulator.src.Service
{
    public class MissionService
    {
        private readonly IMongoCollection<Mission> _missions;

        public MissionService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MissionControlDB");
            _missions = database.GetCollection<Mission>("Missions");
        }

        public async Task<List<Mission>> GetAsync(bool includeDeleted = false)
        {
            if (includeDeleted)
                return await _missions.Find(m => true).ToListAsync();
            return await _missions.Find(m => !m.IsDeleted).ToListAsync();
        }

        public async Task<Mission?> GetAsync(string id)
        {
            return await _missions.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Mission>> GetByAircraftAsync(string aircraftId)
        {
            return await _missions.Find(m => m.AircraftId == aircraftId && !m.IsDeleted).ToListAsync();
        }

        public async Task<Mission> CreateAsync(Mission mission)
        {
            mission.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            mission.CreatedAt = DateTime.UtcNow;
            mission.UpdatedAt = DateTime.UtcNow;
            mission.IsDeleted = false;

            await _missions.InsertOneAsync(mission);
            return mission;
        }

        public async Task<bool> UpdateAsync(string id, Mission missionIn)
        {
            missionIn.UpdatedAt = DateTime.UtcNow;
            var result = await _missions.ReplaceOneAsync(m => m.Id == id, missionIn);
            return result.ModifiedCount > 0;
        }

        // update only status (convenience)
        public async Task<bool> UpdateStatusAsync(string id, string newStatus)
        {
            var update = Builders<Mission>.Update
                .Set(m => m.Status, newStatus)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);
            var result = await _missions.UpdateOneAsync(m => m.Id == id && !m.IsDeleted, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> SoftDeleteAsync(string id)
        {
            var mission = await _missions.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (mission == null) return false;
            mission.IsDeleted = true;
            mission.DeletedAt = DateTime.UtcNow;
            mission.UpdatedAt = DateTime.UtcNow;
            var result = await _missions.ReplaceOneAsync(m => m.Id == id, mission);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RestoreAsync(string id)
        {
            var mission = await _missions.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (mission == null) return false;
            mission.IsDeleted = false;
            mission.DeletedAt = null;
            mission.UpdatedAt = DateTime.UtcNow;
            var result = await _missions.ReplaceOneAsync(m => m.Id == id, mission);
            return result.ModifiedCount > 0;
        }

        // Purge permanently missions deleted before cutoff
        public async Task PurgeDeletedAsync(int olderThanDays = 30)
        {
            var cutoff = DateTime.UtcNow.AddDays(-olderThanDays);
            var filter = Builders<Mission>.Filter.And(
                Builders<Mission>.Filter.Eq(m => m.IsDeleted, true),
                Builders<Mission>.Filter.Lt(m => m.DeletedAt, cutoff)
            );
            await _missions.DeleteManyAsync(filter);
        }
    }
}
