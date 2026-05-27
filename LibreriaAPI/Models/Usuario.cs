using System.ComponentModel.DataAnnotations;


namespace LibreriaAPI.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }


    }
}
