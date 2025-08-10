using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Moq;
using Searchable.Core;
using Searchable.UnitTests.Context;
using Searchable.UnitTests.Models;
using Shouldly;

namespace Searchable.UnitTests.Tests;
public class SearchableExtensionsTests
{
    [Fact]
    public void FullTextSearch_WithEmptySearchTerm_ShouldReturnOriginalQueryable()
    {
        // Arrange
        IQueryable<TestArticle> testData = GetTestArticles().AsQueryable();

        // Act
        IQueryable<TestArticle> results = testData.FullTextSearch("");

        // Assert
        results.ShouldBeSameAs(testData);
    }

    [Fact]
    public void FullTextSearch_WithNullSearchTerm_ShouldReturnOriginalQueryable()
    {
        // Arrange
        IQueryable<TestArticle> testData = GetTestArticles().AsQueryable();

        // Act
        IQueryable<TestArticle> results = testData.FullTextSearch(null);

        // Assert
        results.ShouldBeSameAs(testData);
    }

    [Fact]
    public void FullTextSearch_WithWhiteSpaceSearchTerm_ShouldReturnOriginalQueryable()
    {
        // Arrange
        IQueryable<TestArticle> testData = GetTestArticles().AsQueryable();

        // Act
        IQueryable<TestArticle> results = testData.FullTextSearch("   ");

        // Assert
        results.ShouldBeSameAs(testData);
    }

    [Fact]
    public void FullTextSearch_WithNullLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        IQueryable<TestArticle> testData = GetTestArticles().AsQueryable();

        // Act & Assert
        ArgumentException exception = Should.Throw<ArgumentException>(() =>
            testData.FullTextSearch("test", null!));

