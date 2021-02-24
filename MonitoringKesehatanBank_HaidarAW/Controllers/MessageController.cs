using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonitoringKesehatanBank_HaidarAW.Controllers
{
    public class MessageController : Controller
    {
        // GET: ErrrorMessage
        public ActionResult ErrorNullUpload()
        {
            return View();
        }
        public ActionResult ErrorDataExists()
        {
            return View();
        }
        public ActionResult ErrorFile()
        {
            return View();
        }
        public ActionResult ErrorDataNull()
        {
            return View();
        }
    }
}