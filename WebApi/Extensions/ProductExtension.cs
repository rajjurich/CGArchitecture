using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ViewModels.ResponseModel;

namespace WebApi.Extensions
{
    public static class ProductExtension
    {        
        public static ProductResponseModel ToProductResponseModel(this Product model)
        {
            return new ProductResponseModel()
            {
                ProductDescription = model.ProductDescription,
                ProductId = model.ProductId,
                ProductImage = model.ProductImage,
                ProductName = model.ProductName,
                ProductPrice = model.ProductPrice
            };
        }
    }
}