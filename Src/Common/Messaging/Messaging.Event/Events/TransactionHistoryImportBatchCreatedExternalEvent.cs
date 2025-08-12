using Messaging.Event.Common.Implementation;

namespace Messaging.Event.Events;
public record TransactionHistoryImportBatchCreatedExternalEvent(Guid ImportId) : IntegrationEvent;
