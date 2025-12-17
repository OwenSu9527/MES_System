using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        /// <summary>
        /// User. Role can be "Admin", "Technician", "User", etc.
        /// </summary>
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
    }
}
