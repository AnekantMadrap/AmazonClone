using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AmazonClone.Infrastructure.Data
{
    public interface ISqlDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class SqlDbConnectionFactory : ISqlDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlDbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }
    }
}
