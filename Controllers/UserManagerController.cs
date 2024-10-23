using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quiz_App.Models;
using System.Threading.Tasks;

namespace Quiz_App.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Block a user
        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsBlocked = true;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("UserManagement");
        }

        // Unblock a user
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsBlocked = false;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("UserManagement");
        }

        // Promote user to admin
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsAdmin = true;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("UserManagement");
        }

        // Remove admin access
        public async Task<IActionResult> RemoveAdminAccess(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsAdmin = false;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("UserManagement");
        }

        // Display User Management page
        public async Task<IActionResult> UserManagement()
        {
            var users = _userManager.Users;
            return View(users); // Pass the list of users to the view
        }
    }
}
