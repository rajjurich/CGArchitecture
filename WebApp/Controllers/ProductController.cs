using Client;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModels.RequestModel;
using ViewModels.ResponseModel;

namespace WebApp.Controllers
{
    public class ProductController : BaseController
    {
        IProductClient productClient;
        IFileUploadClient fileUploadClient;
        public ProductController()
        {
            this.productClient = (IProductClient)new ProductClient();
            this.fileUploadClient = (IFileUploadClient)new FileUploadClient();
        }

        // GET: Product
        public async Task<ActionResult> Index()
        {
            HttpResponseMessage result = await productClient.Get();
            List<ProductResponseModel> products = null;

            if (result.IsSuccessStatusCode)
            {
                products = await result.Content.ReadAsAsync<List<ProductResponseModel>>();
            }

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return await getById(id);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCreateModel collection, HttpPostedFileBase UploadImage)
        {
            // TODO: Add insert logic here
            if (ModelState.IsValid)
            {
                if (UploadImage != null)
                {
                    HttpResponseMessage resultFile = await fileUploadClient.UploadFile(UploadImage);
                    var contentsFile = await resultFile.Content.ReadAsStringAsync();
                    if (resultFile.IsSuccessStatusCode)
                    {
                        var uniqueId = resultFile.Content.ReadAsAsync<string>().Result;
                        collection.ProductImage = uniqueId + "_" + UploadImage.FileName;

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, contentsFile);
                        return View();
                    }
                }

                HttpResponseMessage result = await productClient.Add(collection);
                var contents = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    ProductResponseModel productResponseModel = JsonConvert.DeserializeObject<ProductResponseModel>(contents);
                    return RedirectToAction("Details", new { id = productResponseModel.ProductId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, contents);
                    return View();
                }
            }
            else
            {
                return View();
            }

        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return await getById(id);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ProductResponseModel collection, HttpPostedFileBase UploadImage)
        {
            // TODO: Add update logic here
            if (ModelState.IsValid)
            {
                if (UploadImage != null)
                {
                    HttpResponseMessage resultFile = await fileUploadClient.UploadFile(UploadImage);
                    var contentsFile = await resultFile.Content.ReadAsStringAsync();
                    if (resultFile.IsSuccessStatusCode)
                    {
                        var uniqueId = resultFile.Content.ReadAsAsync<string>().Result;
                        collection.ProductImage = uniqueId + "_" + UploadImage.FileName;

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, contentsFile);
                        return View();
                    }
                }

                HttpResponseMessage result = await productClient.Edit(id, collection);
                var contents = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    ProductResponseModel productResponseModel = JsonConvert.DeserializeObject<ProductResponseModel>(contents);
                    return RedirectToAction("Details", new { id = productResponseModel.ProductId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, contents);
                    return View();
                }

            }
            else
            {
                return View();
            }
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return await getById(id);
        }

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            // TODO: Add delete logic here
            HttpResponseMessage result = await productClient.Delete(id);
            var contents = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                ProductResponseModel productResponseModel = JsonConvert.DeserializeObject<ProductResponseModel>(contents);
                return RedirectToAction("Details", new { id = productResponseModel.ProductId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, contents);
                return View();
            }
        }

        public ActionResult Report()
        {
            return View();
        }

        public async Task<ActionResult> Print(int id)
        {
            return await getById(id);
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Print(string GridHtml)
        {

            MemoryStream stream = new System.IO.MemoryStream();
            StringReader sr = new StringReader(GridHtml);
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            string fname = "test_" + Guid.NewGuid().ToString() + ".pdf";
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(@"C:\text\" + fname, FileMode.Create));


            pdfDoc.Open();
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);

            pdfDoc.Close();


            //var f = File(stream.ToArray(), "application/pdf", "Grid.pdf");

            PdfReader pdfFile;
            Document doc;
            PdfWriter pCopy;
            MemoryStream msOutput = new MemoryStream();

            pdfFile = new PdfReader(@"C:\text\" + fname);
            doc = new Document();
            pCopy = new PdfSmartCopy(doc, msOutput);

            doc.Open();

            for (int i = 1; i < pdfFile.NumberOfPages + 1; i++)
            {
                ((PdfSmartCopy)pCopy).AddPage(pCopy.GetImportedPage(pdfFile, i));
            }
            pCopy.FreeReader(pdfFile);

            pdfFile = new PdfReader(@"C:\text\Report.pdf");

            for (int i = 1; i < pdfFile.NumberOfPages + 1; i++)
            {
                ((PdfSmartCopy)pCopy).AddPage(pCopy.GetImportedPage(pdfFile, i));
            }
            pCopy.FreeReader(pdfFile);


            pdfFile.Close();
            pCopy.Close();
            doc.Close();

            return File(msOutput.ToArray(), "application/pdf", "Grid.pdf");


        }
        private async Task<ActionResult> getById(int id)
        {
            HttpResponseMessage result = await productClient.Get(id);

            ProductResponseModel product = null;

            if (result.IsSuccessStatusCode)
            {
                product = await result.Content.ReadAsAsync<ProductResponseModel>();
            }

            return View(product);
        }
    }
}
