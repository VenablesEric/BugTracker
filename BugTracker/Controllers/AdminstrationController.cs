using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminstrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BugTrackerUser> _userManager;

        public AdminstrationController(RoleManager<IdentityRole> roleManager,
            UserManager<BugTrackerUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var users = _userManager.Users;
            var userData = new List<UserModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                string userRole = "";
                if (userRoles != null && userRoles.Count > 0)
                {
                    userRole = userRoles.First();
                }
                userData.Add(new UserModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Role = userRole
                });
            }
            return View(userData);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new BugTrackerUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role.ToString());
                    return RedirectToAction("Listusers", "Adminstration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return RedirectToAction("Listusers", "Adminstration");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            string userRole = "";
            if (userRoles != null && userRoles.Count > 0)
            {
                userRole = userRoles.First();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = (ROLE)Enum.Parse(typeof(ROLE), userRole)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return RedirectToAction("Listusers", "Adminstration");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var resultUser = await _userManager.UpdateAsync(user);
                var role = (await _userManager.GetRolesAsync(user)).First();
                await _userManager.RemoveFromRoleAsync(user, role);
                var resultRole = await _userManager.AddToRoleAsync(user, model.Role.ToString());

                if (!resultUser.Succeeded || !resultRole.Succeeded)
                {
                    foreach (var error in resultUser.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    foreach (var error in resultRole.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }
            }

            return RedirectToAction("ListUsers", "Adminstration");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("ListUsers");
                //return View("NotFound");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError(string.Empty, "You can't delete an admin");
                return View("ListUsers");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers", "Adminstration");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ListUsers");
        }
    }
}
