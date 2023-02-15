using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Application.Models.Auth.Requests
{
    public class SignupRequest
    {
           public string Username { get; set; }
           public string Email { get; set; }
           public string Password { get; set; }
    }
}