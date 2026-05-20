using Microsoft.AspNetCore.Identity;

namespace UsuariosAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
}
