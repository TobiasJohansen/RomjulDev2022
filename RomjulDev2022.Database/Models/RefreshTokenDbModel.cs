namespace RomjulDev2022.Database.Models;

internal class RefreshTokenDbModel
{
    public Guid? Id { get; set; }

    public string? UserId { get; set; }

    public string? RefreshToken { get; set; }

    public string? System { get; set; }
}
