using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ViewModels.RequestModel;
using ViewModels.ResponseModel;

namespace Client
{
    public interface IProductClient
    {        
        Task<HttpResponseMessage> Get();
        Task<HttpResponseMessage> Get(int id);
        Task<HttpResponseMessage> Add(ProductCreateModel model);
        Task<HttpResponseMessage> Edit(int id, ProductResponseModel model);
        Task<HttpResponseMessage> Delete(int id);        
    }
    public class ProductClient : IProductClient
    {
        private static HttpClient httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "api/product";
        public ProductClient()
        {
            httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> Add(ProductCreateModel model)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUriTemplate, model);
            return response;
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUriTemplate}/{id}");
            return response;
        }

        public async Task<HttpResponseMessage> Edit(int id, ProductResponseModel model)
        {
            HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUriTemplate}/{id}", model);
            return response;
        }

        public async Task<HttpResponseMessage> Get(int id)
        {
            HttpResponseMessage response = await httpClient.GetAsync($"{BaseUriTemplate}/{id}");
            return response;
        }

        public async Task<HttpResponseMessage> Get()
        {
            HttpResponseMessage response = await httpClient.GetAsync(BaseUriTemplate);
            return response;
        }
    }
}
