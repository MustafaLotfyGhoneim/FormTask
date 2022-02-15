using FormTask.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FormTask.Controllers
{
    public class HomeController : Controller
    {
        MyContext db = new MyContext();
        [HttpGet]
        public ActionResult Index()
        {
            List<formData> lst = db.formDatas.ToList();
            return View(lst);
        }
        public ActionResult create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult create(formData data)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(data.ImageFile.FileName);
                string extension = Path.GetExtension(data.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                data.Image = "~/Images/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images/"), fileName);
                data.ImageFile.SaveAs(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                using (MyContext db = new MyContext())
                {
                    db.formDatas.Add(data);
                    db.SaveChanges();
                }
            }
            // for catching validation errors on inputs
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Console.WriteLine(e.Message);
            }

            ModelState.Clear();
            return RedirectToAction("Index");
        }
        // using Rotativa nuget package, you can export pdf files
        public ActionResult PrintViewToPdf()
        {
            var report = new ActionAsPdf("Index");
            return report;
        }
        // for uploading files to FTP server
        public ActionResult UploadFTP()
        {
            return View();
        }
            [HttpPost, ValidateInput(false)]
        public ActionResult UploadFTP(formData data)
        {

            if (data.ImageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileNameWithoutExtension(data.ImageFile.FileName);
                string extension = Path.GetExtension(data.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                data.Image = "~/Images/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images/"), fileName);
                data.ImageFile.SaveAs(fileName);
                //string fileName = HttpContext.Server.MapPath(@"~/dirname/readme.txt");
                //string fileName = Path.Combine(data.ImageFile.FileName);
                var fileInf = new FileInfo(fileName);
                var reqFtp =
                    (System.Net.FtpWebRequest)
                        System.Net.FtpWebRequest.Create(
                            new Uri("ftp://192.168.1.3/" + fileInf.Name));
                string username = "ftp-user";
                string password = "123";
                reqFtp.Credentials = new NetworkCredential(username, password);
                reqFtp.KeepAlive = false;
                reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
                reqFtp.UseBinary = true;
                reqFtp.ContentLength = data.ImageFile.ContentLength;
                int bufferlength = 2048;
                byte[] buff = new byte[bufferlength];
                int contentLen;
                FileStream fs = fileInf.OpenRead();

                try
                {
                    Stream strm = reqFtp.GetRequestStream();
                    contentLen = fs.Read(buff, 0, bufferlength);
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, bufferlength);
                    }
                    strm.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            db.formDatas.Add(data);
            db.SaveChanges();
            ModelState.Clear();
            return RedirectToAction("Index");
        }
 
    public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}