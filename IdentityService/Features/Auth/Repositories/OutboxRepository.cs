using Dapper;
using IdentityService.Data;
using IdentityService.Domain.Entities;
using IdentityService.Features.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
namespace IdentityService.Features.Auth.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly DapperContext _context;
        public OutboxRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OutboxMessage message, IDbTransaction transaction)
        {
            using var connection = _context.CreateConnection();

            var parameters = new
            {
                message.EventId,
                message.EventType,
                message.RoutingKey,
                message.Payload
            };

            await transaction.Connection!.ExecuteAsync(
           "dbo.usp_Outbox_Insert",
           parameters,
           transaction: transaction,
           commandType: CommandType.StoredProcedure);
        }

        public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize)
        {
            using var connection = _context.CreateConnection();
            var parameters = new
            {
                BatchSize = batchSize
            };

            var result = await connection.QueryAsync<OutboxMessage>(
                    "dbo.usp_Outbox_GetPending",
                    parameters,
                    commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task MarkAsProcessedAsync(long id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new
            {
                Id = id
            };

            await connection.ExecuteAsync(
                "dbo.usp_Outbox_MarkProcessed",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task MarkAsFailedAsync(long id, string errorMessage)
        {
            using var connection =_context.CreateConnection();

            var parameters = new
            {
                Id = id,
                ErrorMessage = errorMessage
            };

            await connection.ExecuteAsync(
                "dbo.usp_Outbox_MarkFailed",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}