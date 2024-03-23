using BreadPit.Areas.Identity.Data;
using BreadPit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BreadPit.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<BreadPitUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<BreadPitUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var usersWithRoles = _userManager.Users.ToList()
            .Select(user => new UserViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result.ToList()
            }).ToList();

            ViewBag.AllRoles = _roleManager.Roles.ToList();

            return View(usersWithRoles);
        }

        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }
    }
}
