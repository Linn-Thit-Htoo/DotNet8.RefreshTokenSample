namespace DotNet8.RefreshTokenSample.Api.Models;

public class JwtResponseModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }

    public JwtResponseModel(string userId, string userName, string email)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
    }
}
