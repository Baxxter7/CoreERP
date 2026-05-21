namespace UsuariosAPI.DTOs;

public record UpdateUserDto(string Email, bool Estado, string? NuevoRol);
