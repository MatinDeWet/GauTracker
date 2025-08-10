using FluentValidation;
using Pagination.Models.Requests;

namespace GauTracker.API.Common.Validation;

/// <summary>
/// Provides extension methods for validating PageableRequest properties.
/// </summary>
public static class PageableRequestValidatorExtensions
{
    /// <summary>
    /// Validates all properties of a PageableRequest at once.
    /// </summary>
    /// <typeparam name="T">The type that inherits from PageableRequest.</typeparam>
    /// <param name="validator">The validator being configured.</param>
    /// <param name="maxPageSize">The maximum allowed page size (default: 100).</param>
    /// <param name="isOrderByRequired">Whether the OrderBy property is required (default: false).</param>
    /// <param name="maxOrderByLength">The maximum length of the OrderBy property (default: 100).</param>
    /// <returns>The validator with PageableRequest validation rules applied.</returns>
    public static AbstractValidator<T> ValidatePageableRequest<T>(
        this AbstractValidator<T> validator,
        int maxPageSize = 100,
        bool isOrderByRequired = false,
        int maxOrderByLength = 100) where T : PageableRequest
    {
        validator.RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        validator.RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.")
            .LessThanOrEqualTo(maxPageSize)
            .WithMessage("{PropertyName} cannot exceed {ComparisonValue}.");

        IRuleBuilderInitial<T, string?> orderByRule = validator.RuleFor(x => x.OrderBy);

        if (isOrderByRequired)
        {
            orderByRule
                .NotEmpty()
                .WithMessage("{PropertyName} is required.");
        }

        orderByRule
            .MaximumLength(maxOrderByLength)
            .WithMessage("{PropertyName} must not exceed {MaxLength} characters.");

        return validator;
    }

    /// <summary>
    /// Configures validation rules for PageNumber property of PageableRequest.
    /// </summary>
    /// <typeparam name="T">The type containing the PageNumber property.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <returns>Rule builder options for the PageNumber property.</returns>
    public static IRuleBuilderOptions<T, int> ValidPageNumber<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");
    }

    /// <summary>
    /// Configures validation rules for PageSize property of PageableRequest.
    /// </summary>
    /// <typeparam name="T">The type containing the PageSize property.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="maxPageSize">The maximum allowed page size (optional).</param>
    /// <returns>Rule builder options for the PageSize property.</returns>
    public static IRuleBuilderOptions<T, int> ValidPageSize<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        int maxPageSize = 100)
    {
        return ruleBuilder
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.")
            .LessThanOrEqualTo(maxPageSize)
            .WithMessage("{PropertyName} cannot exceed {ComparisonValue}.");
    }

    /// <summary>
    /// Configures validation rules for OrderBy property of PageableRequest.
    /// </summary>
    /// <typeparam name="T">The type containing the OrderBy property.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="isRequired">Whether the OrderBy property is required.</param>
    /// <param name="maxLength">The maximum length of the OrderBy property.</param>
    /// <returns>Rule builder options for the OrderBy property.</returns>
    public static IRuleBuilderOptions<T, string?> ValidOrderBy<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        bool isRequired = false,
        int maxLength = 100)
    {
        if (isRequired)
        {
            ruleBuilder = ruleBuilder
                .NotEmpty()
                .WithMessage("{PropertyName} is required.");
        }

        return ruleBuilder
            .MaximumLength(maxLength)
            .WithMessage("{PropertyName} must not exceed {MaxLength} characters.");
    }
}
