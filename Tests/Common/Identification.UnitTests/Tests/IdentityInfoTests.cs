using System.Security.Claims;
using Identification.Core.Implementation;
using Shouldly;

namespace Identification.UnitTests.Tests;

public class IdentityInfoTests
{
    [Fact]
    public void GetIdentityId_WithValidGuid_ReturnsGuid()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, testGuid.ToString())]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        Guid result = identityInfo.GetIdentityId();

        // Assert
        result.ShouldBe(testGuid);
    }

    [Fact]
    public void GetIdentityId_WithValidGuidInDifferentFormat_ReturnsGuid()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, testGuid.ToString("D"))]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        Guid result = identityInfo.GetIdentityId();

        // Assert
        result.ShouldBe(testGuid);
    }

    [Fact]
    public void GetIdentityId_WithValidGuidInBrackets_ReturnsGuid()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, testGuid.ToString("B"))]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        Guid result = identityInfo.GetIdentityId();

        // Assert
        result.ShouldBe(testGuid);
    }

    [Fact]
    public void GetIdentityId_WithInvalidGuid_ThrowsInvalidOperationException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, "invalid-guid")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => identityInfo.GetIdentityId())
            .Message.ShouldBe("The identity ID is not set or is not a valid GUID.");
    }

    [Fact]
    public void GetIdentityId_WithNoId_ThrowsInvalidOperationException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => identityInfo.GetIdentityId())
            .Message.ShouldBe("The identity ID is not set or is not a valid GUID.");
    }

    [Fact]
    public void GetIdentityId_WithWhitespaceId_ThrowsInvalidOperationException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, "   ")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => identityInfo.GetIdentityId())
            .Message.ShouldBe("The identity ID is not set or is not a valid GUID.");
    }

    [Fact]
    public void GetIdentityId_WithEmptyId_ThrowsInvalidOperationException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, "")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => identityInfo.GetIdentityId())
            .Message.ShouldBe("The identity ID is not set or is not a valid GUID.");
    }

    [Fact]
    public void GetIdentityId_WithNumericId_ThrowsInvalidOperationException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, "123")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => identityInfo.GetIdentityId())
            .Message.ShouldBe("The identity ID is not set or is not a valid GUID.");
    }

    [Fact]
    public void GetIdentityId_WithEmptyGuid_ReturnsEmptyGuid()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.NameIdentifier, Guid.Empty.ToString())]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        Guid result = identityInfo.GetIdentityId();

        // Assert
        result.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void HasRole_WithMatchingRole_ReturnsTrue()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.Role, "Admin")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        bool result = identityInfo.HasRole("Admin");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasRole_WithNonMatchingRole_ReturnsFalse()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new Claim(ClaimTypes.Role, "User")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        bool result = identityInfo.HasRole("Admin");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void HasRole_WithCaseInsensitiveMatch_ReturnsTrue()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new(ClaimTypes.Role, "Admin")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        bool result = identityInfo.HasRole("ADMIN");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasRole_WithMultipleRoles_FindsCorrectRole()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([
            new(ClaimTypes.Role, "User"),
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "Manager")
        ]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        identityInfo.HasRole("User").ShouldBeTrue();
        identityInfo.HasRole("Admin").ShouldBeTrue();
        identityInfo.HasRole("Manager").ShouldBeTrue();
        identityInfo.HasRole("Guest").ShouldBeFalse();
    }

    [Fact]
    public void HasRole_WithNullRole_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.HasRole(null!))
            .ParamName.ShouldBe("role");
    }

    [Fact]
    public void HasRole_WithEmptyRole_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.HasRole(""))
            .ParamName.ShouldBe("role");
    }

    [Fact]
    public void HasRole_WithWhitespaceRole_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.HasRole("   "))
            .ParamName.ShouldBe("role");
    }

    [Fact]
    public void HasRole_WithRoleHavingWhitespaceValue_IgnoresWhitespaceRoles()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "   "),  // Whitespace role should be ignored
            new(ClaimTypes.Role, "")      // Empty role should be ignored
        ]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        identityInfo.HasRole("Admin").ShouldBeTrue();
        Should.Throw<ArgumentException>(() => identityInfo.HasRole("   "))
            .ParamName.ShouldBe("role");
    }

    [Fact]
    public void GetValue_WithExistingClaim_ReturnsValue()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new Claim(ClaimTypes.Name, "TestUser")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        string result = identityInfo.GetValue(ClaimTypes.Name);

        // Assert
        result.ShouldBe("TestUser");
    }

    [Fact]
    public void GetValue_WithNonExistingClaim_ReturnsEmpty()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        string result = identityInfo.GetValue(ClaimTypes.Name);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetValue_WithNullName_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.GetValue(null!))
            .ParamName.ShouldBe("name");
    }

    [Fact]
    public void GetValue_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.GetValue(""))
            .ParamName.ShouldBe("name");
    }

    [Fact]
    public void GetValue_WithWhitespaceName_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.GetValue("   "))
            .ParamName.ShouldBe("name");
    }

    [Fact]
    public void GetValue_WithMultipleSameClaims_ReturnsFirstValue()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([
            new Claim(ClaimTypes.Name, "FirstUser"),
            new Claim(ClaimTypes.Name, "SecondUser")
        ]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        string result = identityInfo.GetValue(ClaimTypes.Name);

        // Assert
        result.ShouldBe("FirstUser");
    }

    [Fact]
    public void HasValue_WithExistingClaim_ReturnsTrue()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new Claim(ClaimTypes.Name, "TestUser")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        bool result = identityInfo.HasValue(ClaimTypes.Name);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasValue_WithNonExistingClaim_ReturnsFalse()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        bool result = identityInfo.HasValue(ClaimTypes.Name);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void HasValue_WithNullName_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.HasValue(null!))
            .ParamName.ShouldBe("name");
    }

    [Fact]
    public void HasValue_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.HasValue(""))
            .ParamName.ShouldBe("name");
    }

    [Fact]
    public void HasValue_WithWhitespaceName_ShouldThrowArgumentException()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var identityInfo = new IdentityInfo(infoSetter);

        // Act & Assert
        Should.Throw<ArgumentException>(() => identityInfo.HasValue("   "))
            .ParamName.ShouldBe("name");
    }

    [Fact]
    public void HasValue_WithClaimHavingEmptyValue_StillReturnsTrue()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([new Claim(ClaimTypes.Name, "")]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        bool result = identityInfo.HasValue(ClaimTypes.Name);

        // Assert
        result.ShouldBeTrue(); // HasValue checks for existence, not content
    }

    [Fact]
    public void HasValue_WithMultipleSameClaims_ReturnsTrue()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        infoSetter.SetUser([
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "User")
        ]);
        var identityInfo = new IdentityInfo(infoSetter);

        // Act
        bool result = identityInfo.HasValue(ClaimTypes.Role);

        // Assert
        result.ShouldBeTrue();
    }
}
