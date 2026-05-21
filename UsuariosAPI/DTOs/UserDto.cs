namespace UsuariosAPI.DTOs;
public record UserDto
{
    public string  Email { get; set; }  = string.Empty;
    public string Nombre {  get; set; } = string.Empty;
    public bool Estado { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}