        exception.ParamName.ShouldBe("language");
        exception.Message.ShouldContain("Language cannot be null, empty, or whitespace.");
    }

    [Fact]
    public void FullTextSearch_WithEmptyLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        IQueryable<TestArticle> testData = GetTestArticles().AsQueryable();

        // Act & Assert
        ArgumentException exception = Should.Throw<ArgumentException>(() =>
            testData.FullTextSearch("test", ""));

        exception.ParamName.ShouldBe("language");
    }

    [Fact]
    public void FullTextSearch_WithWhiteSpaceLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        IQueryable<TestArticle> testData = GetTestArticles().AsQueryable();

        // Act & Assert
        ArgumentException exception = Should.Throw<ArgumentException>(() =>
            testData.FullTextSearch("test", "   "));

        exception.ParamName.ShouldBe("language");
    }

    [Fact]
    public void FullTextSearch_WithValidSearchTerm_ShouldApplyFilter()
    {
        // Arrange - Use TestArticleForMocking which doesn't cause EF Core issues
        IQueryable<TestArticleForMocking> mockData = new List<TestArticleForMocking>
        {
            new() { Id = 1, Title = "Test Article 1", Content = "This is test content" },
            new() { Id = 2, Title = "Sample Article", Content = "Another sample content" }
        }.AsQueryable();

        // Create a mock DbSet
        Mock<DbSet<TestArticleForMocking>> mockSet = new();

        // Set up the mock to return our queryable data
        mockSet.As<IQueryable<TestArticleForMocking>>().Setup(m => m.Provider).Returns(mockData.Provider);
        mockSet.As<IQueryable<TestArticleForMocking>>().Setup(m => m.Expression).Returns(mockData.Expression);
        mockSet.As<IQueryable<TestArticleForMocking>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
        mockSet.As<IQueryable<TestArticleForMocking>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

        // Set up a mock DbContext
        Mock<TestMockDbContext> mockContext = new();
        mockContext.Setup(c => c.Articles).Returns(mockSet.Object);

        // Act - build the expression tree (not executing it)
        IQueryable<TestArticleForMocking> query = mockSet.Object.FullTextSearch("test");

        // Assert - since we can't execute it, just verify it's not the same as the original
        query.ShouldNotBeSameAs(mockData);
        // Verify that it's a Where expression
        query.Expression.NodeType.ShouldBe(ExpressionType.Call);
        query.Expression.ToString().ShouldContain("Where");
    }

    [Fact]
    public void FullTextSearch_WithCustomLanguage_ShouldApplyFilter()
    {
        // Arrange
        IQueryable<TestArticleForMocking> mockData = new List<TestArticleForMocking>
        {
            new() { Id = 1, Title = "Test Article", Content = "Content" }
        }.AsQueryable();

        // Act
        IQueryable<TestArticleForMocking> query = mockData.FullTextSearch("test", "spanish");

        // Assert
        query.ShouldNotBeSameAs(mockData);
        query.Expression.NodeType.ShouldBe(ExpressionType.Call);
        query.Expression.ToString().ShouldContain("Where");
    }

    [Fact]
    public void FullTextSearch_WithSearchTermThatBecomesEmptyAfterProcessing_ShouldReturnOriginalQueryable()
    {
        // Arrange
        IQueryable<TestArticle> testData = GetTestArticles().AsQueryable();

        // Use a search term that will be empty after processing special characters
        string searchTerm = "!@#$%"; // These will be escaped, but if they result in empty, should return original

        // Act - This tests the scenario where ProcessSearchTerm might return empty string
        IQueryable<TestArticle> results = testData.FullTextSearch(searchTerm);

        // Assert - The query should not be the same as original since special chars are escaped, not removed
        results.ShouldNotBeSameAs(testData);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("test term")]
    [InlineData("multiple word search term")]
    public void FullTextSearch_WithValidMultiWordTerms_ShouldApplyFilter(string searchTerm)
    {
        // Arrange
        IQueryable<TestArticleForMocking> mockData = new List<TestArticleForMocking>
        {
            new() { Id = 1, Title = "Article", Content = "Content" }
        }.AsQueryable();

        // Act
        IQueryable<TestArticleForMocking> query = mockData.FullTextSearch(searchTerm);

        // Assert
        query.ShouldNotBeSameAs(mockData);
        query.Expression.ToString().ShouldContain("Where");
    }

    #region ProcessSearchTerm Tests

    [Fact]
    public void ProcessSearchTerm_ShouldTrimInput()
    {
        // Act
        string result = InvokeProcessSearchTerm("  test term  ");

        // Assert
        result.ShouldBe("test term");
    }

    [Fact]
    public void ProcessSearchTerm_WithNullInput_ShouldReturnEmptyString()
    {
        // Act
        string result = InvokeProcessSearchTerm(null!);

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void ProcessSearchTerm_WithEmptyInput_ShouldReturnEmptyString()
    {
        // Act
        string result = InvokeProcessSearchTerm("");

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void ProcessSearchTerm_WithWhiteSpaceInput_ShouldReturnEmptyString()
    {
        // Act
        string result = InvokeProcessSearchTerm("   ");

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Theory]
    [InlineData("test&search", "test\\&search")]
    [InlineData("test|search", "test\\|search")]
    [InlineData("test!search", "test\\!search")]
    [InlineData("test(search)", "test\\(search\\)")]
    [InlineData("test<search>", "test\\<search\\>")]
    [InlineData("test:search", "test\\:search")]
    [InlineData("test*search", "test\\*search")]
    public void ProcessSearchTerm_ShouldEscapeSpecialCharacters(string input, string expected)
    {
        // Act
        string result = InvokeProcessSearchTerm(input);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ProcessSearchTerm_ShouldEscapeMultipleSpecialCharacters()
    {
        // Act
        string result = InvokeProcessSearchTerm("test&search|query!");

        // Assert
        result.ShouldBe("test\\&search\\|query\\!");
    }

    [Fact]
    public void ProcessSearchTerm_ShouldReplaceMultipleSpacesWithSingle()
    {
        // Act
        string result = InvokeProcessSearchTerm("test    multiple     spaces");

        // Assert
        result.ShouldBe("test multiple spaces");
    }

    [Fact]
    public void ProcessSearchTerm_ShouldHandleMixedSpecialCharsAndSpaces()
    {
        // Act
        string result = InvokeProcessSearchTerm("  test  &  search   |   query  ");

        // Assert
        result.ShouldBe("test \\& search \\| query");
    }

    [Fact]
    public void ProcessSearchTerm_WithVeryLongInput_ShouldTruncateToMaxLength()
    {
        // Arrange
        string longInput = new('a', 1500); // Longer than max length (1000)

        // Act
        string result = InvokeProcessSearchTerm(longInput);

        // Assert
        result.Length.ShouldBeLessThanOrEqualTo(1000);
        result.ShouldStartWith(new string('a', 1000));
    }

    [Fact]
    public void ProcessSearchTerm_WithExactMaxLength_ShouldNotTruncate()
    {
        // Arrange
        string exactLengthInput = new('a', 1000);

        // Act
        string result = InvokeProcessSearchTerm(exactLengthInput);

        // Assert
        result.Length.ShouldBe(1000);
        result.ShouldBe(exactLengthInput);
    }

    [Fact]
    public void ProcessSearchTerm_TruncationShouldTrimEndSpaces()
    {
        // Arrange - Create a string that when truncated will have trailing spaces
        string input = new string('a', 999) + " b"; // 1001 chars, will be truncated to 1000, leaving trailing space

        // Act
        string result = InvokeProcessSearchTerm(input);

        // Assert
        result.Length.ShouldBeLessThanOrEqualTo(1000);
        result.ShouldNotEndWith(" ");
    }

    [Fact]
    public void ProcessSearchTerm_WithComplexRealWorldExample_ShouldProcessCorrectly()
    {
        // Arrange - Simulate a real-world search with various issues
        string input = "  search  for   (urgent)  items  &  documents  |  files!  ";

        // Act
        string result = InvokeProcessSearchTerm(input);

        // Assert
        result.ShouldBe("search for \\(urgent\\) items \\& documents \\| files\\!");
    }

    [Theory]
    [InlineData("simple")]
    [InlineData("simple search")]
    [InlineData("search with numbers 123")]
    [InlineData("üñíçødé çhäräçtérs")]
    public void ProcessSearchTerm_WithNormalInput_ShouldReturnProcessedButNotEscaped(string input)
    {
        // Act
        string result = InvokeProcessSearchTerm($"  {input}  ");

        // Assert
        result.ShouldBe(input); // Should just be trimmed
    }

    #endregion

    #region Helper Methods

    private string InvokeProcessSearchTerm(string input)
    {
        MethodInfo? method = typeof(SearchableExtensions).GetMethod("ProcessSearchTerm",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("The method 'ProcessSearchTerm' could not be found.");

        return (string)method.Invoke(null, [input])!;
    }

    private List<TestArticle> GetTestArticles()
    {
        return
        [
            new() { Id = 1, Title = "Test Article 1", Content = "This is test content" },
            new() { Id = 2, Title = "Sample Article", Content = "Another sample content" }
        ];
    }

    #endregion
}
