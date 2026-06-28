using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsuariosAPI.DTOs;
using UsuariosAPI.Models;

namespace UsuariosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager
                .Roles.Select(r => new RoleDto(r.Name, r.Estado))
                .ToListAsync();

            return Ok(roles);
        }

        [HttpPatch("toggle/{roleName}")]
        public async Task<IActionResult> ToogleEstado(string roleName)
        {
            var rol = await _roleManager
                .Roles
                .FirstAsync(x => x.Name == roleName);

            if (rol is null)
                return NotFound("Rol no encontrado");

            rol.Estado = !rol.Estado;

            var result = await _roleManager.UpdateAsync(rol);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Estado del rol cambiado a: {(rol.Estado ? "Activo" : "Inactivo")}");
        }
    }
}
