using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class FileUploadController : ApiController
    {
        [HttpPost]
        [Route("api/FileUpload/UploadFile")]
        public async Task<IHttpActionResult> UploadFile()
        {
            var ctx = HttpContext.Current;
            var httpRequest = ctx.Request;
            var root = ctx.Server.MapPath("~/ProductImages");
            var uniqueId = Guid.NewGuid().ToString();
            if (httpRequest.Files.Count > 0)
            {

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = Path.Combine(root, uniqueId + "_" + postedFile.FileName);

                    await Task.Run(() => postedFile.SaveAs(filePath));

                }
                return Ok(uniqueId);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
