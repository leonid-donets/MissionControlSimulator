using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MissionControlSimulator.src.model
{

    public enum UserRole
    {
        Admin,
        User
    }

    public class User
    {

        public string Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string PasswordHash { get; set; }

        [Required]
        [RegularExpression("Admin|User")]
        public string UserRole { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}