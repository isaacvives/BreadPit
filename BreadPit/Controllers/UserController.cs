using BreadPit.Areas.Identity.Data;
using BreadPit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

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

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string userId)
        {
            Debug.WriteLine(userId);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var viewModel = new UserViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = allRoles.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(model.SelectedRole))
            {
                ModelState.AddModelError("SelectedRole", "Please select a role");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
            Debug.WriteLine(model.SelectedRole);
            await _userManager.AddToRoleAsync(user, model.SelectedRole);

            return RedirectToAction("Index");
        }

    }
}
