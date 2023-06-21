using API.Enums;
using API.Models.Base;
using System.Data.Common;

namespace API.Services
{
    public class ServiceInit
    {
        public static dynamic GetDataInstance(DB providerInfo)
        {
            switch (providerInfo.Provider)
            {
                case DBProvider.MSSQL:
                    DbProviderFactories.RegisterFactory(DBProvider.MSSQL.ToString(), System.Data.SqlClient.SqlClientFactory.Instance);
                    break;
                case DBProvider.MySQL:
                    DbProviderFactories.RegisterFactory(DBProvider.MySQL.ToString(), MySqlConnector.MySqlConnectorFactory.Instance);
                    break;
                case DBProvider.Oracle:
                    DbProviderFactories.RegisterFactory(DBProvider.Oracle.ToString(), Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
                    break;
            }

            return new DataAccess(providerInfo);
        }
        public static dynamic GetIDPInstance(Service providerInfo)
        {
            return new IDP(providerInfo);
        }
    }
}
