using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsuariosAPI.DTOs;
using UsuariosAPI.Models;

namespace UsuariosAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
            return Unauthorized("Credenciales inválidas");

        if (!user.Estado)
            return Unauthorized("Credenciales inválidas");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
            return Unauthorized("Credenciales inválidas");

        var roles = await _userManager.GetRolesAsync(user);

        foreach(var roleName in roles)
        {
            var role  = await _roleManager.FindByIdAsync(roleName);

            if (role is null || !role.Estado)
                return Unauthorized($"Su rol {roleName} está inactivo");
        }


        return Ok();
    }
}