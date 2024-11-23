using Sefia.Common;
using System.Diagnostics.CodeAnalysis;

namespace Sefia.Entities;

public class User
{
    public User()
    {
        Id = Guid.NewGuid().ToString();
    }

    [SetsRequiredMembers]
    public User(string email, string passwordHash, string name)
    {
        Id = Guid.NewGuid().ToString();
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
    }

    public string Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Name { get; set; }
    public bool Enabled { get; set; } = true;
    public string Role { get; set; } = UserRoles.User;
    public List<LoginHistory> History { get; set; } = new List<LoginHistory>();
}
