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

        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("Admin|User", ErrorMessage = "Role must be either 'Admin' or 'User'")]
        public string UserRole { get; set; }

    }
}