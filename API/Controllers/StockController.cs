using API.Const;
using API.Filters;
using API.Models.Base;
using API.Models.Responses;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Services;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly Settings settings;

        public StockController(IConfiguration _configuration, IOptions<Settings> _settings)
        {
            config = _configuration;
            settings = _settings.Value;
        }

        [HttpPost, Route("Create")]
        [Authorize(Roles = Roles.ADMIN, Policy = Policy.Create)]
        public async Task<IActionResult> Create_1([FromBody] Product context)
        {
            if (!TryValidateModel(context))
            {
                string errors = "";
                if (ModelState.ErrorCount > 0)
                    errors = JsonConvert.SerializeObject(ModelState.Where(val => val.Value.Errors.Count > 0).Select(val => new { val.Key, val.Value.Errors }).ToArray());
                throw new Exception(string.Concat(ErrorStatus.STATUS_MSG_MODEL_INVALID, errors));
            }

            IDataAccess db = ServiceInit.GetDataInstance(settings.DataAccess);
            string  productDetails = await db.SaveProductDetails(context);
            ProductResponse Response = new()
            {
                products = JsonConvert.DeserializeObject<List<Product>>(JsonConvert.SerializeObject(productDetails))
            };

            return new Context("").ToContextResult();
        }

        [HttpPost, Route("View")]
        [Authorize(Roles = Roles.ADMIN + "," + Roles.USER + "," + Roles.AUDITOR, Policy = Policy.View)]
        public async Task<IActionResult> View_1()
        {
            IDataAccess db = ServiceInit.GetDataInstance(settings.DataAccess);           
            DataTable productDetails = await db.GetProductDetails();
            ProductResponse Response = new()
            {     
                products = JsonConvert.DeserializeObject<List<Product>>(JsonConvert.SerializeObject(productDetails))
            };
            return new Context(Response).ToContextResult(); ;
        }

        [HttpPost, Route("Edit")]
        [Authorize(Roles = Roles.ADMIN + "," + Roles.USER, Policy = Policy.Edit)]
        public async Task<IActionResult> Edit_1([FromBody] Product context)
        {
            if (!TryValidateModel(context))
            {
                string errors = "";
                if (ModelState.ErrorCount > 0)
                    errors = JsonConvert.SerializeObject(ModelState.Where(val => val.Value.Errors.Count > 0).Select(val => new { val.Key, val.Value.Errors }).ToArray());
                throw new Exception(string.Concat(ErrorStatus.STATUS_MSG_MODEL_INVALID, errors));
            }

            IDataAccess db = ServiceInit.GetDataInstance(settings.DataAccess);
            string productDetails = await db.UpdateProductDetails(context);
            ProductResponse Response = new()
            {
                products = JsonConvert.DeserializeObject<List<Product>>(JsonConvert.SerializeObject(productDetails))
            };

            return new Context("").ToContextResult();          
        }
        [HttpPost, Route("ViewID")]
        [Authorize(Roles = Roles.ADMIN + "," + Roles.USER + "," + Roles.AUDITOR, Policy = Policy.View)]
        public async Task<IActionResult> ViewID([FromBody] int ProductID)
        {
            IDataAccess db = ServiceInit.GetDataInstance(settings.DataAccess);
            DataTable productDetails = await db.GetProductDetailsbyID(ProductID);
            ProductResponse Response = new()
            {
                products = JsonConvert.DeserializeObject<List<Product>>(JsonConvert.SerializeObject(productDetails))
            };
            return new Context(Response).ToContextResult(); ;
        }
    }
}
