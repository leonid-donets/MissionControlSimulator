// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using MissionControlSimulator.src.Models;
// using MongoDB.Driver;

// namespace MissionControlSimulator.src.Service
// {
//     public class UsersService
//     {
//         private readonly IMongoCollection<User> _usersCollection;

//         public UsersService()
//         {
//             var client = new MongoClient("mongodb://localhost:27017");
//             var database = client.GetDatabase("MissionControlDB");
//             _usersCollection = database.GetCollection<User>("Users");
//         }

//         public async Task<User?> FindByUsernameAsync(string username) =>
//             await _usersCollection.Find(u => u.Username == username && !u.IsDeleted).FirstOrDefaultAsync();

//         public async Task<List<User>> GetDeletedUsersAsync() =>
//             await _usersCollection.Find(u => u.IsDeleted).ToListAsync();

//         public async Task<List<User>> GetAsync() =>
//             await _usersCollection.Find(u => true).ToListAsync();

//         public async Task<User?> GetAsync(string id) =>
//             await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

//         public async Task<User> CreateAsync(User user)
//         {
//             user.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
//             user.CreatedAt = DateTime.UtcNow;
//             user.UpdatedAt = DateTime.UtcNow;
//             await _usersCollection.InsertOneAsync(user);
//             return user;
//         }

//         public async Task<bool> SoftDeleteAsync(string id)
//         {
//             var user = await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
//             if (user == null) return false;
//             user.IsDeleted = true;
//             user.DeletedAt = DateTime.UtcNow;
//             user.UpdatedAt = DateTime.UtcNow;
//             var result = await _usersCollection.ReplaceOneAsync(u => u.Id == id, user);
//             return result.ModifiedCount > 0;
//         }

//         public async Task<bool> RestoreAsync(string id, string requesterRole, bool isOwner, int restoreWindowDays = 7)
//         {
//             var user = await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
//             if (user == null || !user.IsDeleted) return false;

//             var now = DateTime.UtcNow;
//             if (requesterRole != "Admin")
//             {
//                 if (!isOwner || user.DeletedAt == null || (now - user.DeletedAt.Value).TotalDays > restoreWindowDays)
//                     return false;
//             }

//             user.IsDeleted = false;
//             user.DeletedAt = null;
//             user.UpdatedAt = DateTime.UtcNow;
//             var result = await _usersCollection.ReplaceOneAsync(u => u.Id == id, user);
//             return result.ModifiedCount > 0;
//         }
//     }
// }
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MissionControlSimulator.src.model;
using MissionControlSimulator.src.Models;
using MongoDB.Driver;

namespace MissionControlSimulator.src.Service
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MissionControlDB");
            _usersCollection = database.GetCollection<User>("Users");
        }

        public async Task<User?> FindByUsernameAsync(string username) =>
            await _usersCollection.Find(u => u.Username == username && !u.IsDeleted).FirstOrDefaultAsync();

        public async Task<List<User>> GetDeletedUsersAsync() =>
            await _usersCollection.Find(u => u.IsDeleted).ToListAsync();

        public async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(u => true).ToListAsync();

        public async Task<User?> GetAsync(string id) =>
            await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<User> CreateAsync(User user)
        {
            user.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _usersCollection.InsertOneAsync(user);
            return user;
        }

        public async Task<bool> SoftDeleteAsync(string id)
        {
            var user = await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return false;
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _usersCollection.ReplaceOneAsync(u => u.Id == id, user);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RestoreAsync(string id, string requesterRole, bool isOwner, int restoreWindowDays = 7)
        {
            var user = await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null || !user.IsDeleted) return false;

            var now = DateTime.UtcNow;
            if (requesterRole != "Admin")
            {
                if (!isOwner || user.DeletedAt == null || (now - user.DeletedAt.Value).TotalDays > restoreWindowDays)
                    return false;
            }

            user.IsDeleted = false;
            user.DeletedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _usersCollection.ReplaceOneAsync(u => u.Id == id, user);
            return result.ModifiedCount > 0;
        }
    }
}
