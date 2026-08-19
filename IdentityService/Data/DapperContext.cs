using IdentityService.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace IdentityService.Data
{
    public class DapperContext
    {
        //private readonly IConfiguration _configuration;
        //public DapperContext(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //public IDbConnection CreateConnection()
        //{
        //    return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //}

        private readonly DatabaseOptions _databaseOptions;

        public DapperContext(IOptions<DatabaseOptions> options)
        {
            _databaseOptions = options.Value;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_databaseOptions.DefaultConnection);
        }

    }
}
