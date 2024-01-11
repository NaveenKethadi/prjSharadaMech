using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using prjSharadaMech.Models;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;

namespace prjSharadaMech.Controllers
{
    public class FileImportController : Controller
    {
        // GET: FileImport
        FileInfo fi = null;
        string msg = "";
        string errors1 = "";
        ClsDataAcceslayer obj = new ClsDataAcceslayer();
        public ActionResult XLImport(int companyId)
        {
            ViewBag.companyId = companyId;
            return View();
        }
        [HttpPost]
        public ActionResult Exsheets(FormCollection formCollection)
        {
            try
            {
                string sessionid = "";
                string Message = "";
                int vtype = 0;
                HttpFileCollectionBase file = Request.Files;
                int compId = Convert.ToInt32(formCollection["CompanyId"]);
                HttpPostedFileBase pfile = Request.Files[0];
                //string fl = formCollection["file"];
                sessionid = GetSessionId(compId);
                fi = new FileInfo(pfile.FileName);
                if (fi == null)
                {
                    msg = "Please select valid excel file";
                    return Json(new { status = false, Messgage = msg, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    var fileName = Path.GetFileName(fi.FullName);
                    //  var filePath = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                    //  file.SaveAs(filePath);
                    var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
                    var dirInfo = new DirectoryInfo(folderPath);
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }
                    string filePath = Path.Combine(folderPath, fileName);
                    pfile.SaveAs(filePath);
                    fi = new FileInfo(filePath);
                    using (var workbook = new XLWorkbook(pfile.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1); // Select the first worksheet

                        // Iterate over the rows and columns to read the data
                        var range = worksheet.RangeUsed();
                        var rows = range.Rows();
                        //  int rowCount = worksheet.Dimension.Rows;
                        var columns = range.Columns();
                        //foreach (var row in rows)
                        //{
                        //    foreach (var cell in row.Cells())
                        //    {
                        //        // Access the cell value
                        //        var cellValue = cell.Value;


                        //    }
                        //}
                        int rowCount = range.RowCount();
                        int columnCount = range.ColumnCount();
                        List<BOMModel> bommodellst = new List<BOMModel>();



                        // Iterate over the rows and columns using nested for loops
                        for (int row = 2; row <= rowCount; row++)
                        {
                            BOMModel bommodelsin = new BOMModel();
                            for (int column = 1; column <= columnCount; column++)
                            {

                                var cellValue = worksheet.Cell(row, column).Value;

                                // Set the corresponding property of the model object
                                // based on the column index or any other criteria
                                switch (column)
                                {
                                    case 1:
                                        bommodelsin.DocNo = cellValue.ToString();
                                        break;
                                    case 2:
                                        bommodelsin.Date = cellValue.ToString();
                                        break;
                                    case 3:
                                        bommodelsin.Time = cellValue.ToString();
                                        break;
                                    case 4:
                                        bommodelsin.sNarration = cellValue.ToString();
                                        break;
                                    case 5:
                                        bommodelsin.VersionNo = cellValue.ToString();
                                        break;
                                    case 6:
                                        bommodelsin.Item = cellValue.ToString();
                                        break;
                                    case 7:
                                        bommodelsin.Description = cellValue.ToString();
                                        break;
                                    case 8:
                                        bommodelsin.Unit = cellValue.ToString();
                                        break;
                                    case 9:
                                        bommodelsin.Quantity = cellValue.ToString();
                                        break;
                                    case 10:
                                        bommodelsin.Length = cellValue.ToString();
                                        break;
                                    case 11:
                                        bommodelsin.Width = cellValue.ToString();
                                        break;
                                    case 12:
                                        bommodelsin.Number = cellValue.ToString();
                                        break;
                                    case 13:
                                        bommodelsin.UnitConv = cellValue.ToString();
                                        break;
                                    case 14:
                                        bommodelsin.ReqQty = cellValue.ToString();
                                        break;
                                    case 15:
                                        bommodelsin.Rate = cellValue.ToString();
                                        break;
                                    case 16:
                                        bommodelsin.Gross = cellValue.ToString();
                                        break;
                                    case 17:
                                        bommodelsin.TypeofItem = cellValue.ToString();
                                        break;
                                    case 18:
                                        bommodelsin.OutputItem = cellValue.ToString();
                                        break;
                                        //case 18:
                                        //    bommodelsin.Property2 = cellValue.ToString();
                                        //    break;
                                }

                            }
                            bommodellst.Add(bommodelsin);
                        }
                        string vouqry = $@"select iVoucherType from cCore_Vouchers_0 where sName='BOM Master'";
                        DataSet dsvouqry = ClsDataAcceslayer.GetData1(vouqry, compId, ref errors1);
                        if (dsvouqry.Tables[0].Rows.Count > 0 && dsvouqry != null)
                        {
                            vtype = Convert.ToInt32(dsvouqry.Tables[0].Rows[0]["iVoucherType"]);
                        }
                        var bomgrpbylst = bommodellst.GroupBy(s => s.DocNo).ToList();
                        string qryfieldids = $@"select cvs.iFieldId,sCaption from cCore_VoucherScreenFields_0 cvs
                                                 join cCore_Fields sf on cvs.iUniqueId=sf.iFieldId
                                                   where iVoucherType={vtype}";
                        DataSet dsfieldids = ClsDataAcceslayer.GetData1(qryfieldids, compId, ref errors1);
                        Fieldids fids = new Fieldids();
                        if (dsfieldids.Tables[0].Rows.Count > 0 && dsfieldids != null)
                        {
                            for (int k = 0; k < dsfieldids.Tables[0].Rows.Count; k++)
                            {
                                switch (k)
                                {
                                    case 0:
                                        fids.Lengthfid = Convert.ToInt32(dsfieldids.Tables[0].Rows[k]["iFieldId"]);
                                        break;
                                    case 1:
                                        fids.Widthfid = Convert.ToInt32(dsfieldids.Tables[0].Rows[k]["iFieldId"]);
                                        break;
                                    case 2:
                                        fids.Numberfid = Convert.ToInt32(dsfieldids.Tables[0].Rows[k]["iFieldId"]);
                                        break;
                                    case 3:
                                        fids.ReqQtyfid = Convert.ToInt32(dsfieldids.Tables[0].Rows[k]["iFieldId"]);
                                        break;
                                    case 4:
                                        fids.UnitConvfid = Convert.ToInt32(dsfieldids.Tables[0].Rows[k]["iFieldId"]);
                                        break;

                                }

                            }
                        }

                        foreach (var items in bomgrpbylst)
                        {
                            Hashtable header1 = new Hashtable();
                            Hashtable header2 = new Hashtable();
                            List<Hashtable> body1 = new List<Hashtable>();
                            List<Hashtable> body2 = new List<Hashtable>();
                            int i = 0;
                            int j = 0;
                            var headlist = items.FirstOrDefault();
                            LoadVouBOM.Root ldvouch = GetBOMMData(items.Key, sessionid, compId);
                            if (ldvouch.result == 1)
                            {
                                var datavou = ldvouch.data;
                                var hId = datavou[0].Header.HeaderId;
                                var bdvoucnt = datavou[0].Body.Count;
                                var verno = "";
                                if (headlist.VersionNo == "")
                                {
                                    verno = datavou[0].Header.VersionNo;
                                }
                                #region Header

                                header1 = new Hashtable
                                        {
                                            //{ "DocNo", VoucherExistsVoucher},
                                            { "HeaderId", hId },
                                            { "Date", headlist.Date },
                                            { "VersionNo",verno}
                                            //{ "sNarration", CIssue.Narration },
                                            //{ "CBJWIssueNo_",DocNo }
                                        };
                                #endregion
                                foreach (var item in items)
                                {
                                    //if (item.OutputItem == items.Key)
                                    //{
                                    var bdxlcount = items.Count();
                                    var unit = 0;
                                    //var qty = 0.0M;
                                    //var lenght = 0.0M;
                                    //var width = 0.0M;
                                    //var num = 0.0M;
                                    //var unitconv = 0.0M;
                                    //var rqty = 0.0M;
                                    //var rate = 0.0M;
                                    if (item.Unit != "")
                                    {
                                        unit = Convert.ToInt32(item.Unit);
                                    }

                                    Hashtable row1 = new Hashtable();
                                    row1 = new Hashtable
                                                   {
                                                        {"Item__Code", item.Item },
                                                      //  {"OutputItem__Code", item.OutputItem },
                                                        //{"Unit", data.ElementAt(d).Unit },
                                                        {"Quantity", item.Quantity },
                                                        {"Rate", item.Rate},
                                                        {"TypeofItem", item.TypeofItem },
                                                      //  {"TransactionId",ldvouch.data[0].Body[0].TransactionId },
                                                        //{"Taxable Value", gross+data.ElementAt(d).OtherCharges},
                                                        //{"CGST",data.ElementAt(d).CGST },
                                                        //{"SGST",data.ElementAt(d).SGST },
                                                        //{"IGST",data.ElementAt(d).IGST },
                                                        //{"Total Rate" ,gross+data.ElementAt(d).OtherCharges}
                                                    };
                                    if (i == 0)
                                    {
                                        row1.Add("OutputItem__Code", item.OutputItem);
                                    }
                                    Hashtable Lenght = new Hashtable()
                                        {
                                            { "Input",item.Length},
                                            { "FieldId",ldvouch.data[0].Body[0].LengthMtr.FieldId},
                                            { "Value",item.Length},
                                         };
                                    Hashtable Width = new Hashtable()
                                        {
                                            { "Input",item.Width},
                                            { "FieldId",ldvouch.data[0].Body[0].WidthMtr.FieldId},
                                            { "Value",item.Width},
                                         };
                                    Hashtable Number = new Hashtable()
                                        {
                                            { "Input",item.Number},
                                            { "FieldId",ldvouch.data[0].Body[0].Number.FieldId},
                                            { "Value",item.Number},
                                         };
                                    Hashtable UnitConv = new Hashtable()
                                        {
                                            { "Input",item.UnitConv},
                                            { "FieldId",ldvouch.data[0].Body[0].UnitConv.FieldId},
                                            { "Value",item.UnitConv},
                                         };
                                    //Length (Mtr)
                                    row1.Add("Length (Mtr)", Lenght);
                                    //Width (Mtr)
                                    row1.Add("Width (Mtr)", Width);
                                    //Number
                                    row1.Add("Number", Number);
                                    //Unit Conv
                                    row1.Add("Unit Conv", UnitConv);
                                    body1.Add(row1);
                                    i++;
                                    //  }
                                }
                                var postingData1 = new PostingData();
                                postingData1.data.Add(new Hashtable { { "Header", header1 }, { "Body", body1 } });
                                var sContent11 = JsonConvert.SerializeObject(postingData1);
                                obj.SetLog("Posting Updating sContent" + sContent11);
                                obj.SetLog("Before Response");
                                var response1 = Focus8API.Post("http://localhost/focus8API/Transactions/" + vtype, sContent11, sessionid, ref errors1);
                                if (response1 != null)
                                {
                                    var responseData1 = JsonConvert.DeserializeObject<APIResponse.PostResponse>(response1);
                                    if (responseData1.result == -1)
                                    {
                                        Message = $"Posting Failed : {responseData1.message }";
                                        obj.SetLog("Error:" + Message);
                                    }
                                    else
                                    {
                                        var soNo = Convert.ToString(responseData1.data[0]["VoucherNo"]);

                                        Message += $"Updated into DocNo = {soNo} Successfully\n";
                                        // Message = $"Consumption Job Work - '{soNo}' Posted Successfully";


                                    }
                                }
                                //   }
                            }
                            if (ldvouch.result == -1)
                            {
                                long dt = 0;
                                string qrydateint = $@"select dbo.DateToInt(getdate()) date";
                                DataSet dsdateint = ClsDataAcceslayer.GetData1(qrydateint, compId, ref errors1);
                                if (dsdateint.Tables[0].Rows.Count > 0 && dsdateint != null)
                                {
                                    dt = Convert.ToInt64(dsdateint.Tables[0].Rows[0]["date"]);
                                }
                                #region Header
                                header2 = new Hashtable
                                        {
                                            { "DocNo", headlist.DocNo},
                                          //  { "HeaderId", hId },
                                            { "Date", dt },
                                            { "VersionNo",headlist.VersionNo}
                                            //{ "sNarration", CIssue.Narration },
                                            //{ "CBJWIssueNo_",DocNo }
                                        };
                                #endregion
                                foreach (var item in items)
                                {

                                    // var bdxlcount = items.Count();
                                    // var unit = 0;
                                    //var qty = 0.0M;
                                    //var lenght = 0.0M;
                                    //var width = 0.0M;
                                    //var num = 0.0M;
                                    //var unitconv = 0.0M;
                                    //var rqty = 0.0M;
                                    //var rate = 0.0M;
                                    //if (item.Unit != "")
                                    //{
                                    //    unit = Convert.ToInt32(item.Unit);
                                    //}

                                    Hashtable row2 = new Hashtable();
                                    row2 = new Hashtable
                                                   {
                                                        {"Item__Code", item.Item },
                                                      //  {"OutputItem__Code", item.OutputItem },
                                                        //{"Unit", data.ElementAt(d).Unit },
                                                        {"Quantity", item.Quantity },
                                                        {"Rate", item.Rate},
                                                        {"TypeofItem", item.TypeofItem },
                                                      //  {"TransactionId",ldvouch.data[0].Body[0].TransactionId },
                                                        //{"Taxable Value", gross+data.ElementAt(d).OtherCharges},
                                                        //{"CGST",data.ElementAt(d).CGST },
                                                        //{"SGST",data.ElementAt(d).SGST },
                                                        //{"IGST",data.ElementAt(d).IGST },
                                                        //{"Total Rate" ,gross+data.ElementAt(d).OtherCharges}
                                                    };
                                    if (j == 0)
                                    {
                                        row2.Add("OutputItem__Code", item.OutputItem);
                                    }
                                    Hashtable Lenght = new Hashtable()
                                        {
                                            { "Input",item.Length},
                                            { "FieldId",fids.Lengthfid},
                                            { "Value",item.Length},
                                         };
                                    Hashtable Width = new Hashtable()
                                        {
                                            { "Input",item.Width},
                                            { "FieldId",fids.Widthfid},
                                            { "Value",item.Width},
                                         };
                                    Hashtable Number = new Hashtable()
                                        {
                                            { "Input",item.Number},
                                            { "FieldId",fids.Numberfid},
                                            { "Value",item.Number},
                                         };
                                    Hashtable UnitConv = new Hashtable()
                                        {
                                            { "Input",item.UnitConv},
                                            { "FieldId",fids.UnitConvfid},
                                            { "Value",item.UnitConv},
                                         };
                                    //Length (Mtr)
                                    row2.Add("Length (Mtr)", Lenght);
                                    //Width (Mtr)
                                    row2.Add("Width (Mtr)", Width);
                                    //Number
                                    row2.Add("Number", Number);
                                    //Unit Conv
                                    row2.Add("Unit Conv", UnitConv);
                                    body2.Add(row2);
                                    j++;
                                }
                                var postingData2 = new PostingData();
                                postingData2.data.Add(new Hashtable { { "Header", header2 }, { "Body", body2 } });
                                var sContent2 = JsonConvert.SerializeObject(postingData2);
                                obj.SetLog("Posting Updating sContent" + sContent2);
                                obj.SetLog("Before Response");
                                var response2 = Focus8API.Post("http://localhost/focus8API/Transactions/" + vtype, sContent2, sessionid, ref errors1);
                                if (response2 != null)
                                {
                                    var responseData = JsonConvert.DeserializeObject<APIResponse.PostResponse>(response2);
                                    if (responseData.result == -1)
                                    {
                                        Message = $"Posting Failed : {responseData.message }\n";
                                        obj.SetLog("Error:" + Message);
                                    }
                                    else
                                    {
                                        var soNo = Convert.ToString(responseData.data[0]["VoucherNo"]);

                                        Message += $"Posted DocNo = {soNo} successfully\n";
                                        // Message = $"Consumption Job Work - '{soNo}' Posted Successfully";


                                    }
                                }
                            }
                            // }
                        }
                        //var postingData = new PostingData();
                        //postingData.data.Add(new Hashtable { { "Header", header }, { "Body", body } });
                        //var sContent1 = JsonConvert.SerializeObject(postingData);
                        //obj.SetLog("Posting Updating sContent" + sContent1);
                        //obj.SetLog("Before Response");
                        //var response = Focus8API.Post("http://localhost/focus8API/Transactions/"+vtype, sContent1, sessionid, ref errors1);
                        //if (response != null)
                        //{
                        //    var responseData = JsonConvert.DeserializeObject<APIResponse.PostResponse>(response);
                        //    if (responseData.result == -1)
                        //    {
                        //        Message = $"Posting Failed : {responseData.message } \n";
                        //        obj.SetLog("Error:" + Message);
                        //    }
                        //    else
                        //    {
                        //        var soNo = Convert.ToString(responseData.data[0]["VoucherNo"]);

                        //        Message = "Posted or Updated Successfully";
                        //        // Message = $"Consumption Job Work - '{soNo}' Posted Successfully";


                        //    }
                        //}
                    }
                }
                return Json(new { status = true, Message = Message });
            }
            catch (Exception ex)
            {
                obj.SetLog("Error:" + ex.Message);
                return Json(new { status = false, Message = ex.Message });

            }

            //return View();
        }
        public LoadVouBOM.Root GetBOMMData(string VoucherNo, string SessionId, int CompanyId)
        {

            LoadVouBOM.Root model = new LoadVouBOM.Root();
            string Vno = VoucherNo.Replace("/", "~~");
            string URL = "http://localhost/focus8api/Screen/Transactions/7938/" + Vno;
            var response = Focus8API.Get(URL, SessionId, ref errors1);
            if (response != null)
            {
                model = JsonConvert.DeserializeObject<LoadVouBOM.Root>(response);
            }
            return model;
        }
        public string GetSessionId(int CompId)
        {
            string sSessionId = "";
            try
            {
                string strServer = BL_Configdata.ServerAPIIP;
                //  obj.SetLog("strServer" + strServer);
                int ccode = CompId;
                string User_Name = BL_Configdata.UserName;
                string Password = BL_Configdata.Password;

                //  obj.SetLog("ccode" + ccode);
                //  obj.SetLog("User_Name" + User_Name);
                //  obj.SetLog("Password" + Password);

                //obj.SetLog("Session Link: " + strServer + " / Login");
                //var httpWebRequest = (HttpWebRequest)WebRequest.Create(strServer + "/Login");
                // obj.SetLog("Session Link: " + strServer + " / Login");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + strServer + "/focus8api/Login");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{" + "\"data\": [{" + "\"Username\":\"" + User_Name + "\"," + "\"password\":\"" + Password + "\"," + "\"CompanyId\":\"" + ccode + "\"}]}";
                    //  obj.SetLog("Session json: " + json);
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader Updatereader = new StreamReader(httpResponse.GetResponseStream());
                string Udtcontent = Updatereader.ReadToEnd();

                JObject odtbj = JObject.Parse(Udtcontent);
                Temperatures Updtresult = JsonConvert.DeserializeObject<Temperatures>(Udtcontent);
                if (Updtresult.Result == 1)
                {
                    sSessionId = Updtresult.Data[0].FSessionId;
                }


                return sSessionId;
            }
            catch (Exception ex)
            {
                obj.SetLog(ex.ToString());
            }
            return sSessionId;
        }


    }
}
