using Feature.Core.Config;
using System.Data.SqlClient;

namespace Feature.Core
{
    public class FeatureRepositoryBase
    {
        private ConfigBase _configBase;

        public FeatureRepositoryBase(ConfigBase configBase)
        {
            _configBase = configBase;
        }


        public SqlConnection GetConnection()
        {
            return GetConnection(connectionTimeout: _configBase.SqlConnectionStringBuilder.ConnectTimeout);
        }


        public SqlConnection GetConnection(int connectionTimeout)
        {
            var connStringBuilder = new SqlConnectionStringBuilder(_configBase.ConnectionString)
            {
                ConnectTimeout = connectionTimeout
            };

            return new SqlConnection(connStringBuilder.ConnectionString);
        }
    }
}
