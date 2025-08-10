using Gautrain.Integration.Csv.Models;

namespace Gautrain.Integration.Csv;
public interface ITravelHistoryCsvReader
{
    /// <summary>
    /// Parses the stream containing the CSV and returns collection of TravelHistoryCsvModel.
    /// </summary>
    Task<ICollection<TravelHistoryCsvModel>> ReadAsync(Stream csvStream, CancellationToken cancellationToken = default);
}
