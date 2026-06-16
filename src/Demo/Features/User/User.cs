using Demo.Common;

namespace Demo.Features.User;

public class User : Entity
{
    public string UserName { get; } = null!;

    public string Password { get; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; } = null!;

    public User(string name, string password, string email, string firstName, string lastName, Guid? id = null)
    : base(id ?? Guid.CreateVersion7())
    {
        UserName = name;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}
