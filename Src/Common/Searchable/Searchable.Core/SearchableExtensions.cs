using Microsoft.EntityFrameworkCore;
using Searchable.Domain;

namespace Searchable.Core;

public static class SearchableExtensions
{
    /// <summary>
    /// Searches entities implementing ISearchableModel using PostgreSQL full-text search
    /// </summary>
    /// <typeparam name="T">Entity type that implements ISearchableModel</typeparam>
    /// <param name="queryable">The queryable to filter</param>
    /// <param name="searchTerm">The search term</param>
    /// <param name="language">The language for text search (defaults to English)</param>
    /// <returns>Filtered queryable with entities matching the search term</returns>
    public static IQueryable<T> FullTextSearch<T>(
        this IQueryable<T> queryable,
        string? searchTerm,
        string language = "english")
        where T : class, ISearchableEntity
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return queryable;
        }

        // Validate language parameter
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Language cannot be null, empty, or whitespace.", nameof(language));
        }

        // Process search term to handle multiple words and clean input
        string processedSearchTerm = ProcessSearchTerm(searchTerm);

        // Return early if processed term is empty after cleaning
        if (string.IsNullOrWhiteSpace(processedSearchTerm))
        {
            return queryable;
        }

        return queryable.Where(e =>
            e.SearchVector.Matches(EF.Functions.PlainToTsQuery(language, processedSearchTerm)));
    }

    /// <summary>
    /// Processes the search term to ensure proper formatting for PostgreSQL full-text search
    /// </summary>
    /// <param name="searchTerm">The raw search term input</param>
    /// <returns>A processed search term safe for PostgreSQL full-text search</returns>
    internal static string ProcessSearchTerm(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return string.Empty;
        }

        // Trim whitespace
        searchTerm = searchTerm.Trim();

        // Handle empty string after trimming
        if (string.IsNullOrEmpty(searchTerm))
        {
            return string.Empty;
        }

        // Escape special PostgreSQL characters that could cause issues
        // These characters have special meaning in PostgreSQL full-text search
        char[] specialChars = ['&', '|', '!', '(', ')', '<', '>', ':', '*'];

        foreach (char specialChar in specialChars)
        {
            searchTerm = searchTerm.Replace(specialChar.ToString(), $"\\{specialChar}");
        }

        // Replace multiple consecutive spaces with single space
        while (searchTerm.Contains("  "))
        {
            searchTerm = searchTerm.Replace("  ", " ");
        }

        // Limit the length to prevent potential performance issues
        // PostgreSQL can handle long queries, but extremely long ones may cause issues
        const int maxSearchTermLength = 1000;
        if (searchTerm.Length > maxSearchTermLength)
        {
            searchTerm = searchTerm[..maxSearchTermLength].TrimEnd();
        }

        return searchTerm;
    }
}
