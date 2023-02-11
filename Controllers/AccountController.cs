using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGSP.Data;
using SGSP.Models;
using SGSP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGSP.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly dbSpotsContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
                                      SignInManager<IdentityUser> signInManager, dbSpotsContext context, IUsuariosRepository usuariosRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usuariosRepository = usuariosRepository;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Register/{returnUrl?}")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register/{returnUrl?}")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    Usuario newUser = new Usuario()
                    {
                        IdAspNetUser = user.Id,
                        IsActive = true,
                        CreatedOn = DateTime.Now
                    };

                    await _usuariosRepository.Create(newUser);

                    var userClaims = await _userManager.FindByEmailAsync(user.Email);

                    var claims = new List<Claim>
                         {
                             new Claim("UserName", user.UserName),
                             new Claim("Email", user.Email),
                             new Claim("IdUser", user.Id),
                             new Claim("Nome", model.Nome),
                             new Claim("Apelido", model.Apelido)

                         };

                    foreach (var roleUser in claims)
                    {
                        await _userManager.AddClaimAsync(userClaims, roleUser);
                    }

                    return RedirectToAction("index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Login/{returnUrl?}")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login/{returnUrl?}")]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    var userLog = await _userManager.FindByNameAsync(user.Username);
                    var roles = await _userManager.GetRolesAsync(userLog);
                    var roleLower = roles.FirstOrDefault().ToLower();
                
                    if (roleLower != null)
                    {
                        //if (roleLower == "administrador" || roleLower == "operador comercial" || roleLower == "chefe de publicidade e vendas")
                        //    return RedirectToAction("Index", "Planificacao");
                        if (roleLower == "locutor")
                            return RedirectToAction("LocutorAdmin", "Planificacao");
                        else if (roleLower == "sonorizador")
                            return RedirectToAction("SonorizadorAdmin", "Planificacao");
                        else if (roleLower == "coordenador")
                            return RedirectToAction("CoordenadorAdmin", "Planificacao");
                        else if (roleLower == "cliente")
                            return RedirectToAction("Cliente", "Home");
                        else
                            return RedirectToAction("Index", "Planificacao");
                        //else if (roleLower == "operador comercial" || roleLower == "chefe de publicidade e vendas")
                        //    return RedirectToAction("Comercial", "Planificacao");
                    }
                    else
                    {
                       await Logout();
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
