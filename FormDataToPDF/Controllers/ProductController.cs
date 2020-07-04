using FormDataToPDF.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormDataToPDF.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Product product, HttpPostedFileBase AttachFile)
        {
            string extension = string.Empty;// Path.GetExtension(upload.FileName);
            //StringReader sr = new StringReader(GridHtml);
            var root = Server.MapPath("~/App_Data/");

            string filePath = string.Empty;
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                extension = Path.GetExtension(file.FileName);
                if (extension != ".pdf")
                {
                    ModelState.AddModelError(string.Empty, "invalid file format");
                    return View();
                }
                filePath = Path.Combine(root, file.FileName);
                file.SaveAs(filePath);
            }


            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            string fname = "test_" + Guid.NewGuid().ToString() + ".pdf";
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(root + fname, FileMode.Create));


            pdfDoc.Open();
            pdfDoc.Add(new Paragraph("Product Name : " + product.ProductName));
            pdfDoc.Add(new Paragraph(""));
            pdfDoc.Add(new Paragraph("Product Description : " + product.ProductDescription));
            pdfDoc.Add(new Paragraph(""));
            pdfDoc.Add(new Paragraph("Product Price : " + product.ProductPrice));
            pdfDoc.Close();

            PdfReader pdfFile;
            Document doc;
            PdfWriter pCopy;
            MemoryStream msOutput = new MemoryStream();

            pdfFile = new PdfReader(root + fname);
            doc = new Document();
            pCopy = new PdfSmartCopy(doc, msOutput);

            doc.Open();

            for (int i = 1; i < pdfFile.NumberOfPages + 1; i++)
            {
                ((PdfSmartCopy)pCopy).AddPage(pCopy.GetImportedPage(pdfFile, i));
            }
            pCopy.FreeReader(pdfFile);

            pdfFile = new PdfReader(filePath);

            for (int i = 1; i < pdfFile.NumberOfPages + 1; i++)
            {
                ((PdfSmartCopy)pCopy).AddPage(pCopy.GetImportedPage(pdfFile, i));
            }
            pCopy.FreeReader(pdfFile);


            pdfFile.Close();
            pCopy.Close();
            doc.Close();

            return File(msOutput.ToArray(), "application/pdf", product.ProductName + ".pdf");
        }
    }
}