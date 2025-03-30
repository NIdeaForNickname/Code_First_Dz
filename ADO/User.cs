using System.ComponentModel.DataAnnotations;

namespace ADO;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string PasswordHash { get; set; }
}