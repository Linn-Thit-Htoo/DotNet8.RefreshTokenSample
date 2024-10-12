namespace DotNet8.RefreshTokenSample.Api.Models;

public class RefreshTokenRequestModel
{
    public int UserId { get; set; }
    public string RefreshToken { get; set; }
}
