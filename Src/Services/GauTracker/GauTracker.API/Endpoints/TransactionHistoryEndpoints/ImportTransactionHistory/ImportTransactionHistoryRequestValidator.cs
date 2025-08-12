using BlobStorage.Common.Extensions;
using FastEndpoints;
using FluentValidation;

namespace GauTracker.API.Endpoints.TransactionHistoryEndpoints.ImportTransactionHistory;

public class ImportTransactionHistoryRequestValidator : Validator<ImportTransactionHistoryApiRequest>
{
    public ImportTransactionHistoryRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required");

        RuleFor(x => x.File.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .When(x => x.File != null);

        RuleFor(x => x.File.Length)
            .GreaterThan(0)
            .WithMessage("File cannot be empty")
            .LessThanOrEqualTo(10.ToBytes())
            .WithMessage("File size cannot exceed 100MB")
            .When(x => x.File != null);

        RuleFor(x => x.File.ContentType)
            .Must(contentType => contentType.IsValidContentType("text/csv", "application/csv", "application/vnd.ms-excel", "text/plain"))
            .WithMessage("Only CSV files are allowed")
            .When(x => x.File != null);

        RuleFor(x => x.File.FileName)
            .Must(fileName => fileName.IsValidFileExtension(".csv"))
            .WithMessage("Only .csv files are allowed")
            .When(x => x.File != null && !string.IsNullOrEmpty(x.File.FileName));

        RuleFor(x => x.CardId)
            .NotEmpty();
    }
}
