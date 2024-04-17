using System.Data.SqlClient;

namespace Feature.Core.Config
{
    public class ConfigBase
    {
        public string ConnectionString { get; set; }

        public SqlConnectionStringBuilder SqlConnectionStringBuilder
        {
            get {
                return new SqlConnectionStringBuilder(ConnectionString);
            }
        }
    }

}