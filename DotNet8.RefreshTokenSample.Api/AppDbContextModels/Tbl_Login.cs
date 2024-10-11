using System.ComponentModel.DataAnnotations;

namespace DotNet8.RefreshTokenSample.Api.AppDbContextModels
{
    public class Tbl_Login
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
