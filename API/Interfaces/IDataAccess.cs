using API.Models.Responses;
using System.Data;

namespace API.Interfaces
{
    public interface IDataAccess
    {
        public Task<DataTable> GetUserScopes(string userName);

        public Task<DataTable> GetProductDetails();

        public  Task<string> SaveProductDetails(Product product);
        public  Task<string> UpdateProductDetails(Product product);

        public Task<DataTable> GetProductDetailsbyID(int ProductID);
    }
}
