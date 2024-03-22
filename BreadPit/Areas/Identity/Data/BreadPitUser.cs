using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BreadPit.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BreadPitUser class
public class BreadPitUser : IdentityUser
{
    public UserType UserType { get; set; } = UserType.User;
}

public enum UserType { 
    User,
    Worker,
    Admin
}