using Npgsql;

namespace BugTracker.Services
{
    public class ConnectionService
    {
        #region Get Connection String
        public static string GetConnectionString(IConfiguration configuration)
        {
            //The default connection string will come from appSettings/secrets like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //Updated for AWS
            //It will be automatically overwritten if we are running on AWS
            var databaseUrl = Environment.GetEnvironmentVariable("RDS_HOSTNAME");

            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }
        #endregion

        #region Build Connection String
        private static string BuildConnectionString(string databaseUrl)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = Environment.GetEnvironmentVariable("RDS_HOSTNAME"),
                Port = Int32.Parse(Environment.GetEnvironmentVariable("RDS_PORT")),
                Username = Environment.GetEnvironmentVariable("RDS_USERNAME"),
                Password = Environment.GetEnvironmentVariable("RDS_PASSWORD"),
                Database = Environment.GetEnvironmentVariable("RDS_DB_NAME"),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }
        #endregion
    }
}
