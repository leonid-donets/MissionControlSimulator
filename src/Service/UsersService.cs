using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MissionControlSimulator.src.model;
using MongoDB.Driver;

namespace MissionControlSimulator.src.Service
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService()
        {
            var client = new MongoClient("mongodb://localhost:27017"); // כתובת MongoDB
            var database = client.GetDatabase("MissionControlDB");      // שם מסד הנתונים
            _usersCollection = database.GetCollection<User>("Users");  // שם הקולקשן
        }

        // קבלת כל המשתמשים // GET ALL USERS
        public async Task<List<User>> GetAsync() => 
            await _usersCollection.Find(a => true).ToListAsync();

        // קבלת משתמש לפי id // GET USER BY ID
        public async Task<User?> GetAsync(string id) => 
            await _usersCollection.Find(a => a.Id == id).FirstOrDefaultAsync();

        // יצירת משתמש חדש // CREATE USER
        public async Task<User> CreateAsync(User user)
        {
            user.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _usersCollection.InsertOneAsync(user);
            return user;
        }

        // עדכון משתמש קיים // UPDATE USER
        public async Task<bool> UpdateAsync(string id, User userIn)
        {
            userIn.UpdatedAt = DateTime.UtcNow;
            var result = await _usersCollection.ReplaceOneAsync(u => u.Id == id, userIn);
            return result.ModifiedCount > 0;
        }

        // מחיקת משתמש לפי id // DELETE USER
        public async Task<bool> RemoveAsync(string id)
        {
            var user = await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return false;

            var result = await _usersCollection.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
