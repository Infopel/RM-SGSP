using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGSP.Repositories;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using SGSP.Dtos;

namespace SGSP.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUsuariosRepository _usuariosRepository;

        public AdministrationController(UserManager<IdentityUser> userManager,
                                      SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IUsuariosRepository usuariosRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _usuariosRepository = usuariosRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles.Where(r => r.Name != "Administrador" && r.Name != "Cliente").ToList();

            var model = new ListRoleViewModel()
            {
                roles = roles
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role com Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.users.Add(user.UserName);
                }

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role com Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role com Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRoles");
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        public IActionResult CreateUserWithRole()
        {
            var roles = _roleManager.Roles.Where(r => r.Name != "Administrador" && r.Name != "Cliente").ToList();

            ViewBag.idRole = new SelectList(roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserWithRole(CreateUserWithRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, "SGSP@2022");

                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByIdAsync(model.idRole);

                    if (role == null)
                    {
                        ViewBag.ErrorMessage = $"Role with Id = {model.idRole} cannot be found";
                        return View("NotFound");
                    }

                    var userBD = await _userManager.FindByIdAsync(user.Id);

                    IdentityResult resultRole = null;

                    resultRole = await _userManager.AddToRoleAsync(userBD, role.Name);

                    foreach(var cananl in model.IdCanal)
                    {
                        Usuario newUser = new Usuario()
                        {
                            IdAspNetUser = user.Id,
                            IdCanal = Int32.Parse(cananl),
                            IsActive = true,
                            CreatedOn = DateTime.Now,
                            Role = role.Name
                        };

                        if (role.Name == "Cliente")
                            newUser.IdCliente = model.IdCliente;

                        await _usuariosRepository.Create(newUser);
                    }

                    var userClaims = await _userManager.FindByEmailAsync(user.Email);

                        var claims = new List<Claim>
                         {
                             new Claim("UserName", user.UserName),
                             new Claim("Email", user.Email),
                             new Claim("IdUser", user.Id),
                             new Claim("IdCanal", model.IdCanal[0]),
                             new Claim("Role", role.Name),
                             new Claim("Nome", model.Nome),
                             new Claim("Apelido", model.Apelido)

                         };

                        foreach (var roleUser in claims)
                        {
                            await _userManager.AddClaimAsync(userClaims, roleUser);

                        }

                    var principal = User;
                    if (principal != null)
                    {
                        if (_signInManager.IsSignedIn(User) && User.IsInRole("Administrador"))
                        {
                            return RedirectToAction("ListUsers", "Administration");
                        }
                    }
                    else
                        return Ok();

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }

            var roles = _roleManager.Roles.ToList();

            ViewBag.idRole = new SelectList(roles, "Id", "Name");

            return View(model);
        }

        public IActionResult ListUsers()
        {
            var users = _signInManager.UserManager.Users.Where(u => !u.UserName.Contains("admin"));
            var roles = new List<string>();

            foreach (var user in users)
            {
                string str = "";

                foreach (var role in _signInManager.UserManager.GetRolesAsync(user).Result)
                {
                    str = (str == "") ? role.ToString() : str + " - " + role.ToString();
                }

                roles.Add(str);
            }

            var model = new ListUserViewModel()
            {
                users = users.ToList(),
                roles = roles.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);


            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
     
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await _userManager.UpdateAsync(user);

                var claimsNew = new List<Claim>
                         {
                             new Claim("UserName", model.UserName),
                             new Claim("Email", model.Email)
                         };

                var claimsOld= new List<Claim>
                         {
                             new Claim("UserName", user.UserName),
                             new Claim("Email", user.Email)
                         };

                var res = claimsNew.Zip(claimsOld, (n, o) => new { NewC = n, OldC = o });

                foreach (var a in res)
                    {
                            await _userManager.ReplaceClaimAsync(user, a.OldC, a.NewC);
                    }


                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddPasswordAsync(user, "SGSP@2022");
                    return RedirectToAction("ListUsers");
                }

                return View("ListUsers");
            }

        }


        [HttpGet]
        public async Task<IActionResult> UpdatePassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new UpdatePasswordModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
                return View(model);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                await _usuariosRepository.UpdateUsuario(user.Id);
                var result = await _userManager.DeleteAsync(user);

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User com Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            //var claimsNew = new Claim("Role", roles.ToString());
            //var claimsOld = new Claim("Role", model.Where(x => x.IsSelected).Select(y => y.RoleName).ToString());
                             
            //await _userManager.ReplaceClaimAsync(user, claimsOld, claimsNew);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }
    }
}
