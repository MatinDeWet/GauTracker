using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Gautrain.Integration.Csv.Mappers;
using Gautrain.Integration.Csv.Models;

namespace Gautrain.Integration.Csv;
public class TravelHistoryCsvReader : ITravelHistoryCsvReader
{
    private readonly CsvConfiguration _config;

    public TravelHistoryCsvReader()
    {
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim
        };
    }

    public async Task<ICollection<TravelHistoryCsvModel>> ReadAsync(Stream csvStream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, _config);
        csv.Context.RegisterClassMap<TravelHistoryCsvMap>();

        var uniqueRecords = new Dictionary<int, TravelHistoryCsvModel>();

        await foreach (TravelHistoryCsvModel? record in csv.GetRecordsAsync<TravelHistoryCsvModel>(cancellationToken))
        {
            uniqueRecords[record.SequenceNumber] = record;
        }

        return uniqueRecords.Values;
    }
}
