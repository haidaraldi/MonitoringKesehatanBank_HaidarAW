using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MonitoringKesehatanBank_HaidarAW.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using MonitoringKesehatanBank_HaidarAW.CustomFilters;
using System.Text;
using System.IO;

namespace MonitoringKesehatanBank_HaidarAW.Controllers
{    
    [AuthLog(Roles = "superadmin")]
    public class DataKinerjaController : Controller
    {
        private AppMonitoringKesehatanBankEntities db = new AppMonitoringKesehatanBankEntities();

        // GET: DataKinerja
        public ActionResult Index(string Periode, string sortOrder, string currentFilter, int? page)
        {
            var PeriodeList = new List<string>();
            var PeriodeQuery = from d in db.DataKesehatan orderby d.Periode select d.Periode;

            PeriodeList.AddRange(PeriodeQuery.Distinct());
            ViewBag.Periode = new SelectList(PeriodeList);

            var periodes = from e in db.DataKesehatan select e;
            if (!string.IsNullOrEmpty(Periode))
            {
                periodes = periodes.Where(x => x.Periode == Periode);
            }

            ViewBag.CurrenSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (Periode != null)
            {
                page = 1;
            }
            else
            {
                Periode = currentFilter;
            }

            ViewBag.currentFilter = Periode;

            //tambah logika untuk sort
            switch (sortOrder)
            {
                case "name_desc":
                    periodes = periodes.OrderByDescending(s => s.SandiBank);
                    break;
                case "Date":
                    periodes = periodes.OrderBy(s => s.Periode);
                    break;
                default: //name ascending
                    periodes = periodes.OrderBy(s => s.SandiBank);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(periodes.ToPagedList(pageNumber, pageSize));
        }
        public void ExportToExcel(string Periode)
        {
            var PeriodeList = new List<string>();
            var PeriodeQuery = from d in db.DataKesehatan orderby d.Periode select d.Periode;

            PeriodeList.AddRange(PeriodeQuery.Distinct());
            ViewBag.Periode = new SelectList(PeriodeList);

            var periodes = from e in db.DataKesehatan select e;
            if (!string.IsNullOrEmpty(Periode))
            {
                periodes = periodes.Where(x => x.Periode == Periode);
            }

            string Filename = "Data Kinerja" + DateTime.Now.ToString("mm_dd_yyy_hh_ss_tt") + ".xls";
            string FolderPath = HttpContext.Server.MapPath("/ExcelFiles/");
            string FilePath = System.IO.Path.Combine(FolderPath, Filename);

            //Step-1: Checking: If file name exists in server then remove from server.
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.File.Delete(FilePath);
            }

            //Step-2: Get Html Data & Converted to String
            string HtmlResult = RenderRazorViewToString("~/Views/DataKinerja/GenerateExcel.cshtml", periodes.ToList());

            //Step-4: Html Result store in Byte[] array
            byte[] ExcelBytes = Encoding.ASCII.GetBytes(HtmlResult);

            //Step-5: byte[] array converted to file Stream and save in Server
            using (Stream file = System.IO.File.OpenWrite(FilePath))
            {
                file.Write(ExcelBytes, 0, ExcelBytes.Length);
            }

            //Step-6: Download Excel file 
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(Filename));
            Response.WriteFile(FilePath);
            Response.End();
            Response.Flush();
        }
        protected string RenderRazorViewToString(string viewName, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
