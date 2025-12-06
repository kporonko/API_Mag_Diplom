using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Models
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string Token { get; set; } // JWT-токен
    }
}
