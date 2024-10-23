using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Quiz_App.Controllers
{
    namespace Quiz.Controllers
    {
        [Authorize(Roles = "Admin")] // Only Admin can access these actions
        public class RoleController : Controller
        {
            private readonly RoleManager<IdentityRole> _roleManager;

            public RoleController(RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
            }

            // GET: Role
            public IActionResult Index()
            {
                var roles = _roleManager.Roles; // Get all roles
                return View(roles);
            }

            // GET: Role/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: Role/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(string roleName)
            {
                if (ModelState.IsValid)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View();
            }

            // GET: Role/Edit/5
            public async Task<IActionResult> Edit(string id)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
                return View(role);
            }

            // POST: Role/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(string id, string roleName)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    role.Name = roleName;
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(role);
            }

            // GET: Role/Delete/5
            public async Task<IActionResult> Delete(string id)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
                return View(role);
            }

            // POST: Role/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(string id)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                return NotFound();
            }
        }
    }
}
