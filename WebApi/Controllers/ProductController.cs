using Domain.Core;
using Domain.Entities;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ViewModels.RequestModel;
using ViewModels.ResponseModel;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    public class ProductController : ApiController
    {
        IProductService productService;
        private static readonly Expression<Func<Product, ProductResponseModel>> AsProductResponseModel =
            x => new ProductResponseModel
            {
                ProductDescription = x.ProductDescription,
                ProductId = x.ProductId,
                ProductImage = x.ProductImage,
                ProductName = x.ProductName,
                ProductPrice = x.ProductPrice
            };

        public ProductController()
        {
            this.productService = (IProductService)new ProductService();
        }
        // GET: api/Product
        public IQueryable<ProductResponseModel> Get()
        {
            return productService.Get().Select(AsProductResponseModel);
        }

        // GET: api/Product/5
        public async Task<IHttpActionResult> Get(int id)
        {
            var model = await productService.Get(id);
            if (model == null)
            {
                return BadRequest();
            }

            return Ok(model.ToProductResponseModel());
        }

        // POST: api/Product
        public async Task<IHttpActionResult> Post([FromBody]ProductCreateModel entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Product()
            {
                ProductDescription = entity.ProductDescription,
                ProductImage = GetAbsolutePath(string.IsNullOrWhiteSpace(entity.ProductImage) ? "111.jpg" : entity.ProductImage),
                ProductName = entity.ProductName,
                ProductPrice = entity.ProductPrice

            };
            var added = await productService.Add(model);

            return CreatedAtRoute("DefaultApi", new { id = added.ProductId }, model.ToProductResponseModel());
        }

        [Route("api/Product/Report")]
        public async Task<IHttpActionResult> Report()
        {
            var ctx = HttpContext.Current;
            int start = Convert.ToInt32(ctx.Request["start"]);
            int length = Convert.ToInt32(ctx.Request["length"]);
            string sortDirection = ctx.Request["order[0][dir]"];
            string searchValue = ctx.Request["search[value]"];
            int sortCol = Convert.ToInt32(ctx.Request["order[0][column]"]);
            string sortColumnName = ctx.Request["columns[" + sortCol + "][data]"];

            IQueryable<ProductResponseModel> datas;

            if (!(string.IsNullOrWhiteSpace(searchValue)))
            {
                datas = await Task.Run(() => productService.FindBy(
                 x => x.ProductName.ToLower().Contains(searchValue.ToLower())
                 || x.ProductPrice.ToString().Contains(searchValue)

                 ).OrderBy(sortColumnName + " " + sortDirection).Skip(start).Take(length).Select(AsProductResponseModel));
            }
            else
            {
                datas = await Task.Run(() => productService.Get()
                .OrderBy(sortColumnName + " " + sortDirection).Skip(start).Take(length).Select(AsProductResponseModel));
            }

            var jsonData = new
            {
                draw = ctx.Request["draw"],
                recordsTotal = await productService.Count(),
                recordsFiltered = await productService.Count(),
                data = datas
            };

            return Ok(jsonData);
        }

        private static string GetAbsolutePath(string fileName)
        {
            var ctx = HttpContext.Current;
            var imgAbsolutePath = string.Format("{0}://{1}/ProductImages/{2}", ctx.Request.Url.Scheme, ctx.Request.Url.Authority, fileName);
            return imgAbsolutePath;
        }

        // PUT: api/Product/5
        public async Task<IHttpActionResult> Put(int id, [FromBody]ProductResponseModel entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await productService.Get(id);
            if (model == null)
            {
                return BadRequest();
            }

            model.ProductDescription = entity.ProductDescription;
            model.ProductImage = GetAbsolutePath(string.IsNullOrWhiteSpace(entity.ProductImage) ? "111.jpg" : entity.ProductImage);
            model.ProductName = entity.ProductName;
            model.ProductPrice = entity.ProductPrice;

            var updated = await productService.Edit(model);

            return Ok(updated.ToProductResponseModel());
        }

        // DELETE: api/Product/5
        public async Task<IHttpActionResult> Delete(int id)
        {

            var entity = await productService.Get(id);
            if (entity == null)
            {
                return BadRequest();
            }
            var deleted = await productService.Delete(id);

            return Ok(deleted.ToProductResponseModel());
        }
    }
}
