using CustomerService.Application.Common.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Persistence.Dapper;
using Dapper;
using System.Data;

namespace CustomerService.Infrastructure.Persistence.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly DapperContext _context;
        public OutboxRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OutboxMessage message, IDbConnection connection,IDbTransaction transaction)
        {
            await connection.ExecuteAsync(
                "usp_Outbox_Insert",
                new
                {
                    message.EventId,
                    message.EventType,
                    message.RoutingKey,
                    message.Payload
                },
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<OutboxMessage>(
                "dbo.usp_Outbox_GetPending",
                new
                {
                    BatchSize = batchSize
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task MarkAsProcessedAsync(long id)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync("dbo.usp_Outbox_MarkProcessed",
                new
                {
                    Id = id
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task MarkAsFailedAsync(long id, string errorMessage)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync("dbo.usp_Outbox_MarkFailed",
                new
                {
                    Id = id,
                    ErrorMessage = errorMessage
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
