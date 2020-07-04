using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Client
{
    public interface IFileUploadClient
    {
        Task<HttpResponseMessage> UploadFile(HttpPostedFileBase file);
    }
    public class FileUploadClient : IFileUploadClient
    {
        private static HttpClient httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "api/FileUpload/";
        public FileUploadClient()
        {
            httpClient = new HttpClient();
        }
        public async Task<HttpResponseMessage> UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                using (var content = new MultipartFormDataContent())
                {
                    byte[] Bytes = new byte[file.InputStream.Length + 1];
                    file.InputStream.Read(Bytes, 0, Bytes.Length);
                    var fileContent = new ByteArrayContent(Bytes);
                    fileContent.Headers.ContentDisposition = new System.Net.Http.Headers
                    .ContentDispositionHeaderValue("attachment")
                    {
                        FileName = file.FileName
                    };
                    content.Add(fileContent);
                    var requestUri = BaseUriTemplate + "UploadFile";
                    HttpResponseMessage response = await httpClient.PostAsync(requestUri, content);

                    return response;
                }
            }
            return null;

        }
    }
}
