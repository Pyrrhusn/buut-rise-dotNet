using Microsoft.EntityFrameworkCore;
using Rise.Domain.Users;
using static Rise.Domain.Users.User;

namespace Rise.Persistence.Seeders.Users;

/// <summary>
/// Seeder for adding users to the database
/// </summary>
/// <param name="dbContext">Database context</param>
internal class UserSeeder(ApplicationDbContext dbContext) : GeneralSeeder<User>(dbContext)
{

    /// <summary>
    /// All users
    /// </summary>
    public readonly IList<User> users = [
    new() {
        Email = "patrick.her.der.gaver@gmail.com",
        FirstName = "Patrick",
        FamilyName = "Her De Gaver",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(1980, 12, 12),
        Address = new (){Street = "Korenlei", Number = "7", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "bram.de.clerk@gmail.com",
        FirstName = "Bram",
        FamilyName = "de Clerk",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(1998, 3, 21),
        Address = new (){Street = "Onderbergen", Number = "23", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "simon.piatti@gmail.com",
        FirstName = "Simon",
        FamilyName = "Piatti",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(2002, 8, 13),
        Address = new (){Street = "Koning Albertlaan", Number = "42", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "bindo.chin@gmail.com",
        FirstName = "Bindo",
        FamilyName = "Chin",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(1999, 6, 5),
        Address = new (){Street = "Veldstraat", Number = "14", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "bas.barabich@gmail.com",
        FirstName = "Bas",
        FamilyName = "Barabich",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(2001, 10, 10),
        Address = new (){Street = "Dampoortstraat", Number = "89", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "pushwant.helks@gmail.com",
        FirstName = "Pushwant",
        FamilyName = "Helks",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(2002, 2, 8),
        Address = new (){Street = "Sint-Pietersnieuwstraat", Number = "31", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "sujan.montu@gmail.com",
        FirstName = "Sujan",
        FamilyName = "Montu",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(2002, 11, 10),
        Address = new (){Street = "Bagattenstraat", Number = "5", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "xan.serket@gmail.com",
        FirstName = "Xan",
        FamilyName = "Serket",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(1995, 1, 1),
        Address = new (){Street = "Henegouwenstraat", Number = "12", City = "Gent", PostalCode = "9000", Country = "België"},
    },
    new() {
        Email = "kimberlie.de.clerck@gmail.com",
        FirstName = "Kimberlie",
        FamilyName = "De Clerck",
        PhoneNumber = "+32477587465",
        DateOfBirth = new DateTime(1973, 7, 23),
        Address = new (){Street = "Sint-Jacobsnieuwstraat", Number = "22", City = "Gent", PostalCode = "9000", Country = "België"},
    },
];

    protected override DbSet<User> DbSet => _dbContext.Users;

    protected override IEnumerable<User> Items => users;

}
