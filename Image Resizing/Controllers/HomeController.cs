using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Image_Resizing.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/Images"), fileName);
                    file.SaveAs(path);
                    ResizeImage(path,path+1,);
                }
                ViewBag.Message = "Upload successful";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Uploads");
            }
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



            private static ImageCodecInfo jpgEncoder;

            public static void ResizeImage(string inFile, string outFile, double maxDimension, long level)
            {
                //
                // Load via stream rather than Image.FromFile to release the file
                // handle immediately
                //
                using (Stream stream = new FileStream(inFile, FileMode.Open))
                {
                    using (Image inImage = Image.FromStream(stream))
                    {
                        double width;
                        double height;

                        if (inImage.Height < inImage.Width)
                        {
                            width = maxDimension;
                            height = (maxDimension / (double)inImage.Width) * inImage.Height;
                        }
                        else
                        {
                            height = maxDimension;
                            width = (maxDimension / (double)inImage.Height) * inImage.Width;
                        }
                        using (Bitmap bitmap = new Bitmap((int)width, (int)height))
                        {
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.DrawImage(inImage, 0, 0, bitmap.Width, bitmap.Height);
                                if (inImage.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                                {
                                    if (jpgEncoder == null)
                                    {
                                        ImageCodecInfo[] ici = ImageCodecInfo.GetImageDecoders();
                                        foreach (ImageCodecInfo info in ici)
                                        {
                                            if (info.FormatID == ImageFormat.Jpeg.Guid)
                                            {
                                                jpgEncoder = info;
                                                break;
                                            }
                                        }
                                    }
                                    if (jpgEncoder != null)
                                    {
                                        EncoderParameters ep = new EncoderParameters(1);
                                        ep.Param[0] = new EncoderParameter(Encoder.Quality, level);
                                        bitmap.Save(outFile, jpgEncoder, ep);
                                    }
                                    else
                                        bitmap.Save(outFile, inImage.RawFormat);
                                }
                                else
                                {
                                    //
                                    // Fill with white for transparent GIFs
                                    //
                                    graphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                                    bitmap.Save(outFile, inImage.RawFormat);
                                }
                            }
                        }
                    }
                }
            }

            public static void GetImageSize(string inFile, out int width, out int height)
            {
                using (Stream stream = new FileStream(inFile, FileMode.Open))
                {
                    using (Image src_image = Image.FromStream(stream))
                    {
                        width = src_image.Width;
                        height = src_image.Height;
                    }
                }
            }

    }
}