using prjSharadaMech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjSharadaMech.Controllers
{
    public class BOMMenuController : Controller
    {
        // GET: BOMMenu
        string error = "";
        ClsDataAcceslayer obj = new ClsDataAcceslayer();
        public ActionResult BOMMenuInd(int companyId)
        {
            ViewBag.compid = companyId;
            ViewBag.Document = GetDocument(companyId);
            ViewBag.Output= GetOutput(companyId);
            return View();
        }
        public IEnumerable<SelectListItem> GetDocument(int companyId)
        {
            List<SelectListItem> vlist = new List<SelectListItem>();
            // vlist.Add(new SelectListItem { Value = "0", Text = "--select--" });
            //fetching Vendor type
            string splistQry = $@"select sVoucherNo from tCore_Header_0 h --join tCore_Data_0 d on h.iHeaderId=d.iHeaderId
                                    where iVoucherType=7938";
            DataSet ds = ClsDataAcceslayer.GetData1(splistQry, companyId, ref error);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                vlist.Add(new SelectListItem()
                {
                    Value = ds.Tables[0].Rows[i]["sVoucherNo"].ToString(),
                    Text = ds.Tables[0].Rows[i]["sVoucherNo"].ToString()
                });
            }

            return new SelectList(vlist.AsEnumerable(), "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetOutput(int companyId)
        {
            List<SelectListItem> otlist = new List<SelectListItem>();
            //otlist.Add(new SelectListItem { Value = "0", Text = "--select--" });
            //fetching outlet group
            string splistQry = $@"select distinct sVoucherNo,OutputItem,mp.sName OPItem from tCore_Header_0 h join tCore_Data_0 d on h.iHeaderId=d.iHeaderId--distinct sVoucherNo,iProduct,
                                    join tCore_Indta_0 i on i.iBodyId=d.iBodyId
                                    join tCore_Data7938_0 dv on dv.iBodyId=d.iBodyId
                                    join mCore_Product mp on mp.iMasterId=OutputItem
                                    where iVoucherType=7938";
            DataSet ds = ClsDataAcceslayer.GetData1(splistQry, companyId, ref error);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                otlist.Add(new SelectListItem()
                {
                    Value = ds.Tables[0].Rows[i]["sVoucherNo"].ToString(),
                    Text = ds.Tables[0].Rows[i]["OPItem"].ToString()
                });
            }

            return new SelectList(otlist.AsEnumerable(), "Value", "Text");
        }
        //GetGrid
        public ActionResult GetGrid(int compid, List<string> selecteddocs, List<string> selectedops)//selecteddocs, selectedops
        {
            try
            {
                ClsDataAcceslayer obj = new ClsDataAcceslayer();
                List<string> doc = new List<string>();
                if(selecteddocs!=null)
                {
                    doc.AddRange(selecteddocs);// doc.AddRange(selecteddocs.Where(s => !string.IsNullOrEmpty(s)));
                }
                if(selectedops != null)
                {
                    doc.AddRange(selectedops);
                }
                List<ImportBOM> iblst = new List<ImportBOM>();
                foreach (var item in doc)
                {
                    
                    string q = $@"select distinct sVoucherNo,mp.sName output,OutputItem from tCore_Header_0 h 
                            join tCore_Data_0 d on h.iHeaderId=d.iHeaderId
                            join tCore_Data7938_0 dv on dv.iBodyId=d.iBodyId
                            join mCore_Product mp on mp.iMasterId=OutputItem
                            where iVoucherType=7938 and sVoucherNo='{item}'";
                    DataSet ds = ClsDataAcceslayer.GetData1(q, compid, ref error);
                    if(ds!=null && ds.Tables[0].Rows.Count>0)
                    {
                        ImportBOM ib = new ImportBOM();
                        ib.DocNo = ds.Tables[0].Rows[0]["sVoucherNo"].ToString();
                        ib.OPItem= ds.Tables[0].Rows[0]["output"].ToString();
                        ib.OPItemId = Convert.ToInt32(ds.Tables[0].Rows[0]["OutputItem"]);
                        iblst.Add(ib);
                    }
                }
                return Json(new { status = true, iblst });
            }
            catch (Exception x)
            {
                return Json(new { status = false, Message = x.Message });
            }
        }
        //PostBOM
        public ActionResult PostBOM(List<ImportBOM> itemGrdlst, int compid)
        {
            try
            {
                string updt = "";string created = "";string vercreated = "";
                foreach (var item1 in itemGrdlst)
                {
                    string opitemname = "";
                    string opitemcode = "";
                    int opitemid = 0; decimal oqty = 0.0M; int odefbaseunit = 0; int variantid1 = 0;
                    int dreplinishment = 0; string narration = ""; int bomid = 0; int bomid1 = 0; long createdmodifieddate = 0; int variantid = 0;
                    string qry1 = $@"select iDate Date,isnull(sNarration,'') Narration,isnull(VersionNo,'') VersionNo,
                            mp1.iMasterId outputitemid,mp1.sCode [Output Item Code],mp1.sName [Output Item],mpr.iDefaultReplenishment,mpu1.iDefaultBaseUnit [Op Default Base Unit],mpu2.iDefaultPurchaseUnit [ip Default Base Unit],
                            mp.iMasterId inputitemid,mp.sCode [Input Item Code],mp.sName InputItem,isnull(mp.sName,'') Description,minput3 [RqQty],ind.fQuantity Quantity,ind.mRate Rate
                            from tCore_Header_0 h join tCore_Data_0 d on h.iHeaderId=d.iHeaderId
                            join tCore_HeaderData7938_0 thv on thv.iHeaderId=d.iHeaderId
                            join tCore_Data7938_0 tdv on tdv.iBodyId=d.iBodyId
                            join tCore_Indta_0 ind on ind.iBodyId=d.iBodyId
                            --join tCore_Data_Tags_0 tdt on tdt.iBodyId=d.iBodyId
                            join mCore_Product mp on mp.iMasterId=iProduct
                            join mCore_Product mp1 on mp1.iMasterId=tdv.OutputItem
                            join muCore_Product_Replenishment mpr on mpr.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu1 on mpu1.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu2 on mpu2.iMasterId=mp.iMasterId
                             --join mCore_Units mu on mu.iMasterId=iUnit
                            join tCore_IndtaBodyScreenData_0 indb on indb.iBodyId=d.iBodyId 
                            where iVoucherType=7938 and sVoucherNo='{item1.DocNo}' order by mp1.iMasterId desc";
                    DataSet ds1 = ClsDataAcceslayer.GetData1(qry1, compid, ref error);
                    List<BOMMasterModel> bommodellist = new List<Models.BOMMasterModel>();
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                        {
                            BOMMasterModel bommodelsin = new Models.BOMMasterModel();
                            bommodelsin.Date = Convert.ToInt32(ds1.Tables[0].Rows[i]["Date"]);
                            bommodelsin.Narration = ds1.Tables[0].Rows[i]["Narration"].ToString();
                            bommodelsin.Output_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["outputitemid"]);
                            bommodelsin.Output_Item_Code = ds1.Tables[0].Rows[i]["Output Item Code"].ToString();
                            bommodelsin.Output_Item = ds1.Tables[0].Rows[i]["Output Item"].ToString();
                            bommodelsin.DefaultReplinishment = Convert.ToInt32(ds1.Tables[0].Rows[i]["iDefaultReplenishment"]);
                            bommodelsin.Output_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["Op Default Base Unit"]);
                            bommodelsin.Input_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["ip Default Base Unit"]);
                            bommodelsin.Input_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["inputitemid"]);
                            bommodelsin.Input_Item_Code = ds1.Tables[0].Rows[i]["Input Item Code"].ToString();
                            bommodelsin.Input_Item = ds1.Tables[0].Rows[i]["InputItem"].ToString();
                            bommodelsin.Description = ds1.Tables[0].Rows[i]["Description"].ToString();
                            bommodelsin.Req_Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["RqQty"]);
                            bommodelsin.Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Quantity"]);
                            bommodelsin.Rate = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Rate"]);
                            bommodelsin.VersionNo= ds1.Tables[0].Rows[i]["VersionNo"].ToString();
                           // bommodelsin.Output_Item_Code= ds1.Tables[0].Rows[i]["Output Item Code"].ToString();
                            bommodellist.Add(bommodelsin);
                        }
                    }
                    var bommasterlst = bommodellist.FirstOrDefault();
                    opitemname = bommasterlst.Output_Item;
                    opitemcode = bommasterlst.Output_Item_Code;
                    opitemid = bommasterlst.Output_Item_Id;
                    dreplinishment = bommasterlst.DefaultReplinishment;
                    oqty = bommasterlst.Quantity;
                    narration = bommasterlst.Narration;
                    odefbaseunit = bommasterlst.Output_DefaultBaseUnit;
                    // string getbomid = $@"select bomh.iBomId from mMRP_BomHeader bomh join mMRP_BomVariantHeader
                    //          bomvar on bomh.iBomId=bomvar.iBomId where bomh.sName='{opitemname}' and bomvar.sName='{opitemname}'";
                    //checking for bom existance
                    string getbomid1 = $@"select bomh.iBomId from mMRP_BomHeader bomh join mMRP_BomVariantHeader
                          bomvar on bomh.iBomId=bomvar.iBomId and bomh.iStatus<>5 where bomh.sName='{opitemname}' --and bomvar.sName='{opitemname}'
                             and bomvar.iStatus<>5";
                    DataSet dsgetbomid1 = ClsDataAcceslayer.GetData1(getbomid1, compid, ref error);
                    if (dsgetbomid1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsgetbomid1.Tables[0].Rows.Count; i++)
                        {
                            bomid1 = Convert.ToInt32(dsgetbomid1.Tables[0].Rows[i]["iBomId"]);
                            //variantid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iVaraintId"]);
                        }
                    }
                    if (bomid1 > 0)
                    {
                       //checking for bom transaction
                        string getbomid = $@"select bh.iBomId,prodord.iVaraintId,bh.sName hName,bvh.sName vhname from 
                                     mMRP_BomVariantHeader bvh join mMRP_BomHeader bh on bvh.iBomId=bh.iBomId
                                     join tMrp_ProdOrder_0 prodord on prodord.iVaraintId=bvh.iVariantId
                                     where bh.sName='{opitemname}'";
                        DataSet dsgetbomid = ClsDataAcceslayer.GetData1(getbomid, compid, ref error);
                        if (dsgetbomid.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsgetbomid.Tables[0].Rows.Count; i++)
                            {
                                bomid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iBomId"]);
                                variantid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iVaraintId"]);
                            }
                        }
                        if (bomid > 0)
                        {
                            // create version for exisiting bom
                            // 
                            string versionfield = bommasterlst.VersionNo;
                            //long moddt = 0;
                            //string varname = ""; int n01 = 0; int n02 = 0;
                            //int count = 0;
                            int versionno = 0;
                            string getverqry = $@"select top 1 iVersion from mMRP_BomHeader where sName='{opitemname}' and iStatus<>5 order by iVersion desc";
                            DataSet dsver = ClsDataAcceslayer.GetData1(getverqry, compid, ref error);
                            if (dsver != null && dsver.Tables[0].Rows.Count > 0)
                            {
                                versionno = Convert.ToInt32(dsver.Tables[0].Rows[0]["iVersion"]);
                            }
                            string bomheaderqry = $@"insert into mMRP_BomHeader (sName,sCode,sDescription,bDefault,iType,iFrom,iTo,iSize,iVersion,bAuthorized,iCreatedBy,iModifiedBy,
                                     iCreatedDate,iModifiedDate,iLocationId,bEditedFrom,iEditingLocation,iStatus,iTagId,iTagValue,sRemarks,iBOMRateOptions,iBOMPriceBookId,iBOMPriceBookFieldId,iAuthStatus)
                                     values( '{opitemname}','{opitemcode}','{opitemname}',1,0,dbo.DateToInt(GETDATE()),dbo.DateToInt(GETDATE()),1,{versionno}+1,0,1,1,dbo.fCore_DateTimeToInt(GETDATE()),
                                     dbo.fCore_DateTimeToInt(GETDATE()),0,0,0,1,0,0,'{narration}',0,0,0,0)";
                            int n1 = obj.GetQueryExe(bomheaderqry, compid, ref error);
                            string getbomidqry = $@"select iBomId,iCreatedDate from mMRP_BomHeader where sName='{opitemname}' and iStatus<>5";
                            DataSet dsbomid = ClsDataAcceslayer.GetData1(getbomidqry, compid, ref error);
                            if (dsbomid.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsbomid.Tables[0].Rows.Count; i++)
                                {
                                    bomid = Convert.ToInt32(dsbomid.Tables[0].Rows[i]["iBomId"]);
                                    createdmodifieddate = Convert.ToInt64(dsbomid.Tables[0].Rows[i]["iCreatedDate"]);
                                }
                            }
                            string bomvariantheaderqry = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,
                                             iModifiedDate) Values('{opitemname}',0,{bomid},1,1,1,{createdmodifieddate},{createdmodifieddate})";
                            int n2 = obj.GetQueryExe(bomvariantheaderqry, compid, ref error);
                            string getvaridqry = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid}";
                            DataSet dsvarid = ClsDataAcceslayer.GetData1(getvaridqry, compid, ref error);
                            if (dsvarid.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsvarid.Tables[0].Rows.Count; i++)
                                {
                                    variantid = Convert.ToInt32(dsvarid.Tables[0].Rows[i]["iVariantId"]);
                                }
                            }
                            string bombobyopqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                    ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                    ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                    values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                            int n3 = obj.GetQueryExe(bombobyopqry, compid, ref error);
                            int n4 = 0;
                            foreach (var item in bommodellist)
                            {
                                string bombodyipqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                                         ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                                         ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                                          iInvTagValue) values({variantid},{item.Input_Item_Id},{item.Req_Quantity},{item.Req_Quantity},{item.Input_DefaultBaseUnit},0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{item.Rate},0,0,0,0,0)";
                                n4 = obj.GetQueryExe(bombodyipqry, compid, ref error);
                            }
                            if (n3 > 0 && n4 > 0)
                            {
                                if (versionfield == "")
                                {
                                    versionfield = opitemname + " V" + (versionno + 1).ToString();
                                }
                                else { versionfield = versionfield + "," + opitemname + " V" + (versionno + 1).ToString(); }
                                string verupdatqry = $@" update tCore_HeaderData7938_0 set VersionNo='{versionfield}',BOMStatus='CREATED' where iHeaderId=
                                         (select h.iHeaderId from tCore_Header_0 h join  tCore_HeaderData7938_0
                                   thv on thv.iHeaderId=h.iHeaderId where iVoucherType=7938 and sVoucherNo='{item1.DocNo}')";
                                int ver = obj.GetQueryExe(verupdatqry, compid, ref error);
                            }
                            if (error != "")
                            {
                                obj.SetLog(error);
                                throw new Exception(error);
                            }
                            obj.SetLog($"BOM '{opitemname}({opitemcode})' Version created with Version No:{versionno+1}");//and Version field:{versionfield}
                            string cv = opitemname+"("+(versionno + 1) + "),";
                            vercreated = vercreated + cv;
                        }
                        else
                        {
                            // update existing bom
                            //
                            string getvariantid1 = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid1} and iStatus<>5";
                            DataSet dsvarid1 = ClsDataAcceslayer.GetData1(getvariantid1, compid, ref error);
                            if (dsvarid1.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsvarid1.Tables[0].Rows.Count; i++)
                                {
                                    variantid = Convert.ToInt32(dsvarid1.Tables[0].Rows[i]["iVariantId"]);
                                }
                            }
                            string updateOqry = $@"update mMRP_BomHeader set sRemarks='{narration}',iType=0,iModifiedDate=dbo.fCore_DateTimeToInt(getdate()) 
                                      where ibomid={bomid1} and iStatus<>5
