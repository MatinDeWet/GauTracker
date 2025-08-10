using System.Security.Claims;
using Identification.Core.Implementation;
using Shouldly;

namespace Identification.UnitTests.Tests;
public class InfoSetterTests
{
    [Fact]
    public void SetUser_ShouldClearExistingAndAddNewClaims()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        Claim[] initialClaims = [new(ClaimTypes.Name, "InitialUser")];
        infoSetter.AddRange(initialClaims);

        Claim[] newClaims = [
            new(ClaimTypes.Name, "NewUser"),
            new(ClaimTypes.Role, "Admin")
        ];

        // Act
        infoSetter.SetUser(newClaims);

        // Assert
        infoSetter.Count.ShouldBe(2);
        infoSetter[0].Value.ShouldBe("NewUser");
        infoSetter[1].Value.ShouldBe("Admin");
    }

    [Fact]
    public void SetUser_WithNullClaims_ShouldThrowArgumentNullException()
    {
        // Arrange
        var infoSetter = new InfoSetter();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => infoSetter.SetUser(null!));
    }

    [Fact]
    public void SetUser_WithEmptyCollection_ShouldClearExistingClaims()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.AddRange([new Claim(ClaimTypes.Name, "TestUser")]);

        // Act
        infoSetter.SetUser([]);

        // Assert
        infoSetter.ShouldBeEmpty();
    }

    [Fact]
    public void Clear_ShouldRemoveAllClaims()
    {
        // Arrange
        InfoSetter infoSetter = [
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin")
        ];

        // Act
        infoSetter.Clear();

        // Assert
        infoSetter.ShouldBeEmpty();
    }

    [Fact]
    public void AddRange_ShouldAddAllClaimsToCollection()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        Claim[] claims = [
            new(ClaimTypes.Name, "TestUser"),
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.NameIdentifier, "123")
        ];

        // Act
        infoSetter.AddRange(claims);

        // Assert
        infoSetter.Count.ShouldBe(3);
        infoSetter.ShouldContain(c => c.Type == ClaimTypes.Name && c.Value == "TestUser");
        infoSetter.ShouldContain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        infoSetter.ShouldContain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "123");
    }

    [Fact]
    public void AddRange_WithNullClaims_ShouldThrowArgumentNullException()
    {
        // Arrange
        var infoSetter = new InfoSetter();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => infoSetter.AddRange(null!));
    }

    [Fact]
    public void AddRange_WithEmptyCollection_ShouldNotAddAnyClaims()
    {
        // Arrange
        var infoSetter = new InfoSetter();

        // Act
        infoSetter.AddRange([]);

        // Assert
        infoSetter.ShouldBeEmpty();
    }

    [Fact]
    public void AddRange_ToExistingClaims_ShouldAppendNewClaims()
    {
        // Arrange
        InfoSetter infoSetter = [];
        infoSetter.Add(new Claim(ClaimTypes.Name, "ExistingUser"));

        Claim[] newClaims = [
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.NameIdentifier, "123")
        ];

        // Act
        infoSetter.AddRange(newClaims);

        // Assert
        infoSetter.Count.ShouldBe(3);
        infoSetter.ShouldContain(c => c.Type == ClaimTypes.Name && c.Value == "ExistingUser");
        infoSetter.ShouldContain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        infoSetter.ShouldContain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "123");
    }
}
