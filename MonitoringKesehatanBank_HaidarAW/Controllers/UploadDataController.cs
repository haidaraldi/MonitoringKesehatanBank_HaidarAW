using LinqToExcel;
using MonitoringKesehatanBank_HaidarAW.CustomFilters;
using MonitoringKesehatanBank_HaidarAW.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonitoringKesehatanBank_HaidarAW.Controllers
{
    [AuthLog(Roles = "superadmin")]
    public class UploadDataController : Controller
    {
        private AppMonitoringKesehatanBankEntities db = new AppMonitoringKesehatanBankEntities();
        // GET: UploadData
        public ActionResult Index()
        {
            return View();
        }
        public FileResult DownloadExcel()
        {
            string path = "/Template/TemplateExcel.xlsx";
            return File(path, "application/vnd.ms-excel", "TemplateExcel.xlsx");
        }

        [HttpPost]
        public ActionResult UploadExcel(DataKesehatan DataKesehatan, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Sheet1";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var dataKesehatans = from a in excelFile.Worksheet<DataKesehatan>(sheetName) select a;

                    foreach (var a in dataKesehatans)
                    {
                        try
                        {
                            var validasiSandiBank = db.DataKesehatan.FirstOrDefault(x => x.SandiBank == a.SandiBank);

                            if (validasiSandiBank != null)
                            {
                                return RedirectToAction("ErrorDataExists", "Message");
                            }

                            if (a.SandiBank != "" && a.Periode != "" && a.KreditKol1.ToString() != "" && a.KreditKol2.ToString() != "" && a.KreditKol3.ToString() != "" && a.KreditKol4.ToString() != "" && a.KreditKol5.ToString() != "" && a.Laba.ToString() != "" && a.TotalAset.ToString() != "" && a.ATMR.ToString() != "" && a.BebanOperasional.ToString() != "" && a.PendapatanOperasional.ToString() != "" && a.DanaPihakKe3.ToString() != "")
                            {
                                DataKesehatan dk = new DataKesehatan();
                                dk.SandiBank = a.SandiBank;
                                dk.Periode = a.Periode;
                                dk.KreditKol1 = a.KreditKol1;
                                dk.KreditKol2 = a.KreditKol2;
                                dk.KreditKol3 = a.KreditKol3;
                                dk.KreditKol4 = a.KreditKol4;
                                dk.KreditKol5 = a.KreditKol5;
                                dk.Laba = a.Laba;
                                dk.Modal = a.Modal;
                                dk.TotalAset = a.TotalAset;
                                dk.ATMR = a.ATMR;
                                dk.BebanOperasional = a.BebanOperasional;
                                dk.PendapatanOperasional = a.PendapatanOperasional;
                                dk.DanaPihakKe3 = a.DanaPihakKe3;
                                dk.NPL = (a.KreditKol3 + a.KreditKol4 + a.KreditKol5) / (a.KreditKol1 + a.KreditKol2 + a.KreditKol3 + a.KreditKol4 + a.KreditKol5) * 100;
                                dk.ROE = (a.Laba / a.Modal) * 100;
                                dk.ROA = (a.Laba / a.TotalAset) * 100;
                                dk.CAR = (a.Modal / a.ATMR) * 100;
                                dk.BOPO = (a.BebanOperasional / a.PendapatanOperasional) * 100;
                                dk.LDR = ((a.KreditKol1 + a.KreditKol2 + a.KreditKol3 + a.KreditKol4 + a.KreditKol5) / a.DanaPihakKe3) * 100;

                                if(dk.NPL > 5)
                                {
                                    dk.TotalRedColor = +1;
                                }
                                if(dk.ROE <= 5 || dk.ROA < 1)
                                {
                                    dk.TotalRedColor = dk.TotalRedColor + 1;
                                }
                                if(dk.CAR < 20)
                                {
                                    dk.TotalRedColor = dk.TotalRedColor + 1;
                                }
                                if(dk.BOPO > 80)
                                {
                                    dk.TotalRedColor = dk.TotalRedColor + 1;
                                }
                                if(dk.LDR > 94 || dk.LDR < 82)
                                {
                                    dk.TotalRedColor = dk.TotalRedColor + 1;
                                }

                                db.DataKesehatan.Add(dk);

                                db.SaveChanges();

                            }
                            else
                            {
                                if (a.SandiBank == "" || a.SandiBank == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.Periode == "" || a.Periode == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.KreditKol1.ToString() == "" || a.KreditKol1.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.KreditKol2.ToString() == "" || a.KreditKol2.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.KreditKol3.ToString() == "" || a.KreditKol3.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.KreditKol4.ToString() == "" || a.KreditKol4.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.KreditKol5.ToString() == "" || a.KreditKol5.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.Laba.ToString() == "" || a.Laba.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.Modal.ToString() == "" || a.Modal.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.TotalAset.ToString() == "" || a.TotalAset.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.ATMR.ToString() == "" || a.ATMR.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.BebanOperasional.ToString() == "" || a.BebanOperasional.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.PendapatanOperasional.ToString() == "" || a.PendapatanOperasional.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }
                                if (a.DanaPihakKe3.ToString() == "" || a.DanaPihakKe3.ToString() == null)
                                {
                                    return RedirectToAction("ErrorDataNull", "Message");
                                }

                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("Index", "DataKinerja");
                }
                else
                {
                    //alert message for invalid file format  
                    return RedirectToAction("ErrorFile", "Message");
                }
            }
            else
            {
                return RedirectToAction("ErrorNullUpload", "Message");
            }
        }

    }
}