update mMRP_BomVariantHeader set iModifiedDate=dbo.fCore_DateTimeToInt(getdate()) where iVariantId={variantid} and ibomid={bomid1}
update mMRP_BOMBody set iProductId={opitemid},fQty={oqty},fQty2={oqty},iUnit={odefbaseunit},sDescription='{opitemname}' 
                                                                           where iVariantId={variantid} and binput=0";
                            int n0 = obj.GetQueryExe(updateOqry, compid, ref error);
                            List<int> varinputids = new List<int>();
                            string varipidqry = $@"select iBomBodyId from mMRP_BOMBody where iVariantId={variantid} and bInput=1";
                            DataSet dsvaripid = ClsDataAcceslayer.GetData1(varipidqry, compid, ref error);
                            if (dsvaripid.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsvaripid.Tables[0].Rows.Count; i++)
                                {
                                    varinputids.Add(Convert.ToInt32(dsvaripid.Tables[0].Rows[i]["iBomBodyId"]));
                                }
                            }
                           
                            for (int j = 0; j < bommodellist.Count; j++)
                            {
                                if (j < varinputids.Count)
                                {
                                    string updateIqry = $@"update mMRP_BOMBody set iProductId={bommodellist[j].Input_Item_Id},fQty={bommodellist[j].Req_Quantity},fQty2={bommodellist[j].Req_Quantity},iUnit={bommodellist[j].Input_DefaultBaseUnit},
                                          fRate={bommodellist[j].Rate},sDescription='{bommodellist[j].Input_Item}' where iVariantId={variantid} and binput=1 and iBomBodyId={varinputids[j]}";
                                    int n1 = obj.GetQueryExe(updateIqry, compid, ref error);
                                }
                                else
                                {
                                    //string insertbomvarhead = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,iModifiedDate) Values
                                    //                                            ('{bommodellist[j].Input_Item}',0,{BomId},1,1,1,{moddt},{moddt})";
                                    // int n00 = obj.GetQueryExe(insertbomvarhead, CompanyId, ref error);
                                    string bombodyipqry1 = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                                     ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                                     ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                                      iInvTagValue) values({variantid},{bommodellist[j].Input_Item_Id},{bommodellist[j].Req_Quantity},{bommodellist[j].Req_Quantity},{bommodellist[j].Input_DefaultBaseUnit},0.00,0,0,0,'{bommodellist[j].Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{bommodellist[j].Rate},0,0,0,0,0)";
                                    int n2 = obj.GetQueryExe(bombodyipqry1, compid, ref error);
                                }
                            }
                            string bomsts = $@"update tCore_HeaderData7938_0 set BOMStatus='CREATED' from 
                            tCore_Header_0 h join tCore_HeaderData7938_0 hv on h.iHeaderId=hv.iHeaderId
                            where sVoucherNo='{item1.DocNo}' and iVoucherType=7938";
                            int n = obj.GetQueryExe(bomsts, compid, ref error);
                            if (error != "")
                            {
                                obj.SetLog(error);
                                throw new Exception(error);
                            }
                            obj.SetLog($"BOM '{item1.OPItem}({opitemcode})' updated");
                            string ut = opitemname + ",";
                            updt = updt + ut;
                        }
                    }
                    // return Json(new { status = true, Message = "Saved into bill of matarial(BOM)" });
                    else
                    {
                        //creating new bom
                        #region BOMcreation
                        string bomheaderqry = $@"insert into mMRP_BomHeader (sName,sCode,sDescription,bDefault,iType,iFrom,iTo,iSize,iVersion,bAuthorized,iCreatedBy,iModifiedBy,
                                     iCreatedDate,iModifiedDate,iLocationId,bEditedFrom,iEditingLocation,iStatus,iTagId,iTagValue,sRemarks,iBOMRateOptions,iBOMPriceBookId,iBOMPriceBookFieldId,iAuthStatus)
                                     values( '{opitemname}','{opitemcode}','{opitemname}',1,0,dbo.DateToInt(GETDATE()),dbo.DateToInt(GETDATE()),1,0,0,1,1,dbo.fCore_DateTimeToInt(GETDATE()),
                                     dbo.fCore_DateTimeToInt(GETDATE()),0,0,0,1,0,0,'{narration}',0,0,0,0)";
                        int n1 = obj.GetQueryExe(bomheaderqry, compid, ref error);
                        string getbomidqry = $@"select iBomId,iCreatedDate from mMRP_BomHeader where sName='{opitemname}' and iStatus<>5";
                        DataSet dsbomid = ClsDataAcceslayer.GetData1(getbomidqry, compid, ref error);
                        if (dsbomid.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsbomid.Tables[0].Rows.Count; i++)
                            {
                                bomid = Convert.ToInt32(dsbomid.Tables[0].Rows[i]["iBomId"]);
                                createdmodifieddate = Convert.ToInt64(dsbomid.Tables[0].Rows[i]["iCreatedDate"]);
                            }
                        }
                        string bomvariantheaderqry = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,
                                             iModifiedDate) Values('{opitemname}',0,{bomid},1,1,1,{createdmodifieddate},{createdmodifieddate})";
                        int n2 = obj.GetQueryExe(bomvariantheaderqry, compid, ref error);
                        string getvaridqry = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid}";
                        DataSet dsvarid = ClsDataAcceslayer.GetData1(getvaridqry, compid, ref error);
                        if (dsvarid.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsvarid.Tables[0].Rows.Count; i++)
                            {
                                variantid = Convert.ToInt32(dsvarid.Tables[0].Rows[i]["iVariantId"]);
                            }
                        }
                        string bombobyopqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                    ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                    ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                    values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                        int n3 = obj.GetQueryExe(bombobyopqry, compid, ref error);
                        foreach (var item in bommodellist)
                        {
                            string bombodyipqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                                         ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                                         ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                                          iInvTagValue) values({variantid},{item.Input_Item_Id},{item.Req_Quantity},{item.Req_Quantity},{item.Input_DefaultBaseUnit},0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{item.Rate},0,0,0,0,0)";
                            int n4 = obj.GetQueryExe(bombodyipqry, compid, ref error);
                        }
                        //return Json(new { Message = "Saved into bill of matarial(BOM)" });
                        string bomsts = $@"update tCore_HeaderData7938_0 set BOMStatus='CREATED' from 
                            tCore_Header_0 h join tCore_HeaderData7938_0 hv on h.iHeaderId=hv.iHeaderId
                            where sVoucherNo='{item1.DocNo}' and iVoucherType=7938";
                        int n = obj.GetQueryExe(bomsts, compid, ref error);
                        if (error != "") {
                            obj.SetLog(error);
                            throw new Exception(error);
                        }
                        obj.SetLog($"New BOM '{opitemname}({opitemcode})' Created");
                        string cr = opitemname +",";
                        created = created + cr;
                        #endregion
                    }
                }
                obj.SetLog("Created BOMS: " + created.TrimEnd(',')); 
                obj.SetLog("Updated BOMS: " + updt.TrimEnd(','));
                obj.SetLog("Version created BOMS: " + vercreated.TrimEnd(','));
                return Json(new {status=true,Message="Posted BOM successfully" });
            }
            catch (Exception x)
            {
                obj.SetLog("Exception: " + x.StackTrace);
                return Json(new { status = false, Message = x.Message });
            }
        }
    }
}