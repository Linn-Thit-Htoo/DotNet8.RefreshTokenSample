﻿using System.ComponentModel.DataAnnotations;

namespace DotNet8.RefreshTokenSample.Api.AppDbContextModels;

public class Tbl_User
{
    [Key]
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
