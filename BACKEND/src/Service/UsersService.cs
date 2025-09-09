using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail) =>
            await _usersCollection
                .Find(u => (u.Username == usernameOrEmail || u.Email == usernameOrEmail) && !u.IsDeleted)
                .FirstOrDefaultAsync();

    }
}
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using MissionControlSimulator.src.model;
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
// public async Task<List<User>> GetDeletedUsersAsync() =>
//     await _usersCollection.Find(u => u.IsDeleted).ToListAsync();

//         // Get all users
//         public async Task<List<User>> GetAsync() =>
//             await _usersCollection.Find(u => true).ToListAsync();

//         // Get user by id
//         public async Task<User?> GetAsync(string id) =>
//             await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

//         // Create new user
//         public async Task<User> CreateAsync(User user)
//         {
//             user.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
//             user.CreatedAt = DateTime.UtcNow;
//             user.UpdatedAt = DateTime.UtcNow;

//             await _usersCollection.InsertOneAsync(user);
//             return user;
//         }

//         // Update user
//         public async Task<bool> UpdateAsync(string id, User userIn)
//         {
//             userIn.UpdatedAt = DateTime.UtcNow;
//             var result = await _usersCollection.ReplaceOneAsync(u => u.Id == id, userIn);
//             return result.ModifiedCount > 0;
//         }

//         // Soft delete user
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

//         // Restore user with rules (user/admin)
//         public async Task<bool> RestoreAsync(string id, string requesterRole, bool isOwner, int restoreWindowDays = 7)
//         {
//             var user = await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
//             if (user == null || !user.IsDeleted) return false;

//             var now = DateTime.UtcNow;

//             if (requesterRole != "Admin")
//             {
//                 // User can restore only own account and within X days
//                 if (!isOwner || user.DeletedAt == null || (now - user.DeletedAt.Value).TotalDays > restoreWindowDays)
//                     return false;
//             }

//             user.IsDeleted = false;
//             user.DeletedAt = null;
//             user.UpdatedAt = DateTime.UtcNow;

//             var result = await _usersCollection.ReplaceOneAsync(u => u.Id == id, user);
//             return result.ModifiedCount > 0;
//         }

//         // Purge permanently deleted users older than X days
//         public async Task PurgeDeletedUsersAsync(int daysLimit = 7)
//         {
//             var cutoff = DateTime.UtcNow.AddDays(-daysLimit);
//             var filter = Builders<User>.Filter.And(
//                 Builders<User>.Filter.Eq(u => u.IsDeleted, true),
//                 Builders<User>.Filter.Lt(u => u.DeletedAt, cutoff)
//             );

//             await _usersCollection.DeleteManyAsync(filter);
//         }
//     }
// }
