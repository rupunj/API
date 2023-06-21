using API.Interfaces;
using API.Models.Base;
using API.Models.Responses;
using System.Data;
using System.Data.Common;

namespace API.Services
{
    public class DataAccess : IDataAccess
    {
        private protected DB Config { get; set; }
        private protected DbProviderFactory Factory { get; set; }

        public DataAccess(DB config)
        {
            Config = config;
            Factory = DbProviderFactories.GetFactory(Config.Provider.ToString());
        }

        public async Task<DataTable> GetUserScopes(string username)
        {
            try
            {
                using DbConnection conn = Factory.CreateConnection();
                conn.ConnectionString = Config.ConnectionString;
                DataTable result = new();

                using DbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "uspGetUserScopesByUsername";
                cmd.Connection = conn;

                DbParameter param1 = Factory.CreateParameter();
                param1.ParameterName = "@Username";
                param1.Value = username;

                cmd.Parameters.Add(param1);

                using DbDataAdapter adt = Factory.CreateDataAdapter();
                adt.SelectCommand = cmd;
                adt.Fill(result);

                return result;
            }
            catch
            {
                throw;
            }
        }
        public async Task<DataTable> GetProductDetails()
        {
            try
            {
                using DbConnection conn = Factory.CreateConnection();
                conn.ConnectionString = Config.ConnectionString;
                DataTable result = new();

                using DbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAllProducts";
                cmd.Connection = conn;

                using DbDataAdapter adt = Factory.CreateDataAdapter();
                adt.SelectCommand = cmd;
                adt.Fill(result);

                return result;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> SaveProductDetails(Product product)
        {
            {
                try
                {
                    using DbConnection conn = Factory.CreateConnection();
                    conn.ConnectionString = Config.ConnectionString;
                    conn.Open();
                    DataTable result = new();
                    DbTransaction trans = conn.BeginTransaction();

                    try
                    {
                        using DbCommand cmd = conn.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "InsertProduct";
                        cmd.Connection = conn;
                        cmd.Transaction = trans;

                        DbParameter param1 = Factory.CreateParameter();
                        param1.ParameterName = "@ProductName";
                        param1.Value = product.Name;

                        DbParameter param2 = Factory.CreateParameter();
                        param2.ParameterName = "@Price";
                        param2.Value = product.Price;

                        DbParameter param3 = Factory.CreateParameter();
                        param3.ParameterName = "@Quantity";
                        param3.Value = product.Quantity;


                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);

                        cmd.ExecuteNonQuery();
                        trans.Commit();
                        conn.Close();

                        return "OK";
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
        public async Task<string> UpdateProductDetails(Product product)
        {
            {
                try
                {
                    using DbConnection conn = Factory.CreateConnection();
                    conn.ConnectionString = Config.ConnectionString;
                    conn.Open();
                    DataTable result = new();
                    DbTransaction trans = conn.BeginTransaction();

                    try
                    {
                        using DbCommand cmd = conn.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "UpdateProduct";
                        cmd.Connection = conn;
                        cmd.Transaction = trans;

                        DbParameter param1 = Factory.CreateParameter();
                        param1.ParameterName = "@ProductId";
                        param1.Value = product.Id;


                        DbParameter param2 = Factory.CreateParameter();
                        param2.ParameterName = "@ProductName";
                        param2.Value = product.Name;

                        DbParameter param3 = Factory.CreateParameter();
                        param3.ParameterName = "@Price";
                        param3.Value = product.Price;

                        DbParameter param4 = Factory.CreateParameter();
                        param4.ParameterName = "@Quantity";
                        param4.Value = product.Quantity;


                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);

                        cmd.ExecuteNonQuery();
                        trans.Commit();
                        conn.Close();

                        return "OK";
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
        public async Task<DataTable> GetProductDetailsbyID(int productID)
        {
            try
            {
                using DbConnection conn = Factory.CreateConnection();
                conn.ConnectionString = Config.ConnectionString;
                DataTable result = new();

                using DbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetProductDetails";
                cmd.Connection = conn;

                DbParameter param1 = Factory.CreateParameter();
                param1.ParameterName = "@ProductId";
                param1.Value = productID;

                cmd.Parameters.Add(param1);

                using DbDataAdapter adt = Factory.CreateDataAdapter();
                adt.SelectCommand = cmd;
                adt.Fill(result);

                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
