using System.Data.Common;
using Microsoft.Data.SqlClient;
namespace ElectronicsShopApp;
public static class DbHelper
{

    static DbHelper()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
    }

    public static DbConnection CreateConnection()
    {

        DbProviderFactory factory = DbProviderFactories.GetFactory("Microsoft.Data.SqlClient");
        DbConnection connection = factory.CreateConnection();
        connection.ConnectionString = DbConfig.ConnectionString;
        return connection;
    }

    public static DbCommand CreateCommand(string sql, DbConnection connection)
    {
        DbCommand command = connection.CreateCommand();
        command.CommandText = sql;
        return command;
    }
}