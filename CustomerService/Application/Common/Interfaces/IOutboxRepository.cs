using CustomerService.Domain.Entities;
using System.Data;
namespace CustomerService.Application.Common.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message, IDbConnection connection, IDbTransaction transaction);
        Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize);

        Task MarkAsProcessedAsync(long id);

        Task MarkAsFailedAsync(long id, string errorMessage);
    }
}
