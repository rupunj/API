using API.Enums;
using API.Models.Base;

namespace API.Models.Responses
{
    public class ProductResponse
    {
        public List<Product> products { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }



}

