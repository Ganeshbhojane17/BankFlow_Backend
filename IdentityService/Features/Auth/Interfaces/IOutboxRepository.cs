using IdentityService.Domain.Entities;
using System.Data;

namespace IdentityService.Features.Auth.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message, IDbTransaction transaction);

        Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize);

        Task MarkAsProcessedAsync(long id);

        Task MarkAsFailedAsync(long id, string errorMessage);
    }
}
