using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UsuariosAPI.DTOs;
using UsuariosAPI.Models;

namespace UsuariosAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsuariosController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var role = await _roleManager.FindByNameAsync(registerDto.Rol);

        if (role is null || !role.Estado)
            return BadRequest("El rol especificado no existe o está inactivo.");

        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            Estado = true,
            Nombre = registerDto.Nombre,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, registerDto.Rol);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return BadRequest("Error al asignar el rol al usuario");
        }

        return Ok("Usuario registrado exitosamente con rol");
    }

    [HttpGet("perfil")]
    public async Task<ActionResult<UserDto>> Perfil()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);

        if (email is null)
            return Unauthorized();

        var user = await _userManager.FindByNameAsync(email);

        if (user is null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Email = email,
            Nombre = user.Nombre,
            Estado = user.Estado,
            Roles = roles
        };
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _userManager.Users.ToListAsync();
        List<UserDto> listadoUsuarios = new List<UserDto>();

        foreach (var user in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(user);
            listadoUsuarios.Add(new UserDto
            {
                Email = user.Email,
                Nombre = user.Nombre,
                Estado = user.Estado,
                Roles = roles
            });
        }

        return Ok(listadoUsuarios);
    }

    [HttpGet("User")]
    public async Task<IActionResult> GetUsuario([FromBody] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return NotFound("Usuario no encontrado");

        var roles = await _userManager.GetRolesAsync(user);

        var userDto = new UserDto
        {
            Email = user.Email,
            Nombre = user.Nombre,
            Estado = user.Estado,
            Roles = roles
        };

        return Ok(userDto);
    }
}
