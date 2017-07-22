using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class ImageController : Controller
    {
        // 图片控制器，管理员可通过该控制器上传图片，用户可通过该控制器看到图片

        // GET: Image
        public ActionResult Index()
        {

            string path = Request.QueryString["path"];
            var filePath = Server.MapPath(string.Format("~/{0}", path + ".png"));

            if (path == null || path.Length == 0 || !System.IO.File.Exists(filePath))
            {
                Bitmap bmp = new Bitmap(200, 200);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Blue);
                g.FillRectangle(Brushes.Red, 2, 2, 65, 31);
                g.DrawString(path.ToString(), new Font("黑体", 15f), Brushes.Yellow, new PointF(5f, 5f));
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                g.Dispose();
                bmp.Dispose();
                return File(ms.ToArray(), "image/jpeg");
            }
                
            
            Image image = Image.FromFile(filePath);
            Bitmap bitmap = new Bitmap(image, 250, 250);
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            image.Dispose();
            return File(memoryStream.ToArray(), "image/png");
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            var fileName = file.FileName;
            string filename = Request["ItemID"];
            if (filename == null || filename.Length == 0)
                filename = "default";
            
            var filePath = Server.MapPath(string.Format("~/{0}", filename + ".png"));
            file.SaveAs(filePath);
            return View();
        }
    }
}