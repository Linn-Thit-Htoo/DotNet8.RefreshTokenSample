namespace DotNet8.RefreshTokenSample.Api.AppDbContextModels;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Tbl_Login> Tbl_Login { get; set; }
    public DbSet<Tbl_User> Tbl_User { get; set; }
